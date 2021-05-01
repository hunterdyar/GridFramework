using Bloops.GridFramework.Managers;
using Bloops.StateMachine;
using UnityEngine;
using UnityEngine.Serialization;

namespace Bloops.GridFramework.Input
{
    public class NonPlayerInput : MonoBehaviour
    {
        [SerializeField] private State dependantOnState;
        public PuzzleManager puzzleManager;
        void Update()
        {
            if (dependantOnState != null)
            {
                if (!dependantOnState.IsActive)
                {
                    return;
                }
            }
            //simple undo input. Todo: move this to keyboard input
            if (UnityEngine.Input.GetKeyDown(KeyCode.Z))
            {
                puzzleManager.CommandManager.Undo();
            }

            if (UnityEngine.Input.GetKeyDown(KeyCode.R))
            {
                puzzleManager.CommandManager.Redo();
            }
        }
    }
}
