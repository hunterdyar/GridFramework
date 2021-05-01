using System;
using System.Collections.Generic;
using UnityEngine;

namespace Bloops.GridFramework.Utility
{
	[System.Serializable]
	public struct NavDirections
	{
		//Note. Using an int array for internal storage (instead of bool) for future possibilities, like off-tile links.
		private int[] openings => new int[] {OpenUp ? 1 : 0, OpenRight ? 1 : 0, OpenDown ? 1 : 0, OpenLeft ? 1 : 0};
		
		[SerializeField] public bool OpenUp;
		[SerializeField] public bool OpenRight;
		[SerializeField] public bool OpenDown;
		[SerializeField] public bool OpenLeft;
		
		public NavDirections(int[] openings)
		{
			if (openings.Length != 4)
			{
				throw new ArgumentOutOfRangeException("Bad openings int array given when trying to create navdirections");
			}
			OpenUp = openings[0] == 1;
			OpenRight = openings[1] == 1;
			OpenDown = openings[2] == 1;
			OpenLeft = openings[3] == 1;
		}
		public NavDirections(bool openUp = true, bool openRight = true, bool openDown = true, bool openLeft = true)
		{
			OpenUp = openUp;
			OpenRight = openRight;
			OpenDown = openDown;
			OpenLeft = openLeft;
		}

		public Vector3Int[] AsV3IArray()
		{
		
			List<Vector3Int> connectedDirections = new List<Vector3Int>();
			if (OpenUp)
			{
				connectedDirections.Add(new Vector3Int(0,1,0));
			}
			if (OpenRight)
			{
				connectedDirections.Add(new Vector3Int(1,0,0));
			}
			if (OpenDown)
			{
				connectedDirections.Add(new Vector3Int(0,-1,0));
			}
			if (OpenLeft)
			{
				connectedDirections.Add(new Vector3Int(-1,0,0));
			}

			return connectedDirections.ToArray();
		}
		


	}
	
	//Enums
	
	public enum PathStatus
	{
		failure,
		searching,
		success
	}
}