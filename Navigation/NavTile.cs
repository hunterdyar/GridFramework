using System;
using Bloops.GridFramework.Utility;
using UnityEngine;
using UnityEngine.Tilemaps;

namespace Bloops.GridFramework.Navigation
{
	[CreateAssetMenu(fileName = "Nav Tile", menuName = "Bloops/GridFramework/Nav Tile", order = 1)]

	public class NavTile : Tile
	{
		public TilemapNavigation nav { get; private set; } //injected navigator.
		public bool walkable = true; //change at runtime for doors, and so on.
		public NavDirections walkableDirections;
	}
}