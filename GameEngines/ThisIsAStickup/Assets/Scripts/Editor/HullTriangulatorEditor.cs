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
            triangulator.allPoints.Clear();
            triangulator.convexHullPoints.Clear();
            triangulator.innerPoints.Clear();
            triangulator.triangles.Clear();

            triangulator.roomGen.roomPositions.Clear();            
        }

        if (GUILayout.Button("Clear"))
        {           
            triangulator.allPoints.Clear();
            triangulator.convexHullPoints.Clear();
            triangulator.innerPoints.Clear();
            triangulator.triangles.Clear();

            triangulator.roomGen.roomPositions.Clear();
        }

        if (GUILayout.Button("Trianglulate"))
        {
            triangulator.roomGen.SetRoomPositions();
            for (int i = 0; i < triangulator.roomGen.roomPositions.Count; i++)
            {
                Vector3 temp = triangulator.roomGen.roomPositions[i];            
                triangulator.allPoints.Add(temp);
            }

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

        if (GUILayout.Button("MST Mode"))
        {
            triangulator.triangleMode = false;
            triangulator.mstMode = true;
            triangulator.convexMode = false;
            triangulator.hallwayMode = false;
            if (triangulator.mstMode)
            {
                triangulator.minimumSpanningTree.Clear();
                triangulator.CalculateMinimumSpanningTree(triangulator.triangles);
            }

            for (int i = 0; i < triangulator.triangles.Count; i++)
            {
                for (int j = 0; j < 3; j++)
                {
                    if (!triangulator.minimumSpanningTree.Contains(triangulator.triangles[i].lines[j]))
                    {
                        int chance = (int)Random.Range(0, 100);
                        if (chance > 95)
                        {
                            triangulator.minimumSpanningTree.Add(triangulator.triangles[i].lines[j]);
                        }
                    }
                }
            }
        }

        if (GUILayout.Button("Convex Hull Mode"))
        {
            triangulator.triangleMode = false;
            triangulator.convexMode = true;
            triangulator.mstMode = false;
            triangulator.hallwayMode = false;
        }

        if (GUILayout.Button("Triangulon!!!"))
        {
            triangulator.triangleMode = true;
            triangulator.convexMode = false;
            triangulator.mstMode = false;
            triangulator.hallwayMode = false;
        }

        if (GUILayout.Button("Hallway Mode"))
        {
            triangulator.triangleMode = false;
            triangulator.convexMode = false;
            triangulator.mstMode = false;
            triangulator.hallwayMode = true;
        }



    }
}

