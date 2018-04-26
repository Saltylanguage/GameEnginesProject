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

            triangulator.roomGen.roomPositions.Clear();
            triangulator.roomGen.SetRoomPositions();
            for (int i = 0; i < triangulator.roomGen.roomPositions.Count; i++)
            {
                Vector3 temp = triangulator.roomGen.roomPositions[i];
                temp.y += 20;
                triangulator.allPoints.Add(temp);
            }

            //triangulator.GeneratePoints();
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
        //if (GUILayout.Button("Next Circle"))
        //{
        //    triangulator.debugIndex++;
        //    if (triangulator.debugIndex >= triangulator.triangles.Count)
        //    {
        //        triangulator.debugIndex = 0;
        //    }
        //}



        if (GUILayout.Button("MST Mode"))
        {
            triangulator.mstMode = true;
            triangulator.convexMode = false;
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
            triangulator.convexMode = true;
            triangulator.mstMode = false;
        }

        if (GUILayout.Button("Triangulon!!!"))
        {
            triangulator.convexMode = false;
            triangulator.mstMode = false;
        }
    }
}

