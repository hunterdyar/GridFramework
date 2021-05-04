using System;
using System.Collections;
using System.Collections.Generic;
using Bloops.GridFramework.Managers;
using Bloops.Utilities;
using UnityEngine;

namespace Bloops.GridFramework.Utility
{
    public class CameraFitter : MonoBehaviour
    {
        public PuzzleManager puzzleManager;

        // Start is called before the first frame update
        void OnEnable()
        {
            puzzleManager.onGameReady += FitCamera;
        }

        void OnDisable()
        {
            puzzleManager.onGameReady -= FitCamera;
        }

        void FitCamera()
        {
            var cellMin = puzzleManager.tilemapNavigation.Min;
            var cellmax = puzzleManager.tilemapNavigation.Max;
            Vector2 min = (Vector2) puzzleManager.tilemapNavigation.CellToWorld(cellMin);
            Vector2 max = puzzleManager.tilemapNavigation.CellToWorld(cellmax);
            min = min - Vector2.one;
            max = max + Vector2.one;
            Rect w = new Rect();
            w.min = min;
            w.max = max;
            CameraUtility.SetCameraToRect(w);
        }
    }
}