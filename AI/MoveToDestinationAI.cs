using Bloops.GridFramework.Navigation;
namespace Bloops.GridFramework.AI
{
	public class MoveToDestinationAI : AIBase
	{
		//This agent will use pathfinding to travel through a sequence of points.

		public NavObject currentDestination;
		
		protected override void TakeTurn()
		{
			if (_node != currentDestination.CurrentNode)
			{
				//todo: Report AI taking turn to puzzle manager
				StartCoroutine(PathfindThenTakeTurn(currentDestination.CurrentNode));
			}
		}
		
	}
}