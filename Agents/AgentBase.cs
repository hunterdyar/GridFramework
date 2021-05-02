using System;
using System.Collections;
using Bloops.GridFramework.Commands;
using Bloops.GridFramework.Items;
using Bloops.GridFramework.Managers;
using Bloops.GridFramework.Navigation;
using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.Serialization;

namespace Bloops.GridFramework.Agents
{
	public partial class AgentBase : NavObject
	{
		/// <summary>
		/// Pushable.
		/// </summary>
		[SerializeField] protected bool canMove = true;
		protected Coroutine simpleLerpCoroutine;
		public Action OnSnappedToNode;
		public Action OnAgentFinishedAnimatingMoveEvent;
		public Action<SubMove> OnMoveTo;
		public bool moving { get; protected set; }
		protected new void OnEnable()
		{
			base.OnEnable();
			if (puzzleManager == null)
			{
				Debug.LogError("Forgot to set puzzleManager for an agent.",this);
			}
			puzzleManager.newSubMove += OnNewSubMove;
			puzzleManager.cancelMove += CancelMove;
			puzzleManager.agentInitiation += AgentInitiation;
		}

		protected new void OnDisable()
		{
			base.OnDisable();
			puzzleManager.agentInitiation -= AgentInitiation;
			puzzleManager.newSubMove -= OnNewSubMove;
			puzzleManager.cancelMove -= CancelMove;
		}

		public virtual object GetLocationData()
		{
			return null;
		}

		public virtual void SetFromLocationData(object locationData)
		{
			
		}

		protected virtual void AgentInitiation()
		{
			SetCurrentNode(_node);
		}

		public void SetCanMove(bool canMove)
		{
			this.canMove = canMove;
		}
		private void CancelMove(Move obj)
		{
			if (simpleLerpCoroutine != null)
			{
				StopCoroutine(simpleLerpCoroutine);
				moving = false;
			}
			SnapTo(_node);
		}

		
		protected virtual void OnNewSubMove(Move m, SubMove sm)
		{
			//if we are not already involved with the move.
			if (!m.IsInvolved(this))
			{
				if (sm.destinationNode == _node)//if we are being pushed.
				{
					//a tile agent is moving to here, we need to either invalidate the move or get pushed or move out of the way or whatever.
					if (CanMoveInDir(sm.dir) )
					{
						//critical to 
						m.AddAgentToMove(this,sm.dir,false,sm,sm,20);
					}
					else
					{
						//we are being pushed, and we cannot move. So the submove tryu
						sm.Invalidate();
					}
				}
			}
		}

		/// <summary>
		/// Returns true or false if this agent can move in a direction. Ignores other agents that may be pushed!
		/// </summary>
		protected virtual bool CanMoveInDir(Vector3Int dir)
		{
			if (!canMove)
			{
				return false;
			}
			
			if (_navigation.GetNode(_node.cellPos + dir, out var final))
			{
				if (final.walkable)
				{
					return true;
				}
			}

			return false;
		}

		//override this and implement animation logic.

		//I use DOTween, but this starts a simple coroutine.

		public virtual void MoveTo(SubMove subMove, Move contextMove)
		{
			Assert.IsTrue(canMove);
			simpleLerpCoroutine = StartCoroutine(SimpleMoveLerp(subMove.destinationNode, 10, contextMove));
			OnMoveTo?.Invoke(subMove);
		}
		/// <summary>
		/// Used when Redo is triggered. 
		/// </summary>
		public virtual void MoveToInstant(SubMove subMove, Move contextMove)
		{
			SnapTo(subMove.destinationNode);
		}

		//For instantaneous movements, like undos.

		public override void SnapTo(NavNode newNode)
		{
			base.SnapTo(newNode);
			SetCurrentNode(newNode);
			OnSnappedToNode?.Invoke();
		}

		public bool PutSelfOnTile()
		{
			if (_node != null)
			{
				SetCurrentNode(_node);
				return true;
			}
			
			if (_navigation.GetNode(transform.position, out NavNode node))
			{
				SetCurrentNode(node);
				return true;
			}
			return false;
		}

		public void SetCurrentNode(NavNode newNode)
		{
			if (_node != null)
			{
				if (_node.AgentBaseHere == this)
				{
					_node.ClearAgentHere(this);
				}
				//An agent may leave a node that it never occupied. This sometimes happens because of the way moves get chained. 
				//This can happen. It's fine. Items will still catch and release as needed, they listen to those events from Moves, not from the navnodes.
			}

			if (newNode == null)
			{
				Debug.LogError("AgentBase cannot set node here. Are they at an appropriate location?",gameObject);
			}
			else
			{
				_node = newNode;
				newNode.SetAgentHere(this);
			}
		}

		
		protected IEnumerator SimpleMoveLerp(NavNode newNode,float speed, Move contextMove)
		{
			moving = true;
			float t = 0;
			var s = transform.position;
			var e = newNode.worldPos;
			while (t<1)
			{
				transform.position = Vector3.Lerp(s, e, t);
				t = t + Time.deltaTime * speed;
				yield return null;
			}
			//snap to perfect position
			transform.position = newNode.worldPos;
			//Set position at end of move. This is... contentious. One may want to 
			SetCurrentNode(newNode);
			
			contextMove?.AgentFinishedAnimatingMove(this);
			OnAgentFinishedAnimatingMove();
			moving = false;
		}

		protected virtual void OnAgentFinishedAnimatingMove()
		{
			OnAgentFinishedAnimatingMoveEvent?.Invoke();
		}
		public virtual void OnOverlapItem(ItemBase item)
		{
			
		}

		public virtual bool CanMoveToNode(NavNode destinationNode)
		{
			return destinationNode.walkable;
		}
	}
}