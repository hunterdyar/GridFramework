using System;
using System.Collections;
using System.Collections.Generic;
using Bloops.GridFramework.Commands;
using Bloops.GridFramework.Items;
using Bloops.GridFramework.Items.Laser;
using Bloops.GridFramework.Navigation;
using Bloops.StateMachine;
using UnityEngine;

namespace Bloops.GridFramework.Agents
{
	public class Player : AgentBase, IRemoveWithAnimation
	{
		 protected Queue<(Vector2Int,bool)> _inputQueue;
		 protected Move _currentMove;
		 private Coroutine fadeCoroutine;
		 [Tooltip("Set this to null to test while invincible.")]
		 [SerializeField] private Transition onDeathTransition;
		 private BasicCondition deathCondition;
		 public Action OnPlayerInitiated;
		 /// <summary>
		 /// Fires off when the player is removed. Remember to Disable them after the desired animation is done playing. An empty string should be a default animation (ex: instant).
		 /// </summary>
		 public Action<string> OnRemove;

		 private void Awake()
		 {
			 //to support future singleton or data patterns, we listen to the command manager callbacks to do clear moves, instead of doing it when we fire off undo.
			 //that way the command manager can be completely separated from the player character. Which is good for puzzle games. -_- trust me.
			 _inputQueue = new Queue<(Vector2Int, bool)>(); //this tuple is the direction and history state.
		 }

		 protected override void AgentInitiation()
		 {
			 base.AgentInitiation();
			 deathCondition = new BasicCondition(false);
			 onDeathTransition?.AddCondition(deathCondition);
			 //various init things
			 OnPlayerInitiated?.Invoke();
		 }

		 protected new void OnEnable()
		 {
			 base.OnEnable();
			 puzzleManager.CommandManager.OnUndo += ClearMoves;
			 puzzleManager.CommandManager.OnRedo += ClearMoves;
		 }

		 protected new void OnDisable()
		 {
			 base.OnDisable();
			 puzzleManager.CommandManager.OnUndo -= ClearMoves;
			 puzzleManager.CommandManager.OnRedo -= ClearMoves;
		 }

		 void ClearMoves()
		 {
			 _inputQueue.Clear();
			 // _currentMove = null;
		 }
		 public void Move(Vector2Int dir, bool historyState = true)
		 {
			 _inputQueue.Enqueue((dir,historyState));
		 }

		 protected virtual void Update()
		 {
			 if (moving && _currentMove == null)
             {
                 return;//this is an edge case for zombies and multi move.
             }
			 if (!moving && _currentMove == null || _currentMove.readyToMoveOn)
			 {
				 if (_inputQueue.Count > 0 && puzzleManager.PlayerCanMove())
				 {
					 TryNewMove(_inputQueue.Dequeue());
				 }
			 }
			 
			 if (_currentMove != null){
				 //clear our inputs if a move didn't work
				 if (!_currentMove.IsValid)
				 {
					 _currentMove = null;
					 _inputQueue.Clear();
				 }
				 //
			 }
		 }
		 
		// a player is a player because they can respond to direction input.
		protected virtual void TryNewMove((Vector2Int, bool) input)
		{
			_currentMove = new Move(puzzleManager, input.Item2); //we will only store the first move as a history point.
		
			_currentMove.AddAgentToMove(this, new Vector3Int(input.Item1.x, input.Item1.y, 0), true, null, null, 100);

			puzzleManager.ExecutePlayerCommand(_currentMove);
			
			//todo I think we can override the OnMoveAnimationFinished but idk
			StartCoroutine(WaitForMoveToFinishThenUpdateGameLoop(_currentMove));
		}

		protected IEnumerator WaitForMoveToFinishThenUpdateGameLoop(Move move)
		{
			//todo multiple players would want to check against a (static?) list of moves....
			while (!move.readyToMoveOn)
			{
				yield return null;
			}

			if (move.executedValidAndComplete)
			{
				puzzleManager.PlayerFinishedMove();//this gets called after the post moves.
			}
		}
		//todo: Rename this. Come up with common terminology for things that are solid (agents) and things that are not (items). 'items' isn't great, but neither is 'Floor'
		public override void OnOverlapItem(ItemBase item)
		{
			base.OnOverlapItem(item);
			if (item is LaserBeam)
			{
				//when we transition states, we lose our ability ot undo/redo.
				deathCondition.Set(true);
			}else if (item is Collectible)
			{
				(item as Collectible).Collect();
			}else if (item is Exit)
			{
				deathCondition?.Set(true);//We should die during testing, but that's likely to change in the future.
			}
		}

		//implementing animations.
		//These can easily fire events that components listen to.
		public void Remove(string animationType)
		{
			if (OnRemove != null)
			{
				//we can write a component elsewhere to handle animations.
				//coroutine or DoTweenFade, oncomplete ->removeInstant.
				OnRemove.Invoke(animationType);
			}
			else
			{
				gameObject.SetActive(false);
			}
		}
		
		public void RemoveInstant()
		{
			gameObject.SetActive(false);
		}

		public void EnableInstant()
		{
			gameObject.SetActive(true);
		}
		
		public override bool CanWalkOnNode(NavNode destinationNode)
		{
			return !destinationNode.painted;
		}

		public void ForceOverrideCurrentMove(Move newMove)
		{
			_currentMove = newMove;
		}
	}
}