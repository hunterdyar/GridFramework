using System;
using System.Collections;
using System.Collections.Generic;
using Bloops.GridFramework.Agents;
using Bloops.GridFramework.Commands;
using Bloops.GridFramework.Navigation;
using Bloops.GridFramework.Pathfinding;
using Bloops.GridFramework.Utility;
using UnityEngine;

namespace Bloops.GridFramework.AI
{
	public class AIBase : FacingAgent
	{
		[SerializeField] private int TurnNumber = 0;
		private IPathfinder _pathfinder;

		protected override void AgentInitiation()
		{
			base.AgentInitiation();
			_pathfinder = new AStarPathfinder(puzzleManager.tilemapNavigation);
			puzzleManager.afterPlayerMoveComplete += AfterPlayerMoveCompletedListener;
		}

		protected void OnDestroy()
		{
			puzzleManager.afterPlayerMoveComplete -= AfterPlayerMoveCompletedListener;
			puzzleManager.StopHoldingForAI(this);//just in case.
		}

		private new void OnDisable()
		{
			base.OnDisable();
			puzzleManager.StopHoldingForAI(this);//juuuust in case.
		}

		protected void AfterPlayerMoveCompletedListener(int AITurnNumber)
		{
			//c# listeners happen?
			if (!gameObject.activeInHierarchy)
			{
				//AI was disabled at some point, but the OnDisable hasn't happened yet. This happens... but idk.
				Debug.LogWarning("TODO We still have to figure out AI Disabling.");
				return;
			}
			
			if (AITurnNumber == TurnNumber)
			{
				TakeTurn();
			}
		}
		protected virtual void TakeTurn()
		{
		}
		protected IEnumerator PathfindThenTakeTurn(NavNode destination)
		{
			//Wait for pathfinder to finish its search.
			_pathfinder.Search(_node,destination);
			while (_pathfinder.Running)
			{
				yield return null;
			}

			if (_pathfinder.PathStatus == PathStatus.success)
			{
				List<NavNode> path = _pathfinder.GetPath();//0 is our current position, so position 1 is the next.

				if (path.Count > 1)
				{
					NavNode next = path[1];
					Vector3Int dir = next.cellPos-_node.cellPos;
					Move m = new Move(puzzleManager,false);
					m.AddAgentToMove(this, dir, true, null, null, 50);
					puzzleManager.ExecuteAICommand(m);
					if (m.IsValid)
					{
						puzzleManager.HoldPlayerForAI(this);
					}
				}
			}
		}

		protected override void OnAgentFinishedAnimatingMove()
		{
			puzzleManager.StopHoldingForAI(this);
		}
	}
}