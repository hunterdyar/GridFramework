using Bloops.GridFramework.Agents;

namespace Bloops.GridFramework.Commands
{
	public class RemoveAgent : ICommand
	{
		public bool executed;
		public bool HistoryPoint { get; }
		public bool IsValid { get; }
		private AgentBase _agent;
		private IRemoveWithAnimation _agentAnim;
		private bool removeWithAnimation;
		private string _anim;
		public RemoveAgent(AgentBase agentBase,string animation = "")
		{
			IsValid = true;
			executed = false;
			HistoryPoint = false;
			
			if (agentBase.isActiveAndEnabled == false)
			{
				IsValid = false;
			}
			_agent = agentBase;
			_agentAnim = agentBase as IRemoveWithAnimation;
			removeWithAnimation = _agentAnim != null;//This boolean only exists so we only do this null check once. It's a minor optimization that I am writing early on in my code, just to make myself feel better.
			_anim = animation;
		}

		public void Execute()
		{
			if (removeWithAnimation)
			{
				_agentAnim.Remove(_anim);
			}
			else
			{
				_agent.gameObject.SetActive(false);
			}
			executed = true;
		}

		public void Undo()
		{
			if (removeWithAnimation)
			{
				_agentAnim.EnableInstant();
			}
			else
			{
				_agent.gameObject.SetActive(true);
			}
			executed = false;
		}

		public void Redo()
		{
			if (removeWithAnimation)
			{
				_agentAnim.RemoveInstant();
			}
			else
			{
				_agent.gameObject.SetActive(false);
			}
			executed = true;
		}
	}
}