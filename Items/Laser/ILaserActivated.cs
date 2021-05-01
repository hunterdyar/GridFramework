using Bloops.GridFramework.Agents;

namespace Bloops.GridFramework.Items.Laser
{
	//ILaserActivated<T> where T : AgentBase... do I want to bother with generic interface to restrict type?
	public interface ILaserActivated
	{
		void SetLaserSource(LaserSource source);
		void RemoveLaserSource(LaserSource source);
		bool HasLaserSource(LaserSource source);
	}
}