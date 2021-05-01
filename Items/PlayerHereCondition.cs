using Bloops.GridFramework.Agents;
using Bloops.StateMachine;
using UnityEngine;

namespace Bloops.GridFramework.Items
{
	public class PlayerHereCondition : ItemBase, ICondition
	{
		[SerializeField] private Transition _transition;

		public override void ItemInitiation()
		{
			base.ItemInitiation();
			_transition.AddCondition(this);
		}
		private void OnDestroy()
		{
			_transition.RemoveCondition(this);
		}

		public bool GetCondition()
		{
			return _node.IsAgentHere && (_node.AgentBaseHere is Player);

		}
	}
}