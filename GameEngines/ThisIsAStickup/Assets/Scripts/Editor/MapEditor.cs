using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(GridGenerator))]
public class MapEditor : Editor
{

    public override void OnInspectorGUI()
    {
        GridGenerator map = target as GridGenerator;

        if (DrawDefaultInspector())
        {
            if (!Application.isPlaying)
            {
                map.CreateGrid();
            }
        }

        if (GUILayout.Button("Generate Map"))
        {
            map.CreateGrid();
        }

    }
}

