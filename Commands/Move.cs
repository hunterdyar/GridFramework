using System.Collections.Generic;
using System.Linq;
using Bloops.GridFramework.Agents;
using Bloops.GridFramework.Managers;
using Bloops.GridFramework.Navigation;
using UnityEngine;

namespace Bloops.GridFramework.Commands
{
	public class Move : ICommand
	{
		//Defines a move. 
		/// <summary>
		/// True when the instant a move has been called. Animations and so-on will still be "executing", the move has been executed.
		/// In other words, its been started.
		/// </summary>
		public bool executed { get; private set; }

		public bool HistoryPoint => _historyPoint;
		private bool _historyPoint;
		private PuzzleManager _puzzleManager;
		/// <summary>
		/// An executed move is not complete until various animations are finished.
		/// Future moves should not start until previous moves are all complete.
		/// </summary>
		private bool _isComplete;
		public bool IsComplete => (_afterMove!=null) ? _afterMove.IsComplete : (_isComplete);
		/// <summary>
		/// SubMoves, as they propogate, may be invalidated. Like trying to push a block that cannot move.
		/// if a submove is both moveCritical (default true) and invalid, the move is invalid and wont execute. 
		/// </summary>
		public bool IsValid
		{
			get;
			private set;
		}
		public bool cancelled { get; private set; }
		//Executed, valid, complete. AKA its done and we are moving on with our lives.
		public bool executedValidAndComplete => (executed && IsComplete && IsValid);
		public bool readyToMoveOn => (cancelled || executedValidAndComplete || !IsValid);

		private Move _afterMove;
		/// <summary>
		/// Called right before the move is completed, giving a chance for post-move reactions to initiate, like floor tiles and afterMoves (ie: floor conveyer belts).
		/// </summary>
		private Dictionary<AgentBase, SubMove> _allSubMoves = new Dictionary<AgentBase, SubMove>();//for undoing the move.
		
		//todo: AfterMoves. A move is not complete until it's chain of cause/effect moves are complete. So it fires a MoveComplete event, listening things may end up chaining an immediate next move.
		//if so, its complete status should become that moves complete status. like iscomplete = (nextMove == null) ? _isComplete || nextMove.complete.
		//
		
		public Move(PuzzleManager puzzleManager,bool historyPoint = true)
		{
			this._puzzleManager = puzzleManager;
			this._historyPoint = historyPoint;
			//set defaults.
			executed = false;
			_isComplete = false;
			IsValid = true;
		}

		public void Execute()
		{
			if (_allSubMoves.Count > 15)
			{
				Debug.LogWarning("high number of subMoves. Can we do something about this? The validity checks are O(n^2) fast.");
			}
			
			cancelled = false;
			_isComplete = false;
			CheckValid();
			
			//no agents got added? This isn't a move!
			if (_allSubMoves.Count == 0)
			{
				IsValid = false;
			}
			//
			if (IsValid)
			{
				foreach (var akp in _allSubMoves)
				{
					if (akp.Value.Valid)
					{
						akp.Key.MoveTo(akp.Value, this);
					}
				}
			}
			else
			{
				_isComplete = true;//we won't be triggering any animation. Move doesn't happen.
			}

			executed = true;
		}
		public void Redo()
		{
			foreach (var akp in _allSubMoves)
			{
				akp.Key.MoveToInstant(akp.Value,this);
				//akp.Key.SetFromLocationData(akp.Value.agentStartingData);
			}

			executed = true;
			_isComplete = true;
		}

		public void Undo()
		{

			//I wrote this check, and I think it fixed the issue of undo-ing during aftermooves, but...
			//feels too easy for a fairly complex system. *squinty eyes* investigate if the problem is really fixed.
			_afterMove?.Undo();
			if (!executed)
			{
				Debug.LogWarning("cant undo a move that hasnt been executed?");
				return;
			}
			if (!_isComplete)
			{
				//The animation hasn't completed, we need to either complete it now (before instantly undoing it)
				//or stop it wherever it is, and bail out of any loops.
				_puzzleManager.CallCancelMove(this);
			}
			
			//the actual undo:
			foreach (var akp in _allSubMoves)
			{
				akp.Key.SnapTo(akp.Value.startingNode);
				akp.Key.SetFromLocationData(akp.Value.agentStartingData);
			}
			cancelled = true;
			executed = false;
			_isComplete = false;
		}

		/// <param name="agentBase">The agent getting moved.</param>
		/// <param name="requiresValidSubMove">Leave null for default validity check. Otherwise, the subMove is only valid if this given submove is also valid. </param>
		/// <param name="criticalToMove">should an invalid move here invalidate the ENTIRE move</param>
		/// <param name="criticalForSubMove">if the submove gets invalidated, should THIS submove also get invalidated?</param>
		public SubMove AddAgentToMove(AgentBase agentBase, Vector3Int dir, bool criticalToMove = true,SubMove criticalForSubMove = null, SubMove reliesOnsubMove = null,int subMovePriority = 0)
		{
			SubMove subMove = null;
			if (agentBase.puzzleManager.tilemapNavigation.GetNode(agentBase.CurrentNode.cellPos + dir, out NavNode destinationNode))
			{
				
				if (agentBase.CanMoveToNode(destinationNode))
				{
					subMove = new SubMove(agentBase,agentBase.CurrentNode,agentBase.GetLocationData(), destinationNode, dir,criticalToMove,criticalForSubMove,reliesOnsubMove, subMovePriority);

					if (_allSubMoves.ContainsKey(agentBase))
					{
						if (subMovePriority > _allSubMoves[agentBase].subMovePriority)
						{
							_allSubMoves[agentBase].Invalidate();
							_allSubMoves[agentBase] = subMove;
							_puzzleManager.CallNewSubMove(this, subMove);
						}
					}
					else
					{
						_allSubMoves[agentBase] = subMove;
						_puzzleManager.CallNewSubMove(this, subMove);
					}
				}
			}
			//after the move has been configured (here), it needs to be executed (by the command manager).
			//this is for undo/redo support.
			return subMove;
		}

		public void RegisterAfterMove(Move afterMove, bool executeImmediately = true)
		{
			if (afterMove != null)
			{
				_afterMove = afterMove;
				if (executeImmediately)
				{
					//ExecuteAfterMove();
				}
			}
		}

		public void ExecuteAfterMove()
		{
			if (executed && !cancelled && _isComplete && IsValid)
			{
				if (_afterMove != null)
				{
					if (!_afterMove.executed)
					{
						_afterMove.Execute();
					}
				}
			}
		}


		private void CheckValid()
		{
			
			//we need tocheck if multiple elements involved share the sameAgent.
			foreach (var akp in _allSubMoves)
			{
				if (!akp.Value.Valid)
				{
					continue;
				}
				
				//check selves against priorities of others.
				foreach (var akp2 in _allSubMoves.Where(akp2 => akp2.Value.Valid).Where(akp2 => akp.Value != akp2.Value && akp.Value.destinationNode == akp2.Value.destinationNode))
				{
					if (akp.Value.subMovePriority > akp2.Value.subMovePriority)
					{
						akp2.Value.Invalidate();
					}else if (akp.Value.subMovePriority < akp2.Value.subMovePriority)
					{
						akp.Value.Invalidate();
					}
					else
					{
						Debug.LogWarning("Two moves conflicted for "+akp.Value.agent.name +" and "+akp2.Value.agent.name+". Invalidating the move. Investigate the situation that got you here and handle the weird edge case please.");
						IsValid = false;
						return;
						// akp.Value.Invalidate();
						// akp2.Value.Invalidate();
					}
				}
				
			}

			foreach (var akp in _allSubMoves)
			{
				
				if (akp.Value.reliesOnAgentMoving.Count > 0)
				{
					bool reliesOnAgentThatIsntMoving = true;
					foreach (var agent in akp.Value.reliesOnAgentMoving)
					{
						if (_allSubMoves.ContainsKey(agent))
						{
							if (_allSubMoves[agent].Valid)
							{
								reliesOnAgentThatIsntMoving = false;
								break;
							}
						}
						else
						{
							reliesOnAgentThatIsntMoving = true;
							IsValid = false;
							return;
						}

						//There might be subMoves that have agent are are invalid, but we just need to find one that is valid.
						if (reliesOnAgentThatIsntMoving)
						{
							IsValid = false;
							return;
						}
					}
				}
			}

			foreach (var akp in _allSubMoves.Where(akp => !akp.Value.Valid))
			{
				akp.Value.criticalFor?.Invalidate();
			}
			foreach (var akp in _allSubMoves)
			{
				//next, lets go through and see which subMoves rely on which subMoves aren't valid.
				
				if (akp.Value.reliesOn.Count != 0 && !akp.Value.AllReliesOnIsValid())
				{
					akp.Value.Invalidate();
				}
			}
			
			//Finally, we have sequentally propagted through everything!
			//There is one optimizaiton todo... optimize
			//When we Invalidate SubMoves at the time of original propogation (in the tileAgent event), we can't recursively invalidate the ones they rely on.
			//Thats because the subMoves might end up replaced entirely as a different element tries to move an actor.
			//So an optimization is to write an InvalidateRecusrively or InvalidateTotally function that we only use here, in the once-dust-settles check.
			//that should remove the above two loops.
			foreach (var akp in _allSubMoves.Where(akp => !akp.Value.Valid && akp.Value.Critical))
			{
				IsValid = false;
			}
		}

		public SubMove GetSubMoveWithDestination(NavNode destinationNode)
		{
			foreach (var sm in _allSubMoves.Values)
			{
				if (sm.destinationNode == destinationNode && sm.Valid)
				{
					return sm;
				}
			}
			return null;
		}
		public bool SubMoveHasDestination(NavNode destinationNode)
		{
			foreach (var sm in _allSubMoves.Values)
			{
				if (sm.destinationNode == destinationNode && sm.Valid)
				{
					return true;
				}
			}

			return false;
		}

		//todo rename executiong to animating or such.
		public void AgentFinishedAnimatingMove(AgentBase agentBase)
		{
			if (_allSubMoves.ContainsKey(agentBase))
			{
				_allSubMoves[agentBase].Finish();
			}
			_isComplete = !IsExecuting();//only update this when it changes.
			
			//When the last agent has completed their animation. 
			if (_isComplete)
			{
				if (executedValidAndComplete)
				{
					_puzzleManager.CallPreMoveComplete(this);
					//execute afterMove
					if (IsValid && _afterMove != null)
					{
						ExecuteAfterMove();
					}
					_puzzleManager.CallPostMoveComplete(this);
				}
			}
			
		}

		public List<SubMove> GetValidSubMoves()
		{
			//todo investigate alternites to linq that are less overhead heavy? is that a problem?
			return _allSubMoves.Values.Where(x => x.Valid == true).ToList();
			//return new List<SubMove>();
		}
		public bool IsInvolved(AgentBase agentBase)
		{
			return _allSubMoves.ContainsKey(agentBase);
		}

		bool IsExecuting()
		{
			foreach(var akp in _allSubMoves)
			{
				if (akp.Value.Valid && akp.Value.finishedExecuting != true)
				{
					//any valid move (actually got executed) is finished.
					//for what its worth, we also set finishedExecuting to true when we invalidate a move
					//just so i can fix this issue TWICE.
					return true;
				}
			}
			return false;
		}

		public bool GetValidSubMove(AgentBase agentBase,out SubMove subMove)
		{
			if (_allSubMoves.ContainsKey(agentBase))
			{
				if (_allSubMoves[agentBase].Valid)
				{
					subMove = _allSubMoves[agentBase];
					return true;
				}	
			}

			subMove = null;
			return false;
		}
	}
}