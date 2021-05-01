using System;
using System.Collections;
using System.Collections.Generic;
using Bloops.GridFramework.Navigation;
using Bloops.GridFramework.Utility;

namespace Bloops.GridFramework.Pathfinding
{
	//Dijkstra
	public class DJPathfinder : IPathfinder
	{
		//Dijkstra's-ish algorithm basically does a blind flood-fill from the destination until the start tile is reached
		//keeping track of which tile flooded into the 'current' allows us to then reverse those directions and get the shortest path from start to end.

		private TilemapNavigation _tilemapNavigation;
		NavNode _cachedStart;
		public readonly List<NavNode> path = new List<NavNode>();
		public Dictionary<NavNode, int> Distances { get; set; } = new Dictionary<NavNode, int>();
		Dictionary<NavNode, NavNode> _cameFrom = new Dictionary<NavNode, NavNode>();
		private PathStatus _pathStatus = PathStatus.failure;//-1 fail 0 searching 1 success
		public PathStatus PathStatus => _pathStatus;
		public Action<IPathfinder> OnPathfindingComplete { get; set; }
		
		public bool Running { get; private set; }

		public DJPathfinder(TilemapNavigation tilemapNavigation)
		{
			_tilemapNavigation = tilemapNavigation;
		}

		public void Search(NavNode start, NavNode end)
		{
			//Reset our status if need be (ie:this object is cached).
			if (_cachedStart != start)
			{
				_pathStatus = PathStatus.searching;
			}

			if (_pathStatus == PathStatus.searching|| _pathStatus == PathStatus.failure)
			{
				//if path is unfound or failed to find

				_tilemapNavigation.StartCoroutine(FindAllPaths(start,end, 100));
				//high iteration number basically means "we need it now!"
				//sadly it restarts. Could we have it find a currently running coroutine and change the iteration value? 
				//that would be neat
			}
		}

		public IEnumerator FindAllPaths(NavNode start, NavNode end, int iterationsPerFrame)
		{
			Running = true;
			var frontier = new Queue<NavNode>();
			_cachedStart = start;
			frontier.Enqueue(start);
			_cameFrom = new Dictionary<NavNode, NavNode>();
			Distances = new Dictionary<NavNode, int> {[start] = 0};
			var iterations = 0;
			// Debug.Log("pathfinding...");
			while (frontier.Count > 0)
			{
				var current = frontier.Dequeue();
				foreach (var next in _tilemapNavigation.GetConnectionsTo(current))
				{
					if (Distances.ContainsKey(next) || !next.walkable)
					{
						continue;
					}

					frontier.Enqueue(next);
					Distances[next] = Distances[current] + 1;
					_cameFrom[next] = current;
				}

				//performance things
				iterations++;
				// ReSharper disable once InvertIf
				if (iterations >= iterationsPerFrame)
				{
					iterations = 0;
					yield return null;
				}
			}
			
			SetPathList(start,end);
		}

		private void SetPathList(NavNode start, NavNode end)
		{
			Running = false;

			_pathStatus = PathStatus.success;
			var search = end;
			path.Clear();
			while (search != start)
			{
				if (_cameFrom.ContainsKey(search))
				{
					path.Add(search);
					search = _cameFrom[search];
				}
				else
				{
					_pathStatus = PathStatus.failure;
					return;
				}
			}

			path.Add(start);
			path.Reverse();
			if (_pathStatus == PathStatus.success)
			{
				OnPathfindingComplete?.Invoke(this);
			}
		}

		public PathStatus GetPathStatus()
		{
			return _pathStatus;
		}

		public List<NavNode> GetPath()
		{
			return path;
		}
	}
}