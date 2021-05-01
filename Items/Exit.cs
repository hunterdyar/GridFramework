using Bloops.GridFramework.Agents;
using Bloops.GridFramework.Commands;

namespace Bloops.GridFramework.Items
{
	public class Exit : ItemBase
	{
		public bool open;
		/// <summary>
		/// Default to null. This string will get passed along, and - depending on how you implement it, likely to an Animator. That's why its a string and not some less gross property to edit, so we can pass anim.SetTrigger(this).
		/// When setting up that animation, you will need to disable the appropriate gameObject after the animation is done playing, and re-set any other properties. Check out the IRemoveWithAnimation interface. 
		/// </summary>
		public string exitAnimation = "";
		protected override void AgentEndedMoveHere(AgentBase agentBase)
		{
			if (open)
			{
				agentBase.OnOverlapItem(this);
				RemoveAgent removeAgent = new RemoveAgent(agentBase,exitAnimation);
				puzzleManager.CommandManager.ExecuteCommand(removeAgent);
				// agent.OnOverlapItem(this);
			}
		}
	}
}