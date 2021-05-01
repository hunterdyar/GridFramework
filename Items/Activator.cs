using Bloops.GridFramework.Agents;
using Bloops.StateMachine;
using UnityEngine;

namespace Bloops.GridFramework.Items
{
	public class Activator : ItemBase
	{
		[SerializeField] private EventTransition _eventTransition;

		protected override void AgentEndedMoveHere(AgentBase agentBase)
		{
			if (agentBase is Player)
			{
				_eventTransition?.Invoke();
			}
		}
	}
}