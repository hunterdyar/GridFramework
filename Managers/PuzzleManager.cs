using System;
using System.Collections.Generic;
using Bloops.GridFramework.AI;
using Bloops.GridFramework.Commands;
using Bloops.GridFramework.Navigation;
using Bloops.StateMachine;
using UnityEngine;

namespace Bloops.GridFramework.Managers
{
	[CreateAssetMenu(fileName = "PuzzleManager", menuName = "Bloops/GridFramework/Puzzle Manager", order = 0)]
	public class PuzzleManager : ScriptableObject
	{
		[Header("Config")]
		[SerializeField] private int numberAfterPlayerMoveEvents = 1;

		[Header("State Machine")] 
		[SerializeField] private Machine gameplayStateMachine;

		[Header("Turn Counter")]
		[SerializeField] private bool puzzleHasMaxTurns = false;
		[SerializeField] private int maxTurnsToWinPuzzle = -1;
		
		//Systems
		public readonly CommandManager CommandManager = new CommandManager();//avoids race conditions with onEnable vs. our initiate events.
		public readonly TurnCounter TurnCounter = new TurnCounter();
		public TilemapNavigation tilemapNavigation;
		public bool currentWinCondition;
		
		//Passing along by Move Events.
		public Action<Move,SubMove> newSubMove;
		public Action<Move> cancelMove;//cancel animation.
		public Action<Move> preMoveComplete;
		public Action<Move> postMoveComplete;

		//Core Game Loop - Setup Phase
		public Action navObjectInitiation;
		public Action agentInitiation;
		public Action itemInitiation;
		public Action onGameReady;

		//Core Game Loop - Playing Phase
		public Action<int> afterPlayerMoveComplete;
		public bool PuzzleHasBegun => _puzzleBegun;
		private bool _puzzleBegun;
		public bool _playerCanMove;
		private List<AIBase> holdingForAI;
		
		//Called by the puzzle manager initiator (in start), which gets the ball rolling.
		//we can use our onEnable calls (before start, after awake) to subscribe to events as needed, and we can be confident that the tilemap has already been inititated (that happens in awake).
		public void Initiate()
		{
			//default variables.
			_puzzleBegun = false;
			currentWinCondition = false;
			holdingForAI = new List<AIBase>();
			//First, initiate our systems.
			CommandManager.Init(this);
			TurnCounter.Reset();
			if (puzzleHasMaxTurns)
			{
				TurnCounter.SetMaxTurns(maxTurnsToWinPuzzle);
			}
			
			//This resets any leftover conditions from level loads, previous runs (these are SO's) and so on.  Has to happen before agent/item init.
			gameplayStateMachine.Init();
			
			//since we know that the tilemap init is complete, we can initiate our objects.
			navObjectInitiation?.Invoke();//First all objects initiate, which sets their current node.
			agentInitiation?.Invoke();//Then agents become solid and navNodes get their activeAgents, and other stuff.
			itemInitiation?.Invoke();//finally items (and conditional checks that arent items), doing whatever they might need to do.
			//Let things take their first turn.
		}
		
		

		//Called by the puzzle manager initiator right after AgentInitiation.
		public void Start()
		{
			//Setup and leave the entry state.
			gameplayStateMachine.StartMachine(false);
			//Useful to have, but in addition: Items will take their first "turn", checking if they are overlapping anything at the time of loading.
			onGameReady?.Invoke();
			_puzzleBegun = true;
			_playerCanMove = true;
			
			// gameplayStateMachine.Trigger();
		}

		/// <summary>
		/// Called by the player after a move, and all of its AfterMoves, is done animating.
		/// </summary>
		public void PlayerFinishedMove()
		{
			//Lasers n win/lose stuff before the AI move.
			TriggerMachine();
			//
			//We cannot move until any AI are done moving.
			_playerCanMove = false;
			
			for (int i = 0; i < numberAfterPlayerMoveEvents; i++)
			{
				//todo: check we are still in the gameplay state? 
				afterPlayerMoveComplete?.Invoke(i);
				TriggerMachine();
			}
			
			CheckIfPlayerCanMove();
		}

		/// <summary>
		/// Updates the internal _playerCanMove variable. Call at appropriate time.
		/// </summary>
		private void CheckIfPlayerCanMove()
		{
			if (holdingForAI.Count == 0)
			{
				_playerCanMove = true;
			}
			else
			{
				_playerCanMove = false;
			}
		}
		
		public void ExecutePlayerCommand(ICommand command)
		{
			//check if such an action is valid for the player right now. 
			CommandManager.ExecuteCommand(command);
		}

		public bool PlayerCanMove()
		{
			return holdingForAI.Count == 0 && _puzzleBegun;
		}

		public void ExecuteAICommand(ICommand command)
		{
			//check if such an action is valid for the AI right now.
			//Allow us to do a delay and check for the trigger machine.
			CommandManager.ExecuteCommand(command);
		}
		
		//
		public void HoldPlayerForAI(AIBase ai)
		{
			//We could make an AI State and switch into and out of it. And that would be good! But that could stop input (doesn't have to).
			//We are mostly using states for UI and Input, and player/AI taking turns doesnt really stop that, its just a matter of holding and making sure they execute in a certain order.
			//So we are using this far simpler "hold/release" ai system.
			
			//we also could have hooked this up to moves or commands, but I wanted to be able to wait for arbitrary things like particle effects or complicated attack animations, so just let the AI sort their own events out.
			
			if (!holdingForAI.Contains(ai))
			{
				holdingForAI.Add(ai);
			}
		}

		public void StopHoldingForAI(AIBase ai)
		{
			if (holdingForAI.Contains(ai))
			{
				holdingForAI.Remove(ai);
				if (holdingForAI.Count == 0)
				{
					//from 1 to 0. last AI is done.
					TriggerMachine();
				}
			}
			
		}
		
		public void TriggerMachine()
		{
			gameplayStateMachine.Trigger();
		}
		
		///Movement calling these Actions
		//Passing along by Move Events.
		public void CallNewSubMove(Move move, SubMove subMove)
		{
			newSubMove?.Invoke(move,subMove);
		}

		public void CallCancelMove(Move move)
		{
			cancelMove?.Invoke(move);
		}

		public void CallPreMoveComplete(Move move)
		{
			preMoveComplete?.Invoke(move);
		}
		public void CallPostMoveComplete(Move move)
		{
			postMoveComplete?.Invoke(move);
		}

		//undo/redo wrapper so we can call these functions from like UnityEvents, ie dropping puzzleManager into an onCLickevent for a button
		public void Undo()
		{
			CommandManager.Undo();
		}

		public void Redo()
		{
			CommandManager.Redo();
		}
	}
}