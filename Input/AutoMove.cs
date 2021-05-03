
using System;
using Bloops.GridFramework.Agents;
using Bloops.StateMachine;
using UnityEngine;

namespace Bloops.GridFramework.Input
{
    public class AutoMove : MonoBehaviour
    {
        private bool _autoMove;
        private Player _player;
        [SerializeField] private State dependantOnState;

        private void Awake()
        {
            _player = GetComponent<Player>();
        }

        void Start()
        {
            _autoMove = false;
        }

        // Update is called once per frame
        void Update()
        {
            if (dependantOnState != null)
            {
                if (!dependantOnState.IsActive)
                {
                    return;
                }
            }
            
            if (UnityEngine.Input.GetButtonDown("Jump"))
            {
                _autoMove = true;
            }

            if (_autoMove && !_player.moving)
            {
                if (_player.CanMoveInDirs(out Vector3Int[] dirs) == 1)
                {
                    _player.Move((Vector2Int)dirs[0]);
                }
                else
                {
                    _autoMove = false;
                }
            }
        }

       
    }
}
