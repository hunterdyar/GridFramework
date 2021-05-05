using Bloops.GridFramework.Agents;
using Bloops.GridFramework.Navigation;

namespace Bloops.GridFramework.EulerPainting
{
	public class PainterlyAgent : StickyAgent
	{
		public override bool CanWalkOnNode(NavNode node)
		{
			return node.walkable && !node.painted;
		}
	}
}