using System.Collections;
using System.Collections.Generic;
using Bloops.GridFramework.Agents;
using Bloops.GridFramework.Navigation;
using Bloops.GridFramework.Pathfinding;
using Bloops.GridFramework.Utility;
using UnityEngine;

namespace Bloops.GridFramework.Input
{
	public class PlayerPathfinding : MonoBehaviour
	{
		private IPathfinder _pathfinder;
		private Player _player;
		public float speed = 10;
		private Coroutine _moveChainCoroutine;

		void Awake()
		{
			//
			_player = GetComponent<Player>();
			_player.OnPlayerInitiated += OnPlayerInitiated;
		}

		void OnPlayerInitiated()
		{ 
			_pathfinder = new AStarPathfinder(_player.puzzleManager.tilemapNavigation);
		}
		

		public void TryGoToDestination(Vector3 destination)
		{
			if (_player.moving)
			{
				return;
			}
			if(_player.puzzleManager.tilemapNavigation.GetNode(destination,out var final))
			{
				StartCoroutine(FindPathToDestination(final));
			}
		}
	
	

		/// <summary>
		/// Coroutine to search for the destination, simply waiting until destination is found. 
		/// </summary>

		private IEnumerator FindPathToDestination(NavNode final)
		{
			_pathfinder.Search(_player.CurrentNode,final);
			//Wait for pathfinder to finish its search.
			while (_pathfinder.Running)
			{
				yield return null;
			}

			if (_pathfinder.PathStatus == PathStatus.success)
			{
				_moveChainCoroutine = StartCoroutine(MoveToDestination(_pathfinder.GetPath()));
			}
		}
	
	
		IEnumerator MoveToDestination(List<NavNode> path)
		{
			// path.RemoveAt(0);//0 is our current position. We're here already!
			for (int i = 1; i < path.Count; i++)
			{
				//Pretty neat that you can wait for a different coroutine to finish :p
				// yield return _agent.SimpleMoveLerp(path[i],speed,null);
				Vector2Int dir = (Vector2Int)(path[i].cellPos-path[i-1].cellPos);
				_player.Move(dir, i==1);
			}
			yield return null;//an extra pause
		}
	}
}