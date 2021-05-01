using System.Collections.Generic;
using Bloops.GridFramework.Agents;
using Bloops.GridFramework.Commands;
using UnityEngine;

namespace Bloops.GridFramework.Items.Laser
{
    public class LaserSource : ItemBase
    {
        public GameObject beamPrefab;
        public Vector3Int laserDirection;
        public LayerMask laserStoppingLayers;
        private List<LaserBeam> beams = new List<LaserBeam>();//create a pool.
        private AgentBase agent;
        private List<ILaserActivated> _activated = new List<ILaserActivated>();
        [SerializeField] private bool createBeamsOnMirrors = false;
        public void FireBeam()
        {
            //If we are being "carried" by a tileAgent component, lets update use their node for our logic.
            //todo: update this variable with listenrs, and do it in the floorbase?
            //Todo: seems we should move things to a parent class of just "something that exists in the grid". 
            if (agent != null)
            {
                _node = agent.CurrentNode;
                ClearBeams();
                ExtendBeamOneSquare(_node.cellPos,laserDirection);
            }
            else
            {
                Debug.LogWarning("Laser source has no agent component.");
            }
        }

        //I dono't know if the unity standard for using event functions with parent objects.
        void Awake()
        {
            agent = GetComponent<AgentBase>();
            //
        }

        public override void ItemInitiation()
        {
            base.ItemInitiation();
            FireBeam();
        }
        
        protected new void OnEnable()
        {
            base.OnEnable();
            // puzzleManager.onGameReady += FireBeam;
            puzzleManager.CommandManager.OnUndo += FireBeam;
            puzzleManager.CommandManager.OnRedo += FireBeam;

        }
        protected new void OnDisable()
        {
            base.OnDisable();
            // puzzleManager.onGameReady -= FireBeam;
            puzzleManager.CommandManager.OnUndo -= FireBeam;
            puzzleManager.CommandManager.OnRedo -= FireBeam;
        }

        void ExtendBeamOneSquare(Vector3Int current, Vector3Int dir)
        {
            Vector3Int lPos = current + dir;
            if (_navigation.GetNode(lPos, out var next))
            {
                //todo a consistent one-place place to check various status's of the thing, if walkable, if agent, etc
                if (next.walkable)
                {
                    Vector3Int newDir = dir;
                    bool laserHitAMirror = false;
                    bool laserHitAThing = false;
                    if (next.IsAgentHere)
                    {
                        if (next.AgentBaseHere is Mirror mirror)
                        {
                            newDir = mirror.OutputDir(dir);
                            if (dir == Vector3.zero || mirror.HasLaserSource(this))//hit a mirror, but the wrong side of it, or this mirror is already lasering (ie: no infinite loops).
                            {
                                laserHitAMirror = false;
                            }
                            else
                            {
                                mirror.SetLaserSource(this);
                                _activated.Add(mirror);
                                laserHitAMirror = true;
                                // laserHitAThing = true;
                            }
                        }else
                        {
                            //If our thing is LaserActivated, and we haven't already activated it.
                            if (next.AgentBaseHere is ILaserActivated la && !_activated.Contains(la))
                            {
                                _activated.Add(la);
                                la.SetLaserSource(this);
                            }
                        }

                        //todo write an extension function to compare a gameObject to a layerMask.
                        laserHitAThing = ((laserStoppingLayers.value & (1 << next.AgentBaseHere.gameObject.layer)) > 0);
                    }

                    if (laserHitAMirror)
                    {
                        if (createBeamsOnMirrors)
                        {
                            CreateBeam(lPos, newDir, Mirror.AngleFromDirs(dir, newDir));
                        }

                        ExtendBeamOneSquare(lPos,newDir);
                    }

                    if (!laserHitAThing)
                    {
                        CreateBeam(lPos,dir);
                        ExtendBeamOneSquare(lPos,newDir);
                    }

                    //newDir defaults to dir when no mirror is involved.
                }
            }
        }

        private void CreateBeam(Vector3Int current, Vector3Int dir, MirrorAngle a = MirrorAngle.none)
        {
            LaserBeam beam = GetBeamFromPool();
            beam.transform.position = _navigation.CellToWorld(current);
            if (a != MirrorAngle.none)
            {
                beam.SetAngle(a);
            }
            else
            {
                beam.SetOrientation(dir);
            }
            
            
            //Force check to fix race-condition things.
            beam.ManualInitiate(puzzleManager);
        }

        private void ClearBeams()
        {
            foreach(var laserActivated in _activated)
            {
                laserActivated.RemoveLaserSource(this);
            }
            _activated.Clear();
            foreach (var beam in beams)
            {
                beam.gameObject.SetActive(false);
            }
        }

        private void DestroyBeams()
        {
            foreach (var beam in beams)
            {
              Destroy(beam.gameObject);
            }

            beams.Clear();
            _activated.Clear();
        }

        private LaserBeam GetBeamFromPool()
        {
            foreach (var beam in beams)
            {
                if (beam.gameObject.activeInHierarchy == false)
                {
                    beam.gameObject.SetActive(true);
                    return beam;
                }
            }

            LaserBeam newBeam = Instantiate(beamPrefab, transform).GetComponent<LaserBeam>();
            beams.Add(newBeam);
            return newBeam;
        }
        
        protected override void AfterAnyMove(Move move)
        {
            FireBeam();
        }
        
        protected override void AgentEndedMoveHere(AgentBase agentBase)
        {
            //tell agent that collision event has happened, and its a laser!
        }

        protected override void AgentLeftHere(AgentBase agentBase)
        {
            //tell agent that a collision event has happened, and its a laser!
        }
    }
}