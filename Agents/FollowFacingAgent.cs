using Bloops.GridFramework.Commands;
using UnityEngine;

namespace Bloops.GridFramework.Agents
{
    public class FollowFacingAgent : FacingAgent
    {
        protected override void OnNewSubMove(Move m, SubMove sm)
        {
            if (!m.IsInvolved(this))
            {
                if (sm.destinationNode == _node)//if we are being pushed.
                {
                    //a tile agent is moving to here, we need to either invalidate the move or get pushed or move out of the way or whatever.
                    if (CanMoveInDir(sm.dir) )
                    {
                        //critical to 
                        m.AddAgentToMove(this,sm.dir,false,sm,sm,20);
                    }
                    else
                    {
                        //we are being pushed, and we cannot move. So the submove tr
                        sm.Invalidate();
                    }
                }
                else
                {
                    Vector3Int dir = (Vector3Int)(sm.startingNode.cellPos - _node.cellPos);
                    if ((Vector3Int)FacingDir == dir && dir.magnitude == 1)//facingDir 
                    {
                        //We need to check if another agent is moving into the spot of the player.
                        
                        SubMove otherMove = m.GetSubMoveWithDestination(sm.startingNode);
                        if (otherMove != null)
                        {
                            if (otherMove.subMovePriority < 11)//ijjj
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
                        
                        if (CanMoveInDir(dir) )
                        {
                            //critical to 
                            SubMove newSM = m.AddAgentToMove(this,dir,false,null,sm,11);
                            newSM.SetExtraData((Vector2Int)sm.dir);
                        }
                    }
                }
            }
        }
        public override void MoveTo(SubMove subMove, Move contextMove)
        {
            if(subMove.GetExtraData() != null){
                SetFacingDir((Vector2Int)subMove.GetExtraData()); //the direction that the thing that moved is on, PROBABLY.
            }
            else
            {
                //SetFacingDir((Vector2Int) subMove.dir); 
            }
            simpleLerpCoroutine = StartCoroutine(SimpleMoveLerp(subMove.destinationNode, 10, contextMove));
        }
        public override void MoveToInstant(SubMove subMove, Move contextMove)
        {
            if(subMove.GetExtraData() != null){
                SetFacingDir((Vector2Int)subMove.GetExtraData()); //the direction that the thing that moved is on, PROBABLY.
            }
            else
            {
                //SetFacingDir((Vector2Int) subMove.dir); 
            }
            SnapTo(subMove.destinationNode);
        }
    }
}
