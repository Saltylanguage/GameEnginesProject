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

    public RoomGenerator roomGen;

    public List<Vector3> allPoints = new List<Vector3>();
    public List<Vector3> convexHullPoints = new List<Vector3>();
    public List<Vector3> innerPoints = new List<Vector3>();

    public List<Geometry.Triangle> triangles = new List<Geometry.Triangle>();

    public List<Geometry.Line> minimumSpanningTree = new List<Geometry.Line>();
    public List<Geometry.Line> hallways = new List<Geometry.Line>();

    public int NumberOfPoints = 100;


    List<Vector2> indexList = new List<Vector2>();

    public bool mstMode = false;
    public bool triangleMode = false;
    public bool convexMode = false;
    public bool hallwayMode = false;


    //UNITY FUNCTIONS    
    void Start()
    {

        //roomGen = gameObject.AddComponent(typeof(RoomGenerator)) as RoomGenerator;
        //mstMode = false;
        //allPoints.Clear();
        //convexHullPoints.Clear();
        //innerPoints.Clear();
        //triangles.Clear();

        //GeneratePoints();
        //Sort(ref allPoints);
        //convexHullPoints = GenerateConvexHull(allPoints);
        //GetInnerPoints();
        //Triangulate();

        //int maxPoints = 0;

        //for (int i = 0; i < 10; i++)
        //{
        //    maxPoints = RunPassOnTriangles(triangles.Count, maxPoints);
        //}
        //roomGen = GetComponent<RoomGenerator>();

        //for (int i = 0; i < roomGen.roomPositions.Count; i++)
        //{
        //    allPoints.Add(new Vector3(roomGen.roomPositions[i].x, roomGen.roomPositions[i].y, roomGen.roomPositions[i].z));
        //}
        ////GenerateTestPoints();
        ////GeneratePoints();
        //Sort(ref allPoints);
        //convexHullPoints = GenerateConvexHull(allPoints);
        //GetInnerPoints();
        //Triangulate();
    }
    void Update()
    {
        if (mstMode)
        {
            DrawLines(minimumSpanningTree, Color.red);
        }
        else if (triangleMode)
        {
            DrawTriangles();
        }
        else if (convexMode)
        {
            DrawConvexHull();
        }
        else if (hallwayMode)
        {
            DrawHallways();
        }
    }

    //POINT GENERATION
    public void GeneratePoints()
    {
        float y = 20.0f;
        for (int index = 0; index < NumberOfPoints; index++)
        {
            float randx = Random.Range(0, 100);
            float randz = Random.Range(0, 100);

            allPoints.Add(new Vector3(randx, y, randz));
        }
    }
    public void GenerateTestPoints()
    {
        //NEW TEST CONDITION
        allPoints.Add(new Vector3(0, 0, 0));
        allPoints.Add(new Vector3(-6, 0, 8));
        allPoints.Add(new Vector3(-8, 0, 3));

        allPoints.Add(new Vector3(-6, 0, 4));
        allPoints.Add(new Vector3(-2, 0, 7));
        allPoints.Add(new Vector3(-1, 0, 4));

        allPoints.Add(new Vector3(1, 0, 4));
        allPoints.Add(new Vector3(0, 0, 9));
        allPoints.Add(new Vector3(4, 0, 5));
    }

    //POINT SORTING
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

    //DRAW FUNCTIONS
    public void DrawConvexHull()
    {
        Color color = Color.magenta;
        for (int i = 0; i < convexHullPoints.Count - 1; i++)
        {
            if (i >= 1)
            {
                color = Color.magenta;
            }
            Debug.DrawLine(convexHullPoints[i], convexHullPoints[i + 1], color);
        }
        Debug.DrawLine(convexHullPoints[convexHullPoints.Count - 1], convexHullPoints[0], Color.magenta);
    }
    public void DrawTriangles()
    {
        Color color = Color.blue;
        for (int i = 0; i < triangles.Count; i++)
        {
            Debug.DrawLine(triangles[i].pointA, triangles[i].pointB, color);
            Debug.DrawLine(triangles[i].pointB, triangles[i].pointC, color);
            Debug.DrawLine(triangles[i].pointC, triangles[i].pointA, color);
        }
    }
    public void DrawHallways()
    {
        Color color = Color.blue;
        for(int i = 0; i < hallways.Count; i++)
        {
            Debug.DrawLine(hallways[i].start, hallways[i].end, color);
        }
    }
    public void DrawLines(List<Geometry.Line> lines, Color color)
    {
        for (int i = 0; i < lines.Count; i++)
        {
            Debug.DrawLine(lines[i].start, lines[i].end, color);
        }
    }
    void OnDrawGizmos()
    {
        if (allPoints != null && allPoints.Count > 0)
        {
            Gizmos.color = Color.blue;
            Gizmos.DrawSphere(allPoints[0], 0.5f);
            Gizmos.color = Color.red;
            for (int i = 1; i < allPoints.Count; i++)
            {
                Gizmos.DrawSphere(allPoints[i], 0.3f);

            }
            Gizmos.color = Color.green;
        }
    }


    //CONVEX HULL GENERATION & HELPER FUNCTIONS
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

    //TRIANGULATION & HELPER FUNCTIONS
    public void Triangulate()
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
    public int RunPassOnTriangles(int triCount, int pointCount)
    {
        for (int i = 0; i < triCount; i++)
        {
            pointCount = Mathf.Max(DelaunayPass(triangles[i]), pointCount);
        }
        Debug.Log("POINT COUNT = " + pointCount);
        return pointCount;
    }
    public int DelaunayPass(Geometry.Triangle triangle)
    {
        int numPoints = 0;
        List<Vector3> pointsInCircle = new List<Vector3>();

        for (int index = 0; index < allPoints.Count; index++)
        {
            if (Geometry.PointInsideCircle(triangle.circumCircle, allPoints[index]))
            {
                pointsInCircle.Add(allPoints[index]);
                triangle.numPointsInCircle++;
            }
        }
        numPoints = pointsInCircle.Count;

        if (numPoints >= 4)
        {
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
                for (int k = 0; k < trianglesWithPoint.Count; k++)
                {
                    if (Adjacent(triangle, trianglesWithPoint[k]))
                    {
                        //find the uncommon points
                        Vector3[] edgeFlipPoints = new Vector3[4];
                        Vector3 uncommonPoint = Geometry.FindUncommonPoint(triangle, trianglesWithPoint[k]);

                        edgeFlipPoints[0] = uncommonPoint;
                        List<Vector3> pointsFormingAngle = Geometry.FindPointsFormingAngle(triangle, uncommonPoint);
                        edgeFlipPoints[1] = pointsFormingAngle[0];
                        edgeFlipPoints[2] = pointsFormingAngle[2];
                        float theta1 = Mathf.Ceil(180 - Geometry.CalculateAngleInDegs(pointsFormingAngle[0], pointsFormingAngle[1], pointsFormingAngle[2]));

                        uncommonPoint = Geometry.FindUncommonPoint(trianglesWithPoint[k], triangle);
                        edgeFlipPoints[3] = uncommonPoint;
                        pointsFormingAngle.Clear();
                        pointsFormingAngle = Geometry.FindPointsFormingAngle(trianglesWithPoint[k], uncommonPoint);

                        float theta2 = Mathf.Ceil(180 - Geometry.CalculateAngleInDegs(pointsFormingAngle[0], pointsFormingAngle[1], pointsFormingAngle[2]));

                        //check if the sum of the angles made of their uncommon edges is greater than 180 degrees
                        //if angle > 180 flip their edges:
                        if (theta1 + theta2 >= 180)
                        {
                            //Mark these two triangles for flipping
                            Vector2 indexPair = new Vector2(FindTriangleIndex(triangle), FindTriangleIndex(trianglesWithPoint[k]));


                            //Triangles P1P2P3 and P2P4P3 when flipped form new triangles  P1P2P4 and P1P4P3
                            int removeIndex = FindTriangleIndex(triangle);
                            triangles.RemoveAt(removeIndex);
                            removeIndex = FindTriangleIndex(trianglesWithPoint[k]);
                            triangles.RemoveAt(removeIndex);

                            Geometry.Triangle temp1 = new Geometry.Triangle(edgeFlipPoints[0], edgeFlipPoints[1], edgeFlipPoints[3]);
                            Geometry.Triangle temp2 = new Geometry.Triangle(edgeFlipPoints[0], edgeFlipPoints[3], edgeFlipPoints[2]);

                            for (int i = 0; i < allPoints.Count; i++)
                            {
                                if (Geometry.PointInsideCircle(temp1.circumCircle, allPoints[i]))
                                {
                                    temp1.numPointsInCircle++;
                                }
                                if (Geometry.PointInsideCircle(temp2.circumCircle, allPoints[i]))
                                {
                                    temp2.numPointsInCircle++;
                                }
                            }

                            triangles.Add(temp1);
                            triangles.Add(temp2);

                            return numPoints;
                        }
                    }
                }
                trianglesWithPoint.Clear();
            }
        }



        return numPoints;
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
    public int FindTriangleIndex(Geometry.Triangle A)
    {
        for (int i = 0; i < triangles.Count; i++)
        {
            if (A.pointA == triangles[i].pointA && A.pointB == triangles[i].pointB && A.pointC == triangles[i].pointC)
            {
                return i;
            }
        }
        return -1;
    }

    // MINIMUM SPANNING TREE & HELPER FUNCTIONS
    public void CalculateMinimumSpanningTree(List<Geometry.Triangle> triangulation)
    {
        //Calculating Euclidean Minimum Spanning Tree(EMST)


        // 1 - first get a list of all edges in the triangulation
        List<Geometry.Line> listOfAllLines = new List<Geometry.Line>();
        for (int i = 0; i < triangulation.Count; i++)
        {
            if (!ListContainsLine(listOfAllLines, triangulation[i].AB))
            {
                listOfAllLines.Add(triangulation[i].AB);
            }
            if (!ListContainsLine(listOfAllLines, triangulation[i].BC))
            {
                listOfAllLines.Add(triangulation[i].BC);
            }
            if (!ListContainsLine(listOfAllLines, triangulation[i].CA))
            {
                listOfAllLines.Add(triangulation[i].CA);
            }
        }

        // 2 - sort this list by length
        listOfAllLines.Sort((a, b) => a.length.CompareTo(b.length));

        // 3 - Creat a list of points to store all points that have already been visited
        List<Vector3> visitedPoints = new List<Vector3>();

        // 4 - Add the shortest Edge's points to a list of points
        visitedPoints.Add(listOfAllLines[0].start);
        visitedPoints.Add(listOfAllLines[0].end);

        minimumSpanningTree.Add(listOfAllLines[0]);

        // 5 - Remove this edge from the list of edges
        listOfAllLines.RemoveAt(0);

        int pointCount = 0;
        bool startIncluded = false;
        bool endIncluded = false;

        // 6 - for every remaining edge:
        for (int i = 0; i < listOfAllLines.Count; i++)
        {
            //     a) check if this edge contains EXACTLY 1 point in the list of points
            //       *LOGIC: if it contains 1 and only 1 point this means it attaches to our list of edges without being
            //        redundant and because we sorted by length the first one we visit will automatically be the shortest


            //we are NOT guaranteeing that the edge does not close a circuit.
            for (int j = 0; j < visitedPoints.Count; j++)
            {
                startIncluded = false;
                endIncluded = false;
                pointCount = 0;
                if (listOfAllLines[i].start == visitedPoints[j])
                {
                    startIncluded = true;
                    pointCount++;
                }
                if (listOfAllLines[i].end == visitedPoints[j])
                {
                    endIncluded = true;
                    pointCount++;
                }

                if (pointCount == 1)
                {
                    if (!(visitedPoints.Contains(listOfAllLines[i].start) && visitedPoints.Contains(listOfAllLines[i].end)))
                    {
                        if (startIncluded)
                        {
                            visitedPoints.Add(listOfAllLines[i].end);
                        }
                        else if (endIncluded)
                        {
                            visitedPoints.Add(listOfAllLines[i].start);
                        }

                        minimumSpanningTree.Add(listOfAllLines[i]);
                        listOfAllLines.RemoveAt(i);
                        i = 0;
                        if (visitedPoints.Count == allPoints.Count)
                        {
                            break;
                        }
                    }
                }
            }
            if (visitedPoints.Count == allPoints.Count)
            {
                break;
            }
        }
        /*                        
         *     b) Once you find the first edge with exactly 1 point in the list of points: 
         *     Add this edge to list of edges and add the point from this Edge that is not already in the list of points and remove the Edge from the list of edges.
         *     
         *    
         * 7 - Repeat Step 6 until the List of points contains all points.
         */
    }
    public bool ListContainsLine(List<Geometry.Line> list, Geometry.Line line)
    {
        for (int i = 0; i < list.Count; i++)
        {
            if (line.start == list[i].start || line.end == list[i].start)
            {
                if (line.start == list[i].end || line.end == list[i].end)
                {
                    return true;
                }
            }
        }
        return false;
    }

    public void MakeStraightLines()
    {
        hallways.Clear();
        for (int i = 0; i < minimumSpanningTree.Count; i++)
        {
            //3 cases:         
            //Within x range - straight vertical line will connect both rooms
            //Within y range - straight horizontal line will connect rooms
            //Diagonal(not within either range) - An L shaped line is needed to connect both rooms.

            //How to tell if they are within range.
            Vector2 positionA = new Vector2(minimumSpanningTree[i].start.x, minimumSpanningTree[i].start.z);
            Vector2 positionB = new Vector2(minimumSpanningTree[i].end.x, minimumSpanningTree[i].end.z);

            //First find the midpoint between RoomA and RoomB
            Vector2 midPoint = new Vector2((positionA.x + positionB.x) / 2.0f, (positionA.y + positionB.y) / 2.0f);
            Vector2 gridSize = new Vector2(5, 5);

            if (midPoint.x + (gridSize.x / 2) >= positionA.x && positionA.x >= midPoint.x - (gridSize.x / 2))
            {
                if (midPoint.x + (gridSize.x / 2) >= positionB.x && positionB.x >= midPoint.x - (gridSize.x / 2))
                {
                    // if both checks return true a straight vertical line passing through the midpoint will connect both rooms
                    // draw a line from Vector2(midPoint.x,roomA.y) to Vector2(midPoint.x, roomB.y)
                    Vector3 a = new Vector3(midPoint.x, 10, positionA.y);
                    Vector3 b = new Vector3(midPoint.x, 10, positionB.y);
                    hallways.Add(new Geometry.Line(a, b));
                }
            }

            //if either check returns false: repeat the process to check along the y axis
            else if (midPoint.y + (gridSize.y / 2) >= positionA.y && positionA.y >= midPoint.y - (gridSize.y / 2))
            {

                if (midPoint.y + (gridSize.y / 2) >= positionB.y && positionB.y >= midPoint.y - (gridSize.y / 2))
                {
                    //if both y checks return true a straight horizontal line passing through the midpoint will connect both rooms.
                    Vector3 a = new Vector3(midPoint.x, 10, positionA.y);
                    Vector3 b = new Vector3(midPoint.x, 10, positionB.y);
                    hallways.Add(new Geometry.Line(a, b));
                }
            }

            //If we get here, it means the two rooms cannot be connected with a single non-diagonal line

            //We need to draw an L shaped line from the midpoints of the two rooms, but which way first?
            //Up then right != Right then Up
            //Rule: Travel along the longer line first.            

            //That means: 
            else
            {
                float rise = positionB.y - positionA.y;
                float run = positionB.x - positionA.x;

                if (rise > run)
                {
                    Vector3 temp = new Vector3(positionA.x, 10, positionB.y);
                    hallways.Add(new Geometry.Line(positionA, temp));
                    hallways.Add(new Geometry.Line(temp, positionB));
                }
                else if (run > rise)
                {
                    Vector3 temp = new Vector3(positionB.x, positionA.y);
                    hallways.Add(new Geometry.Line(positionA, temp));
                    hallways.Add(new Geometry.Line(temp, positionB));
                }

            }
            //If rise > run 
            // travel in the y direction first (either top or bottom edge depending on signage)
            //Draw a line from roomA to Vector2(roomA.xPos, roomB.yPos). Call this line temp
            //Next Draw a line from temp to roomB

            //If run > rise
            // travel in the x direction first(either left or right edge depending on signage)
            //Draw a line from roomA to Vector2(roomB.xPos, roomA.yPos). Call this line temp
            //Next Draw a line from Vector2(roomB.xPos, roomA.yPos) to roomB

            //Do this for every edge and you will have straight, non-diagonal lines connecting every room.
        }


     
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



