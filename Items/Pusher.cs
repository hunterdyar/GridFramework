using Bloops.GridFramework.Commands;
using UnityEngine;

namespace Bloops.GridFramework.Items
{
	public class Pusher : ItemBase
	{
		[SerializeField] private Vector2Int pushDir;
		
		protected override void OnPreMoveComplete(Move m)
		{
			foreach (var sm in m.GetValidSubMoves())
			{
				if (sm.destinationNode == _node)
				{
					Move am = new Move(puzzleManager,false);
					am.AddAgentToMove(sm.agent, (Vector3Int) pushDir, true, null, null,50);
					m.RegisterAfterMove(am);
				}
			}
		}

		protected override void OnGameReady()
		{
			base.OnGameReady();
			if (_node.AgentBaseHere != null)
			{
				Move m = new Move(puzzleManager, false);
				m.AddAgentToMove(_node.AgentBaseHere, (Vector3Int) pushDir, true, null,null,50);
				puzzleManager.CommandManager.ExecuteCommand(m);
			}
		}
	}
}