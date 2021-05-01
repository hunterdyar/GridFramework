using Bloops.GridFramework.Commands;
using UnityEngine;

namespace Bloops.GridFramework.Agents
{
	public class StickyAgent : AgentBase
	{
		[SerializeField] bool forcefulSticky = false;
		[SerializeField] private bool onlyStickToPlayer = false;

		protected override void OnNewSubMove(Move m, SubMove sm)
		{
			//if we are not already involved. There is absolutely a way for this recursive move tree to get bungled up by the first thing saying "yes you can move" and a second one saying "no you cannot".
			// if (!m.IsInvolved(this))
			// {
			//we check pushing because a move should be invalid if we are being pushed.
			//below, the move should only be invalid if we are forcefully sticky. (todo: rename that lol).
			//This could be cleaned up with a check where we set the criticalToMove bool to 'destination == current || forcefullysticky'. But uh, thats conceptually harder, even if this is uglier. Whatever.
			if (sm.destinationNode == _node)
			{
				if ((sm.destinationNode.cellPos - _node.cellPos).magnitude != 1)
				{
					// Debug.Log("edge case found?");
				}

				// bool critical = sm.agent is Player;
				//a tile agent is moving to here, we need to either invalidate the move or get pushed or move out of the way or whatever.
				if (CanMoveInDir(sm.dir))
				{
					//critical to move should be true! but only if the move that was passed to us is critical. If we are being pushed by a stickyblock, then we aren't critical....
					m.AddAgentToMove(this, sm.dir, false, sm, sm, 19);
				}
				else
				{
					//we are being pushed, and we cannot move. so the submove trying to push us cannot move.
					sm.Invalidate();
				}
			}
			else
			{
				if (onlyStickToPlayer)
				{
					if (!(sm.agent is Player))
					{
						return;
					}
				}

				//we arent being pushed. Time to behave like a sticky object, getting scraped off!
				//if we are adjacent to the moving object...
				if ((_node.cellPos - sm.startingNode.cellPos).magnitude == 1)
				{
					if (CanMoveInDir(sm.dir))
					{
						//we follow along. If we don't get to move, thats fine! The thing pushing us isn't a big problem, ya know?
						m.AddAgentToMove(this, sm.dir, false, null, sm, -5);
					}
					else
					{
						//uh oh, youre stuck to us now! sucks lol.
						if (forcefulSticky)
						{
							sm.Invalidate();
						}
					}
				}
			}
		}
	}
}