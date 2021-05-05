using System;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using Bloops.GridFramework.DataStructures;
using Bloops.GridFramework.Managers;

namespace Bloops.GridFramework.Commands
{
	public class CommandManager
	{
		public Stueue<ICommand> history = new Stueue<ICommand>();
		public Stueue<ICommand> redos = new Stueue<ICommand>();
		public Action OnUndo;
		public Action OnRedo;
		private int maxUndo = -1;
		private PuzzleManager _puzzleManager;
		public bool CanUndo => history.Count > 0;//todo this might not be true,because of history points, but should be true when not undo-ing....
		public bool CanRedo => redos.Count > 0;
		public CommandManager(int maxUndoAmt = -1)
		{
			Init(null);
		}

		public void Init(PuzzleManager puzzleManager)
		{
			_puzzleManager = puzzleManager;

			history = new Stueue<ICommand>();
			history.SetMaxSize(maxUndo);
			redos = new Stueue<ICommand>();
			redos.SetMaxSize(maxUndo);
		}
		public void Undo()
		{
			if (history.Count > 0)
			{
				ICommand recent = history.Pop();
				recent.Undo();
				redos.Push(recent);

				if (history.PeekTop() is Move topAsMove)
				{
					//After we undo, lets re-collect items or get killed by lasers or whatever, if that was where we were.
					if (topAsMove.executedValidAndComplete)
					{
						_puzzleManager.preMoveComplete?.Invoke(topAsMove);
						_puzzleManager.postMoveComplete?.Invoke(topAsMove);
					}	
				}

				if (!recent.HistoryPoint)
				{
					Undo();//recursive loop!
				}
			}
			_puzzleManager.TurnCounter.RemoveTurn();
			_puzzleManager.TriggerMachine();
			OnUndo?.Invoke();
		}

		public void Redo()
		{
			if (redos.Count > 0)
			{
				ICommand recent = redos.Pop();
				recent.Redo();
				history.Push(recent);
				if (redos.Count > 0 && !redos.PeekTop().HistoryPoint)
				{
					Redo();//recursive loop!
				}
			}
			OnRedo?.Invoke();
		}

		public void ExecuteCommand(ICommand command)
		{
			if (command.IsValid)
			{
				command.Execute();
				//Again? Yeah :(. The move command checks its validity at the time of execution.
				//todo.... dont reaaaaly know how to skirt around that without creating repetative uneeded validity checks on the move itself. 
				if (command.IsValid)
				{
					redos.Clear();
					//
					history.Push(command);
					//
					_puzzleManager.TurnCounter.TakeTurn();
				}
			}
		}
	}
}