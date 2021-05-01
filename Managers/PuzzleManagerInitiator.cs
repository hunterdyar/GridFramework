using System.Collections;
using System.Collections.Generic;
using Bloops.GridFramework.Managers;
using Bloops.StateMachine;
using UnityEngine;

public class PuzzleManagerInitiator : MonoBehaviour
{
    public PuzzleManager manager;

    // Update is called once per frame
    IEnumerator Start()
    {
        manager.Initiate();
        manager.Start();
        yield return null;
        manager.TriggerMachine();
    }
}
