using System;
using Bloops.GridFramework.Agents;
using UnityEngine.Events;

namespace Bloops.GridFramework.Items
{
	public class Teleportor : ItemBase
	{
		public Teleportor otherTeleporter;
		//public UnityEvent OnTeleportFeedback;

		protected override void AgentEndedMoveHere(AgentBase agentBase)
		{
			if (CanTeleport())
			{
				otherTeleporter.Prime(agentBase);
				agentBase.SnapTo(otherTeleporter._node);
			}
		}

		/// <summary>
		/// Called right before sending an agent to this space. This is in order to prevent it from instantly being teleported right back to where it came from in the same frame.
		/// </summary>
		private void Prime(AgentBase agent)
		{
			//This code is kind of a hack to prevent the 'agentEndedMoveHere' from happening twice.
			//could just a bool, but then we would need to reset it at some point, which is another listener. 
			_node.SetAgentHere(agent);
			occupyingAgent = agent;
		}

		public bool AgentCanLandHere()
		{
			return !_node.IsAgentHere;
		}

		private bool CanTeleport()
		{
			if (otherTeleporter != null)
			{
				if (otherTeleporter.AgentCanLandHere())
				{
					return true;
				}
			}
			return false;
		}
		
	}
}