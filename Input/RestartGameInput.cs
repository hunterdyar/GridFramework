using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Bloops.LevelManager;
using Bloops.StateMachine;
using UnityEngine;
using UnityEngine.SceneManagement;

public class RestartGameInput : MonoBehaviour
{
    public LevelsManager levelManager;
    [SerializeField] private State[] DependantOnStates;

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.R))
        {
            if (DependantOnStates.Any(s => s.IsActive))
            {
                levelManager.RestartCurrentLevel();
            }
        }
    }
}
