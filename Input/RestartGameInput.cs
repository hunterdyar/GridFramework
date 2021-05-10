using System.Linq;
using Bloops.LevelManager;
using Bloops.StateMachine;
using UnityEngine;

namespace Bloops.GridFramework.Input
{
    public class RestartGameInput : MonoBehaviour
    {
        [SerializeField] private State[] DependantOnStates;

        // Update is called once per frame
        void Update()
        {
            if (UnityEngine.Input.GetKeyDown(KeyCode.R))
            {
                if (DependantOnStates.Any(s => s.IsActive))
                {
                    LevelsManager.RestartCurrentLevel();
                }
            }
        }
    }

}