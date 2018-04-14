using UnityEngine;
using System.Collections;
using System.Collections.Generic;



public class Triangulator : MonoBehaviour
{

    /*
     * 1. Sort all point by their x-coordinate first and by the y-coordinate if the x-coordinate of two points is identical.

2. Take the three first points of the sorted list to form the first triangle. You will need another data structure to remember the convex hull of the points you already have connected. 
    At first, it is all the three line of the first triangle.

3. Now iterate over your sorted list of points. For each point check if you need to connect with the two points of the line segments of the convex hull. This can form a new triangle. 
If it does you need to check as well if you have to flip the edge of the newly formed triangle and the existing neighboring one. Don't forget to update the convex hull.

Actually, the convex hull is not necessary for the algorithm to work. But, it will speed things up significantly and is not very hard to implement. 
(Remove an edge from the convex hull if it is part of a new triangle and add the other edges of the new triangle to the convex hull.)

You need data structures that work both ways: A triangle needs to know all its edges and vertices. And the edges need to know the corresponding triangles and its vertices. 
You should store all this information explicitly. It is easiest to use STL vectors of triangles, edges, and vertices and use indices within the data types for triangles and edges. 

How can I perform Delaunay Triangulation algorithm in C++ ??. Available from: https://www.researchgate.net/post/How_can_I_perform_Delaunay_Triangulation_algorithm_in_C [accessed May 19, 2017].
 
*/

    //public Triangulator(List<Vector3> pointMasterList, List<Vector3> hullpoints)
    //{
    //    allPoints = pointMasterList;
    //    ConvexHullPoints = hullpoints;

    //    innerPoints = allPoints;

    //    for (int i = 0; i < innerPoints.Count; i++)
    //    {
    //        if (ConvexHullPoints.Contains(innerPoints[i]))
    //        {
    //            innerPoints.Remove(innerPoints[i]);
    //        }
    //    }
    //}
   
    public List<Geometry.Triangle> triangles = new List<Geometry.Triangle>();
    public List<Vector3> allPoints = new List<Vector3>();
    public List<Vector3> ConvexHullPoints = new List<Vector3>();
    public List<Vector3> innerPoints = new List<Vector3>();

    Vector2 FindIntersectionPoint(Geometry.Line l1, Geometry.Line l2)
    {
        float det = l1.a * l2.b - l2.a * l1.b;
        float x = float.MinValue;
        float y = float.MinValue;
        if (det == 0)
        {
            //Geometry.Lines are parallel
        }
        else
        {
            x = ((l2.b * l1.c) - (l1.b * l2.c)) / det;
            y = ((l1.a * l2.c) - (l2.a * l1.c)) / det;
        }

        return new Vector2(x, y);
    }

    void DelaunayTriangulation()
    {

        FirstTriangulatePass();
        SubDivideTriangles();

    }

    void FirstTriangulatePass()
    {
        for (int i = 1; i < ConvexHullPoints.Count - 2; i++)
        {
            triangles.Add(new Geometry.Triangle(ConvexHullPoints[i - 1], ConvexHullPoints[i], ConvexHullPoints[i + 1]));
        }
    }

    bool PointIsInsideTriangle(Vector2 point, Geometry.Triangle triangle)
    {
        //Determine which triangle the point is inside  (it has to be in 1 since we are only calculating points inside the convex hull)
        bool pointIsInsideTriangle = false;
        Geometry.Line fromPointToRight = new Geometry.Line(point, (point - Vector2.right) * 100);

        if (triangle.minXY.x < point.x && triangle.minXY.y < point.y)
        {
            if (triangle.maxXY.x < point.x && triangle.maxXY.y < point.y)
            {
                Geometry.Line triangleEdge;
                for (int j = 0; j < 3; j++)
                {
                    triangleEdge = triangle.lines[j];

                    Vector2 intersection = FindIntersectionPoint(fromPointToRight, triangleEdge);

                    if (Mathf.Min(fromPointToRight.start.x, triangleEdge.start.x) < intersection.x && intersection.x < Mathf.Max(fromPointToRight.start.x, triangleEdge.start.x))
                    {
                        if (Mathf.Min(fromPointToRight.start.y, triangleEdge.start.y) < intersection.y && intersection.y < Mathf.Max(fromPointToRight.start.y, triangleEdge.start.y))
                        {
                            pointIsInsideTriangle = !pointIsInsideTriangle;
                        }
                    }
                }
            }
        }
        return pointIsInsideTriangle;
    }

    void SubDivideTriangles()
    {
        for (int i = 0; i < innerPoints.Count; i++)
        {
            for (int j = 0; j < triangles.Count; j++)
            {
                if (PointIsInsideTriangle(innerPoints[i], triangles[j]))
                {
                    //Need to subdivide this triangle by creating a new triangle by drawing a line from the current inner point to all 3 vertices of the current triangle
                    //That means:
                    //Creating 3 new lines from the current point to points A, B and C of the current triangle
                    //Keep track of these lines separately.  PointToA, PointToB and PointToC
                    //Construct 3 new Triangles from these new lines and the lines of the current triangle (subdivisions):

                    Geometry.Triangle temp = triangles[j];
                    triangles.Remove(triangles[j]);
                    triangles.Insert(j, new Geometry.Triangle(temp.pointA, temp.pointB, innerPoints[i]));
                    j++;
                    triangles.Insert(j, new Geometry.Triangle(temp.pointB, temp.pointC, innerPoints[i]));
                    j++;
                    triangles.Insert(j, new Geometry.Triangle(temp.pointC, temp.pointA, innerPoints[i]));
                    j++;

                    //Delete the current Triangle from the list and add the 3 new triangles to the list.
                }
            }
        }
    }

    void DrawTriangles()
    {
        Color color = Color.yellow;
        for (int i = 0; i < triangles.Count; i++)
        {
            for (int j = 0; j < 3; j++)
            {
                Debug.DrawLine(triangles[i].lines[j].start, triangles[i].lines[j].end, color);
            }
        }

    }


    public void CreateXAndYPoints()
    {
        GameObject[] rooms = GameObject.FindGameObjectsWithTag("Room");

        Debug.Log("Rooms: " + rooms.Length);

        //Grab Position data for each room and and throw it into a list
    }

    public void SortXPoints()
    {
        //Sort all points by x value
        allPoints.Sort((a, b) => a.x.CompareTo(b.x));
    }

    public void SortYPoints()
    {
        //in the case of equal x values, tie break by sorting by y value
        for (int i = 0; i < allPoints.Count - 1; i++)
        {
            if (allPoints[i].x == allPoints[i + 1].x)
            {
                if(allPoints[i+1].y < allPoints[i].y)
                {
                    var temp = allPoints[i];
                    allPoints[i] = allPoints[i + 1];
                    allPoints[i + 1] = temp;
                }
            }
        }
    }

    public void SortPoints()
    {
        SortXPoints();
        SortYPoints();
    }

    public void RunTriangulator()
    {    
        SortPoints();
        DelaunayTriangulation();
    }

    // Use this for initialization
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        DrawTriangles();
        //if (counter < 1)
        //{
        //    if (!roomGenerator.separateFlag)
        //    {
        //        counter++;
        //        RunTriangulator();
        //    }
        //}
    }
}



//First get the convex hull. 
//Use the convex hull to triangulate using ear clipping method, (or just fan it)
//Use a separate data strucutre to keep track of the list of points that are NOT in the convex hull.
//Iterate over these inner points and do the following:
//Draw a line from each inner point, to each vertex of the triangle it is inside.  
//(to calculate if a point is within a triangle use a raycast: odd intersection means inside, even means outside)
//Just use a vector and test for intersection.