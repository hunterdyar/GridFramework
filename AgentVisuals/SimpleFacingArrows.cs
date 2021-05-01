using System;
using Bloops.GridFramework.Agents;
using UnityEngine;

namespace Bloops.GridFramework.AgentVisuals
{
	public class SimpleFacingArrows : MonoBehaviour
	{
		private FacingAgent _agent;
		[SerializeField] private Transform arrowSprite;

		private void Awake()
		{
			_agent = GetComponent<FacingAgent>();
		}

		private void OnEnable()
		{
			_agent.OnFacingDirChanged += OnFacingDirChanged;
		}

		private void OnDisable()
		{
			_agent.OnFacingDirChanged -= OnFacingDirChanged;
		}

		void OnFacingDirChanged(Vector2Int newFacingDir)
		{
			if (newFacingDir == Vector2Int.up)
			{
				arrowSprite.transform.localRotation = Quaternion.Euler(0,0,0);
			}else if (newFacingDir == Vector2Int.right)
			{
				arrowSprite.transform.localRotation = Quaternion.Euler(0,0,-90);
			}else if (newFacingDir == Vector2Int.down)
			{
				arrowSprite.transform.localRotation = Quaternion.Euler(0,0,180);
			}else if (newFacingDir == Vector2Int.left)
			{
				arrowSprite.transform.localRotation = Quaternion.Euler(0,0,90);
			}
		}
	}
}