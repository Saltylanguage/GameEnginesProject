using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Triangulator : MonoBehaviour
{

    /*
     * 1. Sort all point by their x-coordinate first and by the y-coordinate if the x-coordinate of two points is identical.

2. Take the three first points of the sorted list to form the first triangle. You will need another data structure to remember the convex hull of the points you already have connected. At first, it is all the three line of the first triangle.

3. Now iterate over your sorted list of points. For each point check if you need to connect with the two points of the line segments of the convex hull. This can form a new triangle. 
If it does you need to check as well if you have to flip the edge of the newly formed triangle and the existing neighboring one. Don't forget to update the convex hull.

Actually, the convex hull is not necessary for the algorithm to work. But, it will speed things up significantly and is not very hard to implement. 
(Remove an edge from the convex hull if it is part of a new triangle and add the other edges of the new triangle to the convex hull.)

You need data structures that work both ways: A triangle needs to know all its edges and vertices. And the edges need to know the corresponding triangles and its vertices. 
You should store all this information explicitly. It is easiest to use STL vectors of triangles, edges, and vertices and use indices within the data types for triangles and edges. 

How can I perform Delaunay Triangulation algorithm in C++ ??. Available from: https://www.researchgate.net/post/How_can_I_perform_Delaunay_Triangulation_algorithm_in_C [accessed May 19, 2017].
 
*/

    List<float> xPoints = new List<float>();
    List<float> zPoints = new List<float>();
    List<Vector2> points = new List<Vector2>();

    List<Triangle> triangles = new List<Triangle>();

    public RoomGenerator roomGenerator;

    public class Triangle
    {
        public Vector3 pointA;
        public Vector3 pointB;
        public Vector3 pointC;
    }

    bool isDone;

    int counter = 0;

    public void CreateXAndYPoints()
    {
        GameObject[] rooms = GameObject.FindGameObjectsWithTag("Room");

        Debug.Log("Rooms: " + rooms.Length);
        
        for (int i = 0; i < rooms.Length; i++)
        {
            xPoints.Add(rooms[i].gameObject.transform.position.x);
            zPoints.Add(rooms[i].gameObject.transform.position.z);           
        }
    }

    public void SortXPoints()
    {
        xPoints.Sort();
    }

    public void SortYPoints()
    {
        for (int i = 0; i < xPoints.Count - 1; i++)
        {
            if (xPoints[i] == xPoints[i + 1])
            {
                float temp;
                if (zPoints[i] <= zPoints[i + 1])
                {
                    temp = zPoints[i];
                }
                else
                {
                    temp = zPoints[i + 1];
                }
                zPoints[i] = zPoints[i + 1];
                zPoints[i + 1] = temp;
            }
        }
    }

    public void MergePointValues()
    {
        for (int i = 0; i < xPoints.Count; i++)
        {
            points.Add(new Vector2(xPoints[i], zPoints[i]));
        }
    }

    public void CreateTriangles()
    {
        for (int i = 0; i < xPoints.Count - 2; i++)
        {   
            Debug.Log("Index = " + i);
            Debug.Log("Num Points = " + xPoints.Count);
            triangles.Add(new Triangle());
            triangles[i].pointA = new Vector3(xPoints[i], 100, zPoints[i]);
            triangles[i].pointB = new Vector3(xPoints[i + 1], 100, zPoints[i + 1]);
            triangles[i].pointC = new Vector3(xPoints[i + 2], 100, zPoints[i + 2]);
        }
    }

    public void RunTriangulator()
    {
        CreateXAndYPoints();
        SortXPoints();
        SortYPoints();
        MergePointValues();
        CreateTriangles();
    }

    // Use this for initialization
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

        if (counter < 1)
        {
            if (!roomGenerator.separateFlag)
            {
                counter++;
                RunTriangulator();
            }
        }
        else
        {
            for (int i = 0; i < xPoints.Count - 2; i++)
            {
                Debug.DrawLine(triangles[i].pointA, triangles[i].pointB, Color.green);
                Debug.DrawLine(triangles[i].pointB, triangles[i].pointC, Color.green);
                Debug.DrawLine(triangles[i].pointC, triangles[i].pointA, Color.green);
                }
            }
        }
    }
