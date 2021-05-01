using System.Collections.Generic;
using System.Linq;
using Bloops.GridFramework.Commands;
using UnityEngine;

namespace Bloops.GridFramework.Agents
{
	public class ConnectedBlock : AgentBase
	{
		[SerializeField] private List<ConnectedBlock> siblings;

		void Awake()
		{
			//for lazy setup. Lets us to copy/paste subling properties to all siblings. 
			if (siblings.Contains(this))
			{
				siblings.Remove(this);
			}
		}
		protected override void OnNewSubMove(Move m, SubMove sm)
		{
			if ((sm.agent is ConnectedBlock cBlock))
			{
				if (siblings.Contains(cBlock))//we must move with this, because we are connected.
				{
					if (CanMoveInDir(sm.dir))
					{
						m.AddAgentToMove(this, sm.dir, false, sm, null, 50);//should relies on be a submove?
						return;
					}
					else
					{
						sm.Invalidate();
						return;
					}
				}
			}
			//If this is not a chain setup (which should exit the function before this code happens).

			if (sm.destinationNode == _node)
			{
				if (CanMoveInDir(sm.dir))
				{
					m.AddAgentToMove(this, sm.dir, false, sm, sm, 60);
				}
				else
				{
					sm.Invalidate();
				}
			}
		}
	}
}