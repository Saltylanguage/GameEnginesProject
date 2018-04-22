using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

    [CustomEditor(typeof(Triangulator))]
public class HullTriangulatorEditor : Editor
{
    int passCount = 0;
    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();
        Triangulator triangulator = (Triangulator)target;

        if(GUILayout.Button("TrianglePass"))
        {
            passCount = triangulator.RunPassOnTriangles(triangulator.triangles.Count, passCount);
        }

        if (GUILayout.Button("ResetCount"))
        {
            passCount = 0;
        }
    }
}
