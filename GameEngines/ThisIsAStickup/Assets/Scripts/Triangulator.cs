using UnityEditor;
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


    int circleIndex = 0;
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

        // testCircumCircle = CalculateCircumcircle(triangles[0]);

    }

    // Update is called once per frame
    void Update()
    {

        DrawConvexHull();
        DrawTriangles();
        Debug.DrawLine(fromPointToRight.start, fromPointToRight.end, Color.cyan);

        Vector3 midPoint = CalculateMidpoint(triangles[circleIndex].lines[0]);
        Vector3 temp = midPoint - triangles[circleIndex].lines[0].start;
        Vector3 bisector = new Vector3();
        bisector.x = -temp.z;
        bisector.y = 0.0f;
        bisector.z = temp.x;

        Geometry.Line lineA = new Geometry.Line(midPoint, bisector + midPoint);

        Debug.DrawLine(lineA.start, lineA.end, Color.green);

        if (Input.GetKeyDown(KeyCode.Space))
        {
            Debug.Log("Yo");
            circleIndex++;
            if (circleIndex >= circumCircleList.Count)
            {
                circleIndex = 0;
            }
        }
    }

    void OnDrawGizmosSelected()
    {
        Handles.color = Color.cyan;
        Handles.DrawWireDisc(circumCircleList[circleIndex].center, Vector3.up, circumCircleList[circleIndex].radius);
    }

    void Triangulate()
    {
        FirstTriangulatePass();
        SubDivideTriangles();

        for (int i = 0; i < triangles.Count; i++)
        {
            circumCircleList.Add(CalculateCircumcircle(triangles[i]));
        }

        int pointCount = 10;
        while(pointCount >=4)
        { 
        //for (int i = 0; i < 1000; i++)
        //{
            for (int i = 0; i < triangles.Count; i++)
            {
                pointCount = Mathf.Max(pointCount, DelaunayPass(triangles[i], circumCircleList[i]));
            }

        }
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

        testLines[0] = edges[0];
        testLines[1] = edges[1];
        testLines[2] = edges[2];

        intersectionPoints[0] = FindIntersection(edges[0], edges[1]);
        intersectionPoints[1] = FindIntersection(edges[1], edges[2]);
        intersectionPoints[2] = FindIntersection(edges[2], edges[0]);
        bool isCenter = (intersectionPoints[0] == intersectionPoints[1] && intersectionPoints[1] == intersectionPoints[2]);

        Vector3 vertexPosition = edges[0].start;


        Vector3 circlePosition = intersectionPoints[0];
        float distanceA = Vector3.Distance(triangle.pointA, circlePosition);
        float distanceB = Vector3.Distance(triangle.pointB, circlePosition);
        float distanceC = Vector3.Distance(triangle.pointC, circlePosition);

        bool equidistance = (distanceA == distanceB && distanceB == distanceC);


        //WE HAVE MIDPOINTS!
        //WE HAVE BISECTORS!
        //WE HAVE LINES!
        //WE HAVE INTERSECTION POINTS (POSITION)


        //Radius is wrong....
        float maxDistance = Mathf.Max(distanceA, distanceB, distanceC);
        //float cirlceRadius = Vector3.Distance(vertexPosition, circlePosition);



        return new Geometry.Circle(circlePosition, maxDistance);
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
        Gizmos.DrawSphere(allPoints[0], 0.5f);
        Gizmos.color = Color.red;
        for (int i = 1; i < allPoints.Count; i++)
        {
            Gizmos.DrawSphere(allPoints[i], 0.5f);

        }
        Gizmos.color = Color.green;

        Gizmos.DrawCube(circumCircleList[circleIndex].center, new Vector3(1, 1, 1));
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




    public bool PointInsideCircle(Geometry.Circle circle, Vector3 point)
    {
        float distance = Vector3.Distance(point, circle.center);

        return distance < circle.radius;
    }

    public bool Adjacent(Geometry.Triangle A, Geometry.Triangle B)
    {

        int count = 0;

        bool AA = A.pointA == B.pointA || A.pointA == B.pointB || A.pointA == B.pointC;
        bool AB = A.pointB == B.pointA || A.pointB == B.pointB || A.pointB == B.pointC;
        bool AC = A.pointC == B.pointA || A.pointC == B.pointB || A.pointC == B.pointC;

        if (AA)
        {
            count++;
        }
        if (AB)
        {
            count++;
        }
        if (AC)
        {
            count++;
        }

        return count == 2;
    }

    public float CalculateAngleInRads(Vector3 start, Vector3 pivot, Vector3 end)
    {
        float deltaABx = pivot.x - start.x;
        float deltaBCx = end.x - pivot.x;
        float deltaABy = pivot.z - start.z;
        float deltaBCy = end.z - pivot.z;

        float slopeAB = Mathf.Sqrt(deltaABx * deltaABx + deltaABy * deltaABy);
        float slopeBC = Mathf.Sqrt(deltaBCx * deltaBCx + deltaBCy * deltaBCy);

        float x = deltaABx * deltaBCx + deltaABy * deltaBCy;
        float product = slopeAB * slopeBC;

        float y = Mathf.Acos(x / product);
         
        return  y * Geometry.kRadToDeg;
    }

    public Vector3 FindUncommonPoint(Geometry.Triangle A, Geometry.Triangle B)
    {
        List<Vector3> APoints = new List<Vector3>();
        APoints.Add(A.pointA);
        APoints.Add(A.pointB);
        APoints.Add(A.pointC);

        for (int i = 0; i < 3; i++)
        {
            if (APoints[i] != B.pointA && APoints[i] != B.pointB && APoints[i] != B.pointC)
            {
                return APoints[i];
            }
        }
        throw new System.Exception();
    }

    public List<Vector3> FindPointsFormingAngle(Geometry.Triangle A, Vector3 pivotPoint)
    {
        List<Vector3> pointsInOrder = new List<Vector3>();
        // ensure the list is in start-pivot-end order (put the pivot point in the middle)
        if (A.pointA == pivotPoint)
        {
            pointsInOrder.Add(A.pointB);
            pointsInOrder.Add(A.pointA);
            pointsInOrder.Add(A.pointC);
        }
        else if (A.pointB == pivotPoint)
        {
            pointsInOrder.Add(A.pointA);
            pointsInOrder.Add(A.pointB);
            pointsInOrder.Add(A.pointC);
        }
        else if (A.pointC == pivotPoint)
        {
            pointsInOrder.Add(A.pointA);
            pointsInOrder.Add(A.pointC);
            pointsInOrder.Add(A.pointB);
        }
        return pointsInOrder;
    }

    //Returns the number of points inside the circumcircle if >= 4 Delaunay is not finished yet
    public int DelaunayPass(Geometry.Triangle triangle, Geometry.Circle circle)
    {
        int numPoints = 0;
        List<Vector3> pointsInCircle = new List<Vector3>();
        for (int i = 0; i < allPoints.Count; i++)
        {
            if (PointInsideCircle(circle, allPoints[i]))
            {
                pointsInCircle.Add(allPoints[i]);
            }
        }
        numPoints = pointsInCircle.Count;
        List<Geometry.Triangle> trianglesWithPoint = new List<Geometry.Triangle>();
        for (int pointIndex = 0; pointIndex < pointsInCircle.Count; pointIndex++)
        {
            for (int triangleIndex = 0; triangleIndex < triangles.Count; triangleIndex++)
            {
                if (triangles[triangleIndex].pointA == pointsInCircle[pointIndex] ||
                   triangles[triangleIndex].pointB == pointsInCircle[pointIndex] ||
                   triangles[triangleIndex].pointC == pointsInCircle[pointIndex])
                {
                    trianglesWithPoint.Add(triangles[triangleIndex]);
                }
            }
        }

        for (int i = 0; i < trianglesWithPoint.Count; i++)
        {
            if (Adjacent(triangle, trianglesWithPoint[i]))
            {

                //find the uncommon points

                Vector3[] edgeFlipPoints = new Vector3[4];
                Vector3 uncommonPoint = FindUncommonPoint(triangle, trianglesWithPoint[i]);

                edgeFlipPoints[0] = uncommonPoint;
                List<Vector3> pointsFormingAngle = FindPointsFormingAngle(triangle, uncommonPoint);
                edgeFlipPoints[1] = pointsInCircle[0];
                edgeFlipPoints[2] = pointsInCircle[2];
                float theta1 = CalculateAngleInRads(pointsFormingAngle[0], pointsFormingAngle[1], pointsFormingAngle[2]);

                uncommonPoint = FindUncommonPoint(trianglesWithPoint[i], triangle);
                pointsFormingAngle.Clear();
                pointsFormingAngle = FindPointsFormingAngle(trianglesWithPoint[i], uncommonPoint);
                float theta2 = CalculateAngleInRads(pointsFormingAngle[0], pointsFormingAngle[1], pointsFormingAngle[2]); ;

                //check if the sum of the angles made of their uncommon edges is greater than 180 degrees
                //if angle > 180 flip their edges:
                if (theta1 + theta2 >= 180)
                {
                    //FLIP THAT MUTHAFUCKING EDGE!


                    //Triangles P1P2P3 and P2P4P3 when flipped form new triangles  P1P2P4 and P1P4P3
                    triangles.Add(new Geometry.Triangle(edgeFlipPoints[0], edgeFlipPoints[1], edgeFlipPoints[3]));
                    triangles.Add(new Geometry.Triangle(edgeFlipPoints[0], edgeFlipPoints[3], edgeFlipPoints[2]));
                    triangles.Remove(triangle);
                    triangles.Remove(trianglesWithPoint[i]);

                    circumCircleList.Clear();

                    for (int j = 0; j < triangles.Count; j++)
                    {
                        CalculateCircumcircle(triangles[j]);
                    }

                }

            }
        }
        return numPoints;
    }
    //PSEUDO

    //Go through each triangle and do the following: (triangleA)
    //First, get the circumcircle of the triangleA 
    //Next, get all the points inside the circumcircle
    //For each of these points:
    //Get all the the triangles that include the current point
    //For each of these triangles: (triangleB)
    //Test triangleA and triangleB for adjacency
    //if they are adjacent
    //check if the sum of the angles made of their uncommon edges is greater than 180 degrees
    //if angle > 180 flip their edges:

    //Edge Flip Algorithm
    //Needs 4 points:
    //First point (P1) is the point in triangleA that is uncommon with triangleB
    //Second point(P2) is the first point (traveling clockwise from P1) on the common edge
    //Third point (P3) is the second point (traveling clockwise from P1) on the common edge
    //Fourth point(P4) is the point in triangleB that is unccomon with triangleA


    //Finding these points is fairly trivial (a simple contains function will return true or false)

    //MOST IMPORTANT PART!
    //Triangles P1P2P3 and P2P4P3 when flipped form new triangles  P1P2P4 and P1P4P3


    //alternatively, simply find which point are uncommon, delete both triangles, and create two new triangles from these two points and the third point being 
    //each of the other points no longer part of the common edge.

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
