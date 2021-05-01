using Bloops.GridFramework.Agents;
using Bloops.GridFramework.Commands;
using Bloops.GridFramework.Managers;
using Bloops.GridFramework.Navigation;
using UnityEngine;
using UnityEngine.Assertions;

namespace Bloops.GridFramework.Items
{
	public class ItemBase : NavObject
	{
		protected AgentBase occupyingAgent;
		private bool _active = true;

		public virtual void ItemInitiation()
		{
			_active = true;
		}

		protected override void OnGameReady()
		{
			CheckIfAgentIsHere();
		}
		
		protected new void OnEnable()
		{
			base.OnEnable();
			puzzleManager.itemInitiation += ItemInitiation;
			puzzleManager.preMoveComplete += PreMoveCompleteListener;
			puzzleManager.postMoveComplete += PostMoveCompleteListener;
			
			if (_navigation != null && _node == null)
			{
				SetNode();
			}
		}

		public void ManualInitiate(PuzzleManager _puzzleManager)
		{
			this.puzzleManager = _puzzleManager;
			SetNode();
			CheckIfAgentIsHere();
		}
		
		private void CheckIfAgentIsHere()
		{
			if (_node != null)//Items can be spawned at odd times, so we need to be safe.
			{
				if (_node.AgentBaseHere != null)
				{
					AgentEndedMoveHere(_node.AgentBaseHere);
				}
			}
		}

		protected new void OnDisable()
		{
			base.OnDisable();
			puzzleManager.itemInitiation -= ItemInitiation;
			puzzleManager.preMoveComplete -= PreMoveCompleteListener;
			puzzleManager.postMoveComplete -= PostMoveCompleteListener;
		}
		

		private void PreMoveCompleteListener(Move move)
		{
			if (_active)
			{
				//We use this one for triggering more moves, like conveyer belts pushing players.
				OnPreMoveComplete(move);
			}

			AfterAnyMove(move);
		}
		private void PostMoveCompleteListener(Move move)
		{
			if (_active)
			{
				OnPostMoveComplete(move);

				//This edge case happens for spawnables.
				
				if (_node == null)
				{
					SetNode();//we do setNode instead of ItemInitiation() so it doesnt mess up children.
					Assert.IsTrue(_node != null,"ItemBase created or spawned where there is no node.");
				}
				//
				//And now we check our occupation status. If we trigger a move, that should trigger this, and barring any weird loops, things _should_ work out. :p
				if (_node.AgentBaseHere != occupyingAgent)
				{
					if (occupyingAgent != null)
					{
						//The thing that is here now (null or some agent) isnt the thing that was here. Goodbye, thing that was here.
						AgentLeftHere(occupyingAgent);
					}
					
					
					//CheckIfAgentIsHereNow();
					if (_node.IsAgentHere)
					{
						//There is something here, it wasn't here before. Hello, thing that is now here.
						AgentEndedMoveHere(_node.AgentBaseHere);
					}
				 	
					occupyingAgent = _node.AgentBaseHere;
				}
			}
		}

		public void SetNodeWalkable(bool walkable)
		{
			_node.walkable = walkable;
		}

		protected virtual void AfterAnyMove(Move move)
		{
			
		}

		protected virtual void OnPreMoveComplete(Move move){}

		protected virtual void OnPostMoveComplete(Move move){}

		protected virtual void AgentEndedMoveHere(AgentBase agentBase)
		{
			agentBase.OnOverlapItem(this);
		}
		protected virtual void AgentLeftHere(AgentBase agentBase){}
		
	}
}