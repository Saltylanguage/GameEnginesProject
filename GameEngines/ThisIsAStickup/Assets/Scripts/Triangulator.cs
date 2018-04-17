﻿using UnityEngine;
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

    public class AngleComparer : IComparer<Vector3>
    {
        Vector3 mReferencePoint;

        public AngleComparer(Vector3 referencePoint)
        {
            mReferencePoint = referencePoint;
        }

        public int Compare(Vector3 a, Vector3 b)
        {
            var first = Vector3.Normalize(a - mReferencePoint);
            var second = Vector3.Normalize(b - mReferencePoint);
            return -Vector3.Dot(first, Vector3.right).CompareTo(Vector3.Dot(second, Vector3.right));
        }
    }
    public List<Vector3> allPoints = new List<Vector3>();
    public List<Vector3> convexHullPoints = new List<Vector3>();
    public List<Vector3> innerPoints = new List<Vector3>();

    public List<Geometry.Triangle> triangles = new List<Geometry.Triangle>();
    public List<Geometry.Triangle> subTriangles = new List<Geometry.Triangle>();

    public List<Geometry.Circle> circumCircleList = new List<Geometry.Circle>();

    Geometry.Line fromPointToRight = new Geometry.Line();
    Geometry.Circle testCircumCircle = new Geometry.Circle();

    Vector3[] intersectionPoints = new Vector3[3];
    public Geometry.Line[] testLines = new Geometry.Line[3];

    int intersectionCount = 0;
    bool stopFlag = false;

    // Use this for initialization
    void Start()
    {
        //GenerateTestPoints();
        GeneratePoints();
        Sort(ref allPoints);
        convexHullPoints = GenerateConvexHull(allPoints);
        GetInnerPoints();
        Triangulate();

        testCircumCircle = CalculateCircumcircle(triangles[0]);

    }

    // Update is called once per frame
    void Update()
    {

        DrawConvexHull();
        DrawTriangles();
        Debug.DrawLine(fromPointToRight.start, fromPointToRight.end, Color.cyan);

        Debug.DrawLine(testLines[0].start, testLines[0].end, Color.red);
        Debug.DrawLine(testLines[1].start, testLines[1].end, Color.green);
        Debug.DrawLine(testLines[2].start, testLines[2].end, Color.blue);
    }


    Vector3 FindIntersectionPoint(Geometry.Line l1, Geometry.Line l2)
    {
        float det = l1.rise * l2.run - l2.rise * l1.run;
        float x = float.MinValue;
        float y = float.MinValue;
        if (det == 0)
        {
            //Geometry.Lines are parallel
        }
        else
        {
            x = ((l2.run * l1.c) - (l1.run * l2.c)) / det;
            y = ((l1.rise * l2.c) - (l2.rise * l1.c)) / det;
        }

        return new Vector3(x, 0, y);
    }

    void Triangulate()
    {
        FirstTriangulatePass();
        SubDivideTriangles();
    }

    void FirstTriangulatePass()
    {
        for (int i = 1; i < convexHullPoints.Count - 2; i++)
        {
            triangles.Add(new Geometry.Triangle(convexHullPoints[0], convexHullPoints[i], convexHullPoints[i + 1]));
        }
        triangles.Add(new Geometry.Triangle(convexHullPoints[0], convexHullPoints[convexHullPoints.Count - 2], convexHullPoints[convexHullPoints.Count - 1]));
    }

    void SubDivideTriangles()
    {
        bool triangleFound = false;
        for (int i = 0; i < innerPoints.Count; i++)
        {
            for (int j = 0; j < triangles.Count; j++)
            {
                triangleFound = InsideOutsideCheck(triangles[j], innerPoints[i]);

                if (triangleFound)
                {
                    //Need to subdivide this triangle by creating a new triangle by drawing a line from the current inner point to all 3 vertices of the current triangle
                    //That means:
                    //Creating 3 new lines from the current point to points A, B and C of the current triangle
                    //Keep track of these lines separately.  PointToA, PointToB and PointToC
                    //Construct 3 new Triangles from these new lines and the lines of the current triangle (subdivisions):
                    //Delete the current Triangle from the list and add the 3 new triangles to the list.
                    int x = 5;
                    int y = x + 5;
                    Geometry.Triangle temp = triangles[j];
                    triangles.Add(new Geometry.Triangle(innerPoints[i], temp.pointA, temp.pointB));
                    triangles.Add(new Geometry.Triangle(innerPoints[i], temp.pointB, temp.pointC));
                    triangles.Add(new Geometry.Triangle(innerPoints[i], temp.pointC, temp.pointA));
                    triangles.RemoveAt(j);

                    break;
                }
                if (triangleFound)
                {
                    break;
                }
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
                if (allPoints[i + 1].z < allPoints[i].z)
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
        Triangulate();
    }

    //CONVEX HULL CODE STARTS HERE

    public void GeneratePoints()
    {
        float y = 0.0f;
        for (int index = 0; index < 10; index++)
        {
            float randx = Random.Range(0, 100);
            float randz = Random.Range(0, 100);

            allPoints.Add(new Vector3(randx, y, randz));
        }
    }
    public void GenerateTestPoints()
    {
        //allPoints.Add(new Vector3(34, 0, 0));
        //allPoints.Add(new Vector3(97, 0, 15));
        //allPoints.Add(new Vector3(96, 0, 86));
        //allPoints.Add(new Vector3(84, 0, 23));
        //allPoints.Add(new Vector3(46, 0, 0));
        //allPoints.Add(new Vector3(97, 0, 15));
        //allPoints.Add(new Vector3(84, 0, 23));
        //allPoints.Add(new Vector3(96, 0, 86));
        //allPoints.Add(new Vector3(62, 0, 54));
        //allPoints.Add(new Vector3(42, 0, 98));
        //allPoints.Add(new Vector3(33, 0, 44));
        //allPoints.Add(new Vector3(6, 0, 82));
        //allPoints.Add(new Vector3(0, 0, 56));
        //allPoints.Add(new Vector3(8, 0, 36));
        //allPoints.Add(new Vector3(21, 0, 16));
        //allPoints.Add(new Vector3(34, 0, 0));


        //NEW TEST CONDITION
        allPoints.Add(new Vector3(72, 0, 6));
        allPoints.Add(new Vector3(97, 0, 23));
        allPoints.Add(new Vector3(99, 0, 83));
        allPoints.Add(new Vector3(76, 0, 34));
        allPoints.Add(new Vector3(52, 0, 78));
        allPoints.Add(new Vector3(19, 0, 88));
        allPoints.Add(new Vector3(24, 0, 27));

        //Problem Triangle!
        //allPoints.Add(new Vector3(72, 0, 6));
        //allPoints.Add(new Vector3(99, 0, 83));
        //allPoints.Add(new Vector3(76, 0, 34));
        //allPoints.Add(new Vector3(52, 0, 78));
    }

    void OnDrawGizmos()
    {
        Gizmos.color = Color.blue;
        Gizmos.DrawSphere(allPoints[0], 2.0f);
        Gizmos.color = Color.red;
        for (int i = 1; i < allPoints.Count; i++)
        {
            Gizmos.DrawSphere(allPoints[i], 2.0f);

        }
        Gizmos.color = Color.green;
        for (int i = 0; i < 3; i++)
        {
            Gizmos.DrawCube(intersectionPoints[i], new Vector3(1, 1, 1));
        }
        //Gizmos.DrawSphere(testCircumCircle.center, testCircumCircle.radius);
    }
    public void DrawConvexHull()
    {
        Color color = Color.green;
        for (int i = 0; i < convexHullPoints.Count - 1; i++)
        {
            if (i >= 1)
            {
                color = Color.green;
            }
            Debug.DrawLine(convexHullPoints[i], convexHullPoints[i + 1], color);
        }
        Debug.DrawLine(convexHullPoints[convexHullPoints.Count - 1], convexHullPoints[0], Color.green);
    }
    void DrawTriangles()
    {
        Color color = Color.yellow;
        for (int i = 0; i < triangles.Count; i++)
        {
            Debug.DrawLine(triangles[i].pointA, triangles[i].pointB, color);
            Debug.DrawLine(triangles[i].pointB, triangles[i].pointC, color);
            Debug.DrawLine(triangles[i].pointC, triangles[i].pointA, color);
        }

        for (int i = 0; i < subTriangles.Count; i++)
        {
            Debug.DrawLine(subTriangles[i].pointA, subTriangles[i].pointB, color);
            Debug.DrawLine(subTriangles[i].pointB, subTriangles[i].pointC, color);
            Debug.DrawLine(subTriangles[i].pointC, subTriangles[i].pointA, color);
        }
    }

    public void SortByAngle(ref List<Vector3> points)
    {
        //Find Angle based on initial point and Z Axis
        var startPoint = points[0];
        AngleComparer comparer = new AngleComparer(startPoint);
        var zAxis = -Vector3.forward;
        points.Sort(1, points.Count - 1, comparer);
    }
    public void Sort(ref List<Vector3> points)
    {
        points.Sort((a, b) => a.z.CompareTo(b.z));
        SortByAngle(ref points);
    }

    public List<Vector3> GenerateConvexHull(List<Vector3> points)
    {
        //Start Conditions:
        // Find the bottom most point and add it to the stack
        // Add p2 to the stack

        // Algorithm:
        List<Vector3> convexStack = new List<Vector3>();
        convexStack.Add(points[0]);
        convexStack.Add(points[1]);

        int count = points.Count;
        for (int i = 2; i < count; ++i)
        {
            //  Step 1 
            //  Grab next point(n) in the list,
            Vector3 start = convexStack[convexStack.Count - 2];
            Vector3 pivot = convexStack[convexStack.Count - 1];
            Vector3 end = points[i];

            //TODO: Triangles are populating their points in the wrong order.
            float result = LeftRightCheck(start, pivot, end);

            if (result > 0)
            {
                convexStack.Add(end);
            }
            else if (result < 0)
            {
                convexStack.RemoveAt(convexStack.Count - 1);
                --i;
            }
            else
            {
                if (Vector3.SqrMagnitude(pivot - start) < Vector3.SqrMagnitude(end - start))
                {
                    convexStack.RemoveAt(convexStack.Count - 1);
                    convexStack.Add(end);
                }
            }
        }

        //  Step 2
        //  Make a line with the top 2 points from the stack, and another line with point(n) and the top stack.  If cross product is negative left turn, otherwise right turn 

        //  Step 3
        //  if left add to stack and move to next point
        //  if right pop and repeat step 2

        // convexHullPoints = convexStack;
        return convexStack;

    }
    public void GetInnerPoints()
    {
        for (int i = 0; i < allPoints.Count; i++)
        {
            if (!convexHullPoints.Contains(allPoints[i]))
            {
                innerPoints.Add(allPoints[i]);
            }
        }
    }

    float LeftRightCheck(Vector3 pointA, Vector3 pointB, Vector3 queryPoint)
    {
        float ret = 0;

        ret = Mathf.Sign((pointB.x - pointA.x)  //ABx
            * (queryPoint.z - pointA.z)         //QAz
            - (pointB.z - pointA.z) *           //ABz
              (queryPoint.x - pointA.x));       //AQx

        return ret;
    }
    public bool InsideOutsideCheck(Geometry.Triangle triangle, Vector3 point)
    {
        List<Vector3> trianglePoints = new List<Vector3>();
        trianglePoints.Add(triangle.pointA);
        trianglePoints.Add(triangle.pointB);
        trianglePoints.Add(triangle.pointC);
        trianglePoints.Add(point);

        Sort(ref trianglePoints);
        List<Vector3> convexHull = GenerateConvexHull(trianglePoints);
        if (convexHull.Count == 3)
        {
            if (convexHull.Contains(point))
            {
                return false;
            }
            else
            {
                return true;
            }
        }
        else
        {
            return false;
        }
    }

    public bool Intersect(Geometry.Line a, Geometry.Line b)
    {

        // http://local.wasp.uwa.edu.au/~pbourke/geometry/lineline2d/

        float ua = ((a.end.x - a.start.x) * (b.start.y - a.start.y)) - ((a.end.y - a.start.y) * (b.start.x - a.start.x));
        float ub = ((b.end.x - b.start.x) * (b.start.y - a.start.y)) - ((b.end.y - b.start.y) * (b.start.x - a.start.x));
        float denom = ((a.end.y - a.start.y) * (b.end.x - b.start.x)) - ((a.end.x - a.start.x) * (b.end.y - b.start.y));

        // First check for special cases
        if (denom == 0.0f)
        {

            if (ua == 0.0f && ub == 0.0f)
            {
                // The line segments are the same
                return true;
            }
            else
            {
                // The line segments are parallel
                return false;
            }
        }

        ua /= denom;
        ub /= denom;

        if (ua < 0.0f || ua > 1.0f || ub < 0.0f || ub > 1.0f)
        {
            return false;
        }

        return true;

    }
    public int GetNumIntersections(Geometry.Triangle triangle, Geometry.Line pointToRight)
    {
        int index;
        for (int i = 0; i < 3; i++)
        {
            if (Intersect(triangle.lines[i], pointToRight))
            {
                intersectionCount++;
                if (i == 2)
                {
                    index = 0;
                }
                else
                {
                    index = i + 1;
                }
                Vector3 intersection = FindIntersectionPoint(triangle.lines[index], pointToRight);

                pointToRight = new Geometry.Line(intersection, intersection + (Vector3.right * 100));
                fromPointToRight = pointToRight;
            }
            else
            {
                break;
            }
        }

        return intersectionCount;
    }


    public Geometry.Circle CalculateCircumcircle(Geometry.Triangle triangle)
    {
        Vector3[] midpoints = new Vector3[3];
        Vector3[] bisectors = new Vector3[3];

        Geometry.Line[] edges = new Geometry.Line[3];



        for (int i = 0; i < 3; i++)
        {
            midpoints[i] = CalculateMidpoint(triangle.lines[i]);

            Vector3 temp = midpoints[i] - triangle.lines[i].start;

            bisectors[i].x = -temp.z;
            bisectors[i].y = 0.0f;
            bisectors[i].z = temp.x;
        }


        edges[0] = new Geometry.Line(midpoints[0], bisectors[0] + midpoints[0]);
        edges[1] = new Geometry.Line(midpoints[1], bisectors[1] + midpoints[1]);
        edges[2] = new Geometry.Line(midpoints[2], bisectors[2] + midpoints[2]);




        if (!stopFlag)
        {
            testLines[0] = edges[0];
            testLines[1] = edges[1];
            testLines[2] = edges[2];
            stopFlag = true;
        }
        intersectionPoints[0] = FindIntersection(edges[0], edges[1]);
        intersectionPoints[1] = FindIntersection(edges[1], edges[2]);
        intersectionPoints[2] = FindIntersection(edges[2], edges[0]);
        bool isCenter = (intersectionPoints[0] == intersectionPoints[1] && intersectionPoints[1] == intersectionPoints[2]);

        Vector3 vertexPosition = edges[0].start;
        Vector3 circlePosition = intersectionPoints[0];
        float cirlceRadius = Vector3.Distance(vertexPosition, circlePosition);
        //WE HAVE MIDPOINTS!
        //WE HAVE BISECTORS!
        //WE HAVE LINES!
        //WE HAVE INTERSECTION POINTS (POSITION)
        //WE HAVE DISTANCE (RADIUS)
        //WE HAVE THE CIRCLE!

        return new Geometry.Circle(circlePosition, cirlceRadius);
    }

    public Vector3 CalculateMidpoint(Geometry.Line line)
    {
        float x = (line.end.x + line.start.x) / 2.0f;
        float y = 0;
        float z = (line.end.z + line.start.z) / 2.0f;
        Vector3 midpoint = new Vector3(x, y, z);

        return midpoint;
    }

    public Vector3 FindIntersection(Geometry.Line A, Geometry.Line B)
    {
        float A1 = A.end.z - A.start.z;
        float B1 = A.start.x - A.end.x;
        float C1 = A1 * A.start.x + B1 * A.start.z;

        float A2 = B.end.z - B.start.z;
        float B2 = B.start.x - B.end.x;
        float C2 = A2 * B.start.x + B2 * B.start.z;

        float det = A1 * B2 - A2 * B1;
        if (det == 0)
        {
            return Vector3.up;
        }
        else
        {
            float x = (B2 * C1 - B1 * C2) / det;
            float z = (A1 * C2 - A2 * C1) / det;
            return new Vector3(x, 0, z);
        }
    }


}



//First get the convex hull. 
//Use the convex hull to triangulate using ear clipping method, (or just fan it)
//Use a separate data strucutre to keep track of the list of points that are NOT in the convex hull.
//Iterate over these inner points and do the following:
//Draw a line from each inner point, to each vertex of the triangle it is inside.  
//(to calculate if a point is within a triangle use a raycast: odd intersection means inside, even means outside)
//Just use a vector and test for intersection.


//TODO solve edge case for when the y values are the same.  If 3 points are colinear, they will not form a triangle

//Calculating Circumcircle of a triangle
/*
    1) First Get the midpoints of all 3 edges 
    2) Get the inside facing Normal (bisector) of each edge 
    3) calculate where these 3 normals intersect (this gives you the midpoint)
    4) The distance from all 3 vertices to the intersesction of their midpoints should be equidistant
        - this distance is the radius of the circle.

    5)Now we have a centre and a radius.  Enough for our circle. :)
     
     
*/





//GARBAGE CODE


//public bool InsideOutsideCheck(Geometry.Triangle triangle, Vector3 point)
//{
//    Vector3 rightVector = Vector3.right * 100;
//    Geometry.Line pointToRight = new Geometry.Line(point, point + rightVector);
//    fromPointToRight = pointToRight;
//    GetNumIntersections(triangle, pointToRight);

//    if (intersectionCount % 2 == 0)
//    {
//        return true;
//    }
//    else
//    {
//        return true;
//    }
//}
