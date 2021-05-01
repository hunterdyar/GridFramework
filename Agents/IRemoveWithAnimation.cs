namespace Bloops.GridFramework.Agents
{
	public interface IRemoveWithAnimation
	{
		void Remove(string removalAnimation);
		void RemoveInstant();
		void EnableInstant();
	}
}