using System;
using System.Collections;
using System.Collections.Generic;
using Bloops;
using Bloops.GridFramework.Managers;
using Bloops.GridFramework.Navigation;
using Bloops.StateMachine;
using UnityEngine;

public class AllTilesPaintedCondition : MonoBehaviour, ICondition
{
    [SerializeField] private Transition _transition;
    
    public PuzzleManager puzzleManager;

    private void OnEnable()
    {
        puzzleManager.itemInitiation += Init;
    }

    void Init()
    {
        _transition.AddCondition(this);
    }
    
    void OnDestroy()
    {
        _transition.RemoveCondition(this);
    }
    public bool GetCondition()
    {
        return !puzzleManager.tilemapNavigation.AnyUnpaintedTiles();
    }
}
