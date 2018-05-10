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
            triangulator.mTriangles.Clear();

            triangulator.roomGen.roomPositions.Clear();
        }

        if (GUILayout.Button("Clear"))
        {
            triangulator.allPoints.Clear();
            triangulator.convexHullPoints.Clear();
            triangulator.innerPoints.Clear();
            triangulator.mTriangles.Clear();

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
            //SHOULD BE GOOD UNTIL HERE!
            List<Geometry.Triangle> tempTriangles = new List<Geometry.Triangle>();
            do
            {
                Debug.Log("got here!");
                tempTriangles.Clear();
                for(int i = 0; i < triangulator.mTriangles.Count; i++)
                {
                    tempTriangles.Add(new Geometry.Triangle(triangulator.mTriangles[i].pointA, triangulator.mTriangles[i].pointB, triangulator.mTriangles[i].pointC));
                }
 
                //as of here we have 2 copies of the triangle list
 
                maxPoints = triangulator.DelaunayPass(tempTriangles, maxPoints);
                Debug.Log("POINT COUNT = " + maxPoints);                
                //triangulator.mTriangles = tempTriangles;
            } while (maxPoints > 3);

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
                triangulator.CalculateMinimumSpanningTree(triangulator.mTriangles);
            }

            for (int i = 0; i < triangulator.mTriangles.Count; i++)
            {
                for (int j = 0; j < 3; j++)
                {
                    if (!triangulator.minimumSpanningTree.Contains(triangulator.mTriangles[i].lines[j]))
                    {
                        int chance = (int)Random.Range(0, 100);
                        if (chance > 95)
                        {
                            triangulator.minimumSpanningTree.Add(triangulator.mTriangles[i].lines[j]);
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



// I want a list of triangles to start
// I want to make a copy of that list
// I want to go over every element of the copied list and make changes where necessary
// Finally Clear and Update the original list to the new adjusted copy list

//Index problem:  if you add or remove elements from the list you are iterating over indices get jumbled.

//