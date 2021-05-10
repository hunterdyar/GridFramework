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

	protected override void AgentInitiation()
	{
		base.AgentInitiation();
		if (players.Length == 0)
		{
			players = GameObject.FindObjectsOfType<Player>();
			players = players.Where(p => p != this).ToArray();
		}
	}

	
	protected override void TryNewMove((Vector2Int, bool) input)
	{
		//_currentMove = new Move(puzzleManager, input.Item2); //we will only store the first move as a history point.
		var moves = new List<Move>();
		for(int i = 0;i<players.Length;i++)
		{
			var m = new Move(puzzleManager, input.Item2 && i == 0);//only the first one should be a history state.
			// bool alreadyInvolved = false;

			//check that we aren't already involved in a move. If another player pushes this player, then this player should just get pushed.
			if (moves.Count(move => move.IsInvolved(players[i])) == 0)
			{
				m.AddAgentToMove(players[i], new Vector3Int(input.Item1.x, input.Item1.y, 0), false, null, null, 100);
				// ab.ForceOverrideCurrentMove(_currentMove);
				moves.Add(m);
			}
		}
		
		foreach (Move m in moves)
		{
			puzzleManager.ExecutePlayerCommand(m);
			if (m.IsValid)
			{
				_currentMove = m;
			}
		}
		
		foreach(Move m in moves){
			StartCoroutine(WaitForMoveToFinishThenUpdateGameLoop(_currentMove));
		}
	}

	public override int CanMoveInDirs(out Vector3Int[] dirs)
	{
		HashSet<Vector3Int> dls = new HashSet<Vector3Int>();
		foreach (var p in players)
		{
			foreach (var d in TilemapNavigation.directions)
			{
				if (p.CanMoveInDir(d))
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
