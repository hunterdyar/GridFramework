using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Bloops.GridFramework.Agents;
using Bloops.GridFramework.Commands;
using Bloops.GridFramework.Navigation;
using UnityEngine;

public class MultiPlayer : Player
{
	public Player[] players;
	public override void SetCurrentNode(NavNode newNode)
	{
		//Surpresses errors for keeping this multiPlayer object off-grid
		//or if accidentally placing it on grid.

		//dont put it on one of the zombie players.
		//that shouldnt break anything core, but would fuss up the input scripts GetCOmponent calls.

		return;
	}

	protected override void TryNewMove((Vector2Int, bool) input)
	{
		_currentMove = new Move(puzzleManager, input.Item2); //we will only store the first move as a history point.

		foreach (var ab in players)
		{
			_currentMove.AddAgentToMove(ab, new Vector3Int(input.Item1.x, input.Item1.y, 0), false, null, null, 100);
			// ab.ForceOverrideCurrentMove(_currentMove);
		}

		puzzleManager.ExecutePlayerCommand(_currentMove);

		StartCoroutine(WaitForMoveToFinishThenUpdateGameLoop(_currentMove));
	}

	public override int CanMoveInDirs(out Vector3Int[] dirs)
	{
		HashSet<Vector3Int> dls = new HashSet<Vector3Int>();
		foreach (var ab in players)
		{
			foreach (var d in TilemapNavigation.directions)
			{
				if (CanMoveInDir(d))
				{
					dls.Add(d);
				}
			}
		}

		dirs = dls.ToArray();
		return dls.Count;
	}

	protected override void OnNewSubMove(Move m, SubMove sm)
	{
		//"accidentally in the scene" error prevention.
		return;
	}
	
	//i hate overriding event functions. makes me feel dirty.
	protected override void Update()
	{
		moving = false;
		foreach (var p in players)
		{
			if (p.moving)
			{
				moving = true;
				break;
			}
		}
		base.Update();
	}
}
