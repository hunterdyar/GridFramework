using System.Collections;
using System.Collections.Generic;
using Bloops.GridFramework.Navigation;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(NavTile))]
public class NavTileEditor : Editor
{
    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();
        //This kind of helpful warning should really be in the GridSelection component? i think....
        // if ((target as NavTile).nav == null)
        // {
        //     EditorGUILayout.HelpBox("No TilemapNavigation component found on the Tilemap object.",MessageType.Warning);
        //         
        // }
    }
}
