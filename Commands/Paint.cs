using Bloops.GridFramework.Navigation;
using UnityEngine;

namespace Bloops.GridFramework.Commands
{
	public class Paint : ICommand
	{
		private TilemapNavigation _tilemapNavigation;
		private NavNode node;
		private Color _color;
		public bool HistoryPoint { get; }
		public bool IsValid { get; }

		public Paint(TilemapNavigation tilemapNavigation, NavNode node, Color color,bool historyPoint = false)
		{
			IsValid = true;
			_tilemapNavigation = tilemapNavigation;
			this.node = node;
			_color = color;
			HistoryPoint = historyPoint;
		}

		public void Execute()
		{
			_tilemapNavigation.PaintTile(node,_color);
		}

		public void Undo()
		{
			_tilemapNavigation.UnpaintTile(node);
		}

		public void Redo()
		{
			Execute();
		}
	}
}