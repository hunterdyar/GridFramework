using System;
using System.Collections.Generic;
using Bloops.GridFramework.DataStructures;
using Bloops.GridFramework.Navigation;
using Bloops.GridFramework.Utility;

namespace Bloops.GridFramework.Pathfinding
{
	public class AStarPathfinder: IPathfinder
	{
		private TilemapNavigation _tilemapNavigation;

		public PathStatus PathStatus { get; private set; }
		public readonly List<NavNode> path = new List<NavNode>();
		public Action<IPathfinder> OnPathfindingComplete { get; set; }
		public bool Running { get; private set; }
		public List<NavNode> GetPath()
		{
			return path;
		}

	
		public Dictionary<NavNode, NavNode> cameFrom = new Dictionary<NavNode, NavNode>();
		public Dictionary<NavNode, int> costSoFar = new Dictionary<NavNode, int>();

		public AStarPathfinder(TilemapNavigation nav)
		{
			_tilemapNavigation = nav;
		}

		// Note: a generic version of A* would abstract over Location and
		// also Heuristic
		static public int Heuristic(NavNode a, NavNode b)
		{
			return Math.Abs(a.cellPos.x - b.cellPos.x) + Math.Abs(a.cellPos.y - b.cellPos.y);
		}

		public void Search(NavNode start, NavNode end)
		{
			PathStatus = PathStatus.searching;
			//
			path.Clear();
			cameFrom.Clear();
			costSoFar.Clear();
			if (start == end)
			{
				PathStatus = PathStatus.success;
				return;
			}
			//
			var frontier = new PriorityQueue<NavNode>();
			frontier.Enqueue(start, 0);

			cameFrom[start] = start;
			costSoFar[start] = 0;

			while (frontier.Count > 0)
			{
				var current = frontier.Dequeue();
				if (current.Equals(end))
				{
					break;
				}

				foreach (var next in _tilemapNavigation.GetConnectionsTo(current))
				{
					int newCost = costSoFar[current] + _tilemapNavigation.Cost(current, next);
					if (!costSoFar.ContainsKey(next) || newCost < costSoFar[next])
					{
						if (next != current)
						{
							costSoFar[next] = newCost;
							int priority = newCost + Heuristic(next, end);
							frontier.Enqueue(next, priority);
							cameFrom[next] = current;
						}
					}
				}
			}
			SetPathList(start, end);
			
		}
		
		private void SetPathList(NavNode start, NavNode end)
		{
			PathStatus = PathStatus.success;
			var search = end;
			path.Clear();
			path.Add(end);
			int pathLength = 0;
			while (search != start && pathLength<100)
			{
				pathLength++;
				if (cameFrom.ContainsKey(search))
				{
					search = cameFrom[search];
					path.Add(search);
				}
				else
				{
					PathStatus = PathStatus.failure;
					return;
				}
			}

			// path.Add(start);
			path.Reverse();
			if (PathStatus == PathStatus.success)
			{
				OnPathfindingComplete?.Invoke(this);
			}
			Running = false;
		}
	}
}
