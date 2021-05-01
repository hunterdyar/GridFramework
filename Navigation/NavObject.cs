using Bloops.GridFramework.Managers;
using UnityEngine;

namespace Bloops.GridFramework.Navigation
{
	public class NavObject : MonoBehaviour
	{
		public PuzzleManager puzzleManager;
		[SerializeField] private bool _snapOnStart = true;
		protected TilemapNavigation _navigation => puzzleManager.tilemapNavigation;
		protected NavNode _node;
		public NavNode CurrentNode => _node;
		protected void SetNode()
		{
			if(_navigation.GetNode(transform.position,out var n))
			{
				_node = n;
				//todo an even more generic object for TileBase and ItemBase... a BaseBase. (NavEntityBase)
				if (_snapOnStart)
				{
					transform.position = _node.worldPos;
				}
			}
		}

		protected void Initiate()
		{
			SetNode();
			if (_snapOnStart && _node != null)
			{
				SnapTo(_node);
			}
		}
		
		public virtual void SnapTo(NavNode newNode)
		{
			transform.position = newNode.worldPos;
		}

		protected void OnEnable()
		{
			puzzleManager.navObjectInitiation += Initiate;
			puzzleManager.onGameReady += OnGameReady;

		}

		protected void OnDisable()
		{
			puzzleManager.navObjectInitiation -= Initiate;
			puzzleManager.onGameReady -= OnGameReady;
		}

		protected virtual void OnGameReady()
		{
			
		}
	}
}