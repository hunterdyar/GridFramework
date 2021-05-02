using System;
using Bloops.GridFramework.Agents;
using Bloops.GridFramework.Utility;
using UnityEngine;

namespace Bloops.GridFramework.Navigation
{
	public class NavNode : IComparable
	{
		//injected from initiation, 8Set from nav tile
		public Vector3Int cellPos { get; private set; }
		public NavDirections connections  { get; private set; }
		
		
		//I didn't extend NavNode to add painting because I didn't want to. To change it back, we can just get delete painted and 
		//change Walkable to public bool Walkable => walkable;
		public bool walkable = true;
		//Painted things
		public bool painted = false;

		//injected from initiation
		public bool IsAgentHere { get; private set; }
		public AgentBase AgentBaseHere { get; private set; }//this should be set through the SetAgentHere function.
		private TilemapNavigation _tilemapNavigation;
		public int pathCost => IsAgentHere ? 1:2;//cost is 1 for a free tile and 2 for an occupied tile.

		public Vector3 worldPos => _tilemapNavigation.CellToWorld(cellPos);
		public NavNode(Vector3Int cellPos, TilemapNavigation tilemapNavigation, NavDirections connections, bool walkable)
		{
			this.cellPos = cellPos;
			this._tilemapNavigation = tilemapNavigation;
			this.connections = connections;
			this.walkable = walkable;
		}

		public void ClearAgentHere(AgentBase agentBase)
		{
			//todo Assert(tileAgent == agentHere);
			//bothering with these 'interface' functions specifically for possible future handling of multiple occupied tileAgents
			if (AgentBaseHere == agentBase)
			{
				AgentBaseHere = null;
				IsAgentHere = false;
			}
			else
			{
				Debug.LogWarning("Sanity Check. Tried to clear an agent from where it wasn't.");
			}
		}

		public void SetAgentHere(AgentBase agentBase)
		{
			if (IsAgentHere) 
			{
				ClearAgentHere(AgentBaseHere);
			}
			AgentBaseHere = agentBase;
			IsAgentHere = true;
		}

		public int CompareTo(object n)
		{
			//if we are being compared to a navNode, then compare their pathCosts for weighted heuristics.
			//otherwise... UHHHH.... zero? Probably should check cellPos and compare them...
			return n is NavNode node ? pathCost.CompareTo(node.pathCost) : 0;
		}
	}
}