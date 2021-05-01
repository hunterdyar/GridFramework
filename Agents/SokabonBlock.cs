using Bloops.GridFramework.Commands;

namespace Bloops.GridFramework.Agents
{
	public class SokabonBlock : AgentBase
	{
		protected override void OnNewSubMove(Move m, SubMove sm)
		{
			if (sm.destinationNode == _node)
			{
				//this is almost identical to the default tileAgent except for this "sm.agent is player" check. Only can be pushed by the playER
				if (CanMoveInDir(sm.dir) && sm.agent is Player)
				{
					m.AddAgentToMove(this,sm.dir,false,sm);
				}
				else
				{
					sm.Invalidate();
				}
			}
		}
	}
}