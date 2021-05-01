using Bloops.GridFramework.Commands;
using UnityEngine;

namespace Bloops.GridFramework.Agents
{
	public class FollowerAgent : FacingAgent
	{
		private const int movePriority = 15;
		protected override void OnNewSubMove(Move m, SubMove sm)
        {
	        //distance between us and the cell we would move to (the thing we are following's starting cell).
            Vector3Int dir = sm.startingNode.cellPos - _node.cellPos;
        	if (dir.magnitude == 1 && dir != -sm.dir)//we are one block away, and not being pushed.
            {
	            bool agentIsPlayer = (sm.agent is Player);
	            if (!(sm.agent is FollowerAgent) && !agentIsPlayer)
	            {
		            return;
	            }
                SubMove otherMove = m.GetSubMoveWithDestination(sm.startingNode);
                if (otherMove != null)
                {
	                if (otherMove.subMovePriority < 10)//ijjj
	                {
		                //that other thing is less important.
		                otherMove.Invalidate();
	                }
	                else
	                {
		                //we can't move, a more important thing is moving.
		                return;
	                }
                }
	                int bonus = agentIsPlayer ? 2 : 0;//prioritize first leader in a chain
	                bonus += (dir == sm.dir ? 1 : 0);//prioritize following straight over following curved
	                m.AddAgentToMove(this, dir, false, null, sm, movePriority + bonus);

                }else{
	            
	            //If we are being pushed and are one block away.
	            if (sm.destinationNode == _node && dir.magnitude == 1 && dir == -sm.dir)
	            {
		            //a tile agent is moving to here, we need to either invalidate the move or get pushed or move out of the way or whatever.
		            if (CanMoveInDir(sm.dir))
		            {
			            if(m.GetValidSubMove(this,out SubMove sm2))
			            {
				            sm.SetReliesOn(sm2);
			            }
			            //critical to move should be true! but only if the move that was passed to us is critical. If we are being pushed by a stickyblock, then we aren't critical....
			            //If we ARE being pushed, criticalFor should NOT be null, or the player could end up landing on the current fella in odd edge cases.
			            m.AddAgentToMove(this,sm.dir,false,sm,sm,12);//this needs to be lower than the follower ones, or a follower 'further back' on a chain will push those ahead of it instead of them moving to the side.
		            }
		            else
		            {
			            //If we are moving already, then lets just move on out of the way!
			            if(m.GetValidSubMove(this,out SubMove sm2))
			            {
				            sm.SetReliesOn(sm2);
			            }
			            else
			            {
				            // sm.Invalidate();//cant move in dir that sm moves in...
				            //we need to tell the system that sm MIGHT be valid if we move out of the way... but we don't yet know if that will happen.
							sm.ReliesOnAgentMoving(this);
			            }
		            }
	            }
            }
        }
		
		public override void MoveTo(SubMove subMove, Move contextMove)
		{
			SetFacingDir((Vector2Int)subMove.dir);
			simpleLerpCoroutine = StartCoroutine(SimpleMoveLerp(subMove.destinationNode, 10, contextMove));
		}
	}
}