using Bloops.GridFramework.Agents;
using Bloops.GridFramework.Managers;
using Bloops.StateMachine;

namespace Bloops.GridFramework.Items
{
	public class SokabonBlockLocation : ItemBase, ICondition
	{
		
		public Transition transition;
		public override void ItemInitiation()
		{
			base.ItemInitiation();
			transition.AddCondition(this);
		}

		private void OnDestroy()
		{
			transition.RemoveCondition(this);
		}

		public bool GetCondition()
		{
			return _node.IsAgentHere && (_node.AgentBaseHere is SokabonBlock);
		}
	}
}