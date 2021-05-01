using System;
using Bloops.GridFramework.Commands;
using UnityEngine;
using UnityEngine.Events;

namespace Bloops.GridFramework.Agents
{
	public class FacingAgent : AgentBase
	{
		private Vector2Int _facingDir = Vector2Int.zero;//you don't have to be facing a direction to have a facing dir.
		public Vector2Int FacingDir => _facingDir;

		public Vector2Int startingDir;

		public Action<Vector2Int> OnFacingDirChanged;
		//For the sake of example, we will use children gameObjects for direcitons.
		public UnityEvent<Vector2Int> OnFacingDirChangedEvent;
		void Start()
		{
			_facingDir = startingDir;
		}
		protected void SetFacingDir(Vector2Int facingDir)
		{
			if (_facingDir != facingDir)
			{
				_facingDir = facingDir;
				OnFacingDirChanged.Invoke(_facingDir);
				OnFacingDirChangedEvent.Invoke(_facingDir);
			}
		}

		public override object GetLocationData()
		{
			return _facingDir;//we pass the object "vector2", but we could pass a custom struct or a tuple.
		}

		public override void SetFromLocationData(object locationData)
		{
			//We receive the data and sort cast it back to the same type that we give it.
			SetFacingDir((Vector2Int)locationData);
		}

		public override void MoveTo(SubMove subMove, Move contextMove)
		{
			SetFacingDir((Vector2Int)subMove.dir);
			simpleLerpCoroutine = StartCoroutine(SimpleMoveLerp(subMove.destinationNode, 10, contextMove));
		}
		public override void MoveToInstant(SubMove subMove, Move contextMove)
		{
			SetFacingDir((Vector2Int)subMove.dir);
			SnapTo(subMove.destinationNode);
		}
	}
}