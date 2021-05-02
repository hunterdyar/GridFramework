using System;
using Bloops.GridFramework.Agents;
using Bloops.GridFramework.Commands;
using Bloops.GridFramework.Items;
using Bloops.GridFramework.Managers;
using UnityEngine;

namespace Bloops.GridFramework.EulerPainting
{
	public class Painting : ItemBase
	{
		private AgentBase agent;
		[SerializeField] private Color paintColor = Color.cyan;
		private void Awake()
		{
			agent = GetComponent<AgentBase>();
		}

		public override void ItemInitiation()
		{
			base.ItemInitiation();
			//The first one is not a command so we can undo it. This is hacky bugfix.
			puzzleManager.tilemapNavigation.PaintTile(agent.CurrentNode,paintColor);
		}

		protected new void OnEnable()
		{
			base.OnEnable();
			agent.OnAgentFinishedAnimatingMoveEvent += PaintCurrent;
		}

		protected new void OnDisable()
		{
			base.OnDisable();
			agent.OnAgentFinishedAnimatingMoveEvent -= PaintCurrent;
		}
		
		void PaintCurrent()
		{
			Paint p = new Paint(puzzleManager.tilemapNavigation, agent.CurrentNode, paintColor);
			puzzleManager.CommandManager.ExecuteCommand(p);//or exeucute AI command?
		}
		
	}
}