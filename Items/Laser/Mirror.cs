using System.Collections.Generic;
using Bloops.GridFramework.Agents;
using UnityEngine;

namespace Bloops.GridFramework.Items.Laser
{
	public class Mirror : AgentBase, ILaserActivated
	{
		//this object doesn't override anything about tileagent, we just want something solid and give it its own functionality.
		//neat, right!
		public MirrorAngle[] angles;
		private List<LaserSource> currentSources = new List<LaserSource>();
		void GraphicsCheck()
		{
			if(currentSources.Count > 0)
			{
				GetComponent<SpriteRenderer>().color = Color.red;
			}else
			{
				GetComponent<SpriteRenderer>().color = Color.cyan;

			}
		}
		
		
		public void SetLaserSource(LaserSource source)
		{
			if (!currentSources.Contains(source))
			{
				currentSources.Add(source);
				GraphicsCheck();
			}
		}

		public bool HasLaserSource(LaserSource source)
		{
			return currentSources.Contains(source);
		}
		public void RemoveLaserSource(LaserSource source)
		{
			if (currentSources.Contains(source))
			{
				currentSources.Remove(source);
				GraphicsCheck();
			}
		}
		public Vector3Int OutputDir(Vector3Int laserMovingDir)
		{
			Vector3Int laserInDir = -laserMovingDir;
			foreach (var angle in angles)
			{
				Vector3Int outDir = InToOut(angle,laserInDir);
				if (outDir != Vector3Int.zero)
				{
					return outDir;
				}
			}

			return Vector3Int.zero;
		}

		private Vector3Int InToOut(MirrorAngle angle, Vector3Int laserInDir)
		{
			switch (angle)
			{
				case MirrorAngle.RightUp:
					if(laserInDir == Vector3Int.right){return Vector3Int.up;}
					if(laserInDir == Vector3Int.up){return Vector3Int.right;}
					break;
				case MirrorAngle.RightDown:
					if(laserInDir == Vector3Int.right){return Vector3Int.down;}
					if(laserInDir == Vector3Int.down){return Vector3Int.right;}
					break;
				case MirrorAngle.LeftUp:
					if(laserInDir == Vector3Int.left){return Vector3Int.up;}
					if(laserInDir == Vector3Int.up){return Vector3Int.left;}
					break;
				case MirrorAngle.LeftDown:
					if(laserInDir == Vector3Int.left){return Vector3Int.down;}
					if(laserInDir == Vector3Int.down){return Vector3Int.left;}
					break;
				case MirrorAngle.HorizontalPass:
					if(laserInDir == Vector3Int.left){return Vector3Int.right;}
					if(laserInDir == Vector3Int.right){return Vector3Int.left;}
					break;
				case MirrorAngle.VerticalPass:
					if(laserInDir == Vector3Int.down){return Vector3Int.up;}
					if(laserInDir == Vector3Int.up){return Vector3Int.down;}
					break;
				default:
					//throw new ArgumentOutOfRangeException();
					return Vector3Int.zero;
			}
			return Vector3Int.zero;
		}

		public static MirrorAngle AngleFromDirs(Vector3Int inputDir, Vector3Int outputDir)
		{
			Vector3Int combined = inputDir + outputDir;
			if (combined.x > 0 && combined.y > 0)
			{
				return MirrorAngle.RightUp;
			}else if (combined.x > 0 && combined.y < 0)
			{
				return MirrorAngle.RightDown;
			}else if (combined.x < 0 && combined.y > 0)
			{
				return MirrorAngle.LeftUp;
			}else if (combined.x < 0 && combined.y < 0)
			{
				return MirrorAngle.LeftDown;
			}else if (combined.x == 0 && Mathf.Abs(combined.y) > 0)
			{
				return MirrorAngle.VerticalPass;
			}
			else if (Mathf.Abs(combined.x) > 0 && combined.y == 0)
			{
				return MirrorAngle.HorizontalPass;
			}

			return MirrorAngle.none;
		}
	}
	
	public enum MirrorAngle
	{
		none,
		RightUp,
		RightDown,
		LeftUp,
		LeftDown,
		HorizontalPass,
		VerticalPass
	}
}