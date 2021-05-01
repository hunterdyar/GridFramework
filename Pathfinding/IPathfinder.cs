using System;
using System.Collections.Generic;
using Bloops.GridFramework.Navigation;
using Bloops.GridFramework.Utility;

namespace Bloops.GridFramework.Pathfinding
	{
		public interface IPathfinder
		{
			PathStatus PathStatus { get; }
			bool Running { get; }
			List<NavNode> GetPath();
			void Search(NavNode start, NavNode end);
			
			/// <summary>
			/// This action is called when the pathfinding coroutine is finished. Be sure to check PathStatus if a path was succesfully found or not!
			/// </summary>
			Action<IPathfinder> OnPathfindingComplete { get; set; }

		}
	}
