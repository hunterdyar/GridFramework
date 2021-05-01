using Bloops.GridFramework.Commands;
using UnityEngine;

namespace Bloops.GridFramework.AI
{
	public class MoveInFacingDirAI : AIBase
	{
		protected override void TakeTurn()
		{
			Move m = new Move(puzzleManager,false);
			m.AddAgentToMove(this, (Vector3Int)FacingDir, true, null, null, 50);
			puzzleManager.ExecuteAICommand(m);
			if (m.IsValid)
			{
				puzzleManager.HoldPlayerForAI(this);
			}
		}
	}
}