using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Bloops.GridFramework.DataStructures;
using Bloops.GridFramework.Managers;
using Bloops.Utilities;
using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.Serialization;
using UnityEngine.Tilemaps;

namespace Bloops.GridFramework.Navigation
{
	public class TilemapNavigation : MonoBehaviour
	{
		public PuzzleManager puzzleManager;
		public static readonly Vector3Int[] directions = new Vector3Int[]{new Vector3Int(0,1,0),new Vector3Int(1,0,0),new Vector3Int(0,-1,0),new Vector3Int(-1,0,0)};
		public Tilemap tilemap { get; private set; }
		[Tooltip("The grid component handles the grid, which uses a corner as worldPos/anchor. If your agents are anchored in their center, set this to half your grid size for snapping to work.")]
		public Vector3 worldPosAnchorOffset;
		BiDictionary<Vector3Int, NavNode> map = new BiDictionary<Vector3Int, NavNode>();
		private bool initiated = false;
		public ColorReference defaultTileColor;
		private Vector3Int _min;
		private Vector3Int _max;
		public Vector3Int Min => _min;
		public Vector3Int Max => _max;
		
		private void Awake()
		{
			initiated = false;
			puzzleManager.tilemapNavigation = this;
			tilemap = GetComponent<Tilemap>();
			InitiateTiles();
		}
		//INitiate tiles could be called bu the puzzle manager.
		private void InitiateTiles()
		{
			initiated = true;
			//if we do this at runtime, not for initialization, theres probably gonna need to be event unregistering and memory management issues. I dont have events yet soooo
			map.Clear();
			//reset min and max to improbable states.
			_max = new Vector3Int(Int32.MinValue, Int32.MinValue, 0);
			_min = new Vector3Int(Int32.MaxValue, Int32.MaxValue, 0);

			BoundsInt bounds = tilemap.cellBounds;
			for (int x = bounds.xMin; x <= bounds.xMax; x++)
			{
				for (int y = bounds.yMin; y <= bounds.yMax; y++)
				{
					for (int z = bounds.zMin; z <= bounds.zMax; z++)//not officially supporting layered grids but might as well try. //Probably better to use multiple tilemaps.
					{
						Vector3Int position = new Vector3Int(x,y,z);
						NavTile nt = tilemap.GetTile(position) as NavTile; //todo z values?
						if (nt != null)
						{
							//Create NavNode
							NavNode node = new NavNode(position, this, nt.walkableDirections, nt.walkable);
							RegisterTile(position, node);
							if (defaultTileColor != null){
								tilemap.SetColor(position, defaultTileColor.Value); 
							}
							//Adjust Min and Max
							_min = new Vector3Int(Mathf.Min(_min.x, position.x), Mathf.Min(_min.y, position.y), 0);
							_max = new Vector3Int(Mathf.Max(_max.x, position.x), Mathf.Max(_max.y, position.y), 0);
						}
					}
				}
			}
		}

		public void RegisterTile(Vector3Int position,NavNode navNode)
		{
			if (!map.ContainsKey(position))
			{
				map.Add(position,navNode);
			}
		}
		public bool ClearTile(NavNode node)
		{
			return map.RemoveValue(node);
		}
		public bool ClearTileAtCellPos(Vector3Int pos)
		{
			return map.RemoveKey(pos);
		}
		/// <summary>
		/// Get a navNode from the map.
		/// </summary>
		/// <param name="worldPos">input in world space.</param>
		/// <param name="node">The node at the asked for position</param>
		/// <returns>True if node was found.</returns>
		public bool GetNode(Vector3 worldPos, out NavNode node)
		{
			Assert.IsTrue(initiated);
			Vector3Int cell = WorldToCell(worldPos);
			if (map.ContainsKey(cell))
			{
				node = map.KeyMap[cell];
				return true;
			}

			node = null;
			return false;
		}
		public bool GetNode(Vector3Int cellPos, out NavNode node)
		{
			Assert.IsTrue(initiated);

			if (map.ContainsKey(cellPos))
			{
				node = map.KeyMap[cellPos];
				return true;
			}
			node = null;
			return false;
		}

		public bool GetCellPos(NavNode node, out Vector3Int cellPos)
		{
			Assert.IsTrue(initiated);

			if (map.ContainsValue(node))
			{
				cellPos = map.ValueMap[node];
				return true;
			}

			cellPos = -Vector3Int.one;
			return false;
		}
		
		public Vector3 CellToWorld(Vector3Int cellPos)
		{
			return tilemap.layoutGrid.CellToWorld(cellPos)+worldPosAnchorOffset;
		}
		public Vector3Int WorldToCell(Vector3 worldPos)
		{
			return tilemap.layoutGrid.WorldToCell(worldPos);
		}

		public Vector3 GetWorldPos(NavNode node)
		{
			if (GetCellPos(node, out Vector3Int pos))
			{
				return CellToWorld(pos);
			}
			else
			{
				Debug.LogError("Cant get world pos, bad tile.");
				return -Vector3.one;
			}
		}


		public List<NavNode> GetConnectionsTo(NavNode node)
		{
			Assert.IsTrue(initiated);

			List<NavNode> connections = new List<NavNode>();
			
			foreach (var dir in node.connections.AsV3IArray())
			{
				if (map.ContainsKey((node.cellPos + dir)))
				{
					connections.Add(map.KeyMap[(node.cellPos + dir)]);
				}
			}
			return connections;
		}
		public int Cost(NavNode current, NavNode next)
		{
			return current.pathCost + next.pathCost;
		}

		public void PaintTile(NavNode node, Color col)
		{
			node.painted = true;
			tilemap.SetColor(node.cellPos,col);
		}

		public void UnpaintTile(NavNode node)
		{
			node.painted = false;
			tilemap.SetColor(node.cellPos,defaultTileColor.Value);
		}

		public bool AnyUnpaintedTiles()
		{
			foreach (var t in map)
			{
				if (!t.Value.painted)
				{
					return true;
				}
			}

			return false;
		}

		public Vector3Int[] GetPositions()
		{
			return map.KeyMap.Keys.ToArray();
		}
	}
}
