using System;
using System.Collections;
using System.Collections.Generic;
using Bloops.GridFramework.Agents;
using Bloops.GridFramework.Navigation;
using Bloops.GridFramework.Pathfinding;
using Bloops.GridFramework.Utility;
using Bloops.StateMachine;
using UnityEngine;

namespace Bloops.GridFramework.Input
{
	public class ClickToMove : MonoBehaviour
	{
		private PlayerPathfinding _playerPathfinding;
		public State dependantOnState;
		private void Awake()
		{
			_playerPathfinding = GetComponent<PlayerPathfinding>();
		}

		void Update()
		{
			if (dependantOnState != null)
			{
				if (!dependantOnState.IsActive)
				{
					return;
				}
			}
			
			
			if (UnityEngine.Input.GetMouseButtonDown(0))
			{
				var world = Camera.main.ScreenToWorldPoint(UnityEngine.Input.mousePosition);
				_playerPathfinding.TryGoToDestination(world);
			}
			
		}
	}
}
