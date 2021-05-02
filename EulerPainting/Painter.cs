using System;
using Bloops.GridFramework.Agents;
using Bloops.GridFramework.Managers;
using UnityEngine;

namespace Bloops.GridFramework.EulerPainting
{
	public class Painter : MonoBehaviour
	{
		private AgentBase _agentBase;
		public PuzzleManager puzzleManager;
		[SerializeField] private Color _color = Color.cyan;
		
		//This may still be buggy if on a non-player agent. We may want change its listeners, or have it listen directly to an AgentBase. Which, uh, todo. that.
		void Awake()
		{
			_agentBase = GetComponent<AgentBase>();
		}
		private void OnEnable()
		{
			// _agentBase.OnMoveTo += OnMoveTo;
			puzzleManager.onGameReady += Paint;
			puzzleManager.afterPlayerMoveComplete += AfterPlayerMoveComplete;
		}

		private void OnDisable()
		{
			// _agentBase.OnMoveTo -= OnMoveTo;
			puzzleManager.afterPlayerMoveComplete -= AfterPlayerMoveComplete;
			puzzleManager.onGameReady -= Paint;

		}
		
		void Paint()
		{
			_agentBase.CurrentNode.walkable = false;
			puzzleManager.tilemapNavigation.SetColor(_agentBase.CurrentNode, _color);
		}

		void AfterPlayerMoveComplete(int i)
		{
			Paint();
		}
	}
}