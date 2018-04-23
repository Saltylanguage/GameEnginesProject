using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(Triangulator))]
public class HullTriangulatorEditor : Editor
{

    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();
        Triangulator triangulator = (Triangulator)target;

        if (GUILayout.Button("Reset"))
        {
            triangulator.mstMode = false;
            triangulator.allPoints.Clear();
            triangulator.convexHullPoints.Clear();
            triangulator.innerPoints.Clear();
            triangulator.triangles.Clear();

            triangulator.GeneratePoints();
            triangulator.Sort(ref triangulator.allPoints);
            triangulator.convexHullPoints = triangulator.GenerateConvexHull(triangulator.allPoints);
            triangulator.GetInnerPoints();
            triangulator.Triangulate();

            int maxPoints = 0;

            for (int i = 0; i < 10; i++)
            {
                maxPoints = triangulator.RunPassOnTriangles(triangulator.triangles.Count, maxPoints);
            }
        }


        if (GUILayout.Button("Clear"))
        {
            triangulator.mstMode = false;
            triangulator.allPoints.Clear();
            triangulator.convexHullPoints.Clear();
            triangulator.innerPoints.Clear();
            triangulator.triangles.Clear();
        }

        if (GUILayout.Button("TrianglePass"))
        {
            int maxPoints = 0;
            maxPoints = triangulator.RunPassOnTriangles(triangulator.triangles.Count, maxPoints);
        }
        if (GUILayout.Button("Next Circle"))
        {
            triangulator.debugIndex++;
            if (triangulator.debugIndex >= triangulator.triangles.Count)
            {
                triangulator.debugIndex = 0;
            }
        }



        if (GUILayout.Button("MST Mode"))
        {
            triangulator.mstMode = true;
            triangulator.convexMode = false;
            if (triangulator.mstMode)
            {
                triangulator.minimumSpanningTree.Clear();
                triangulator.CalculateMinimumSpanningTree(triangulator.triangles);
            }
        }

        if (GUILayout.Button("Convex Hull Mode"))
        {
            triangulator.convexMode = true;
            triangulator.mstMode = false;
        }

        if (GUILayout.Button("Normal Mode"))
        {
            triangulator.convexMode = false;
            triangulator.mstMode = false;
        }
    }
}

