using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Geometry : MonoBehaviour {

    
    public static float kPi = 3.14159f;
    public static float kRadToDeg = 180 / kPi;
    public static float kDegToRad = kPi / 180;

    [System.Serializable]
    public enum CellType
    {
        Empty = 0,
        MajorRoom,
        MinorRoom,
        Hallway,

        MAXTYPE

    }

    [System.Serializable]
    public class Coord
    {
        public int x;
        public int y;

        public CellType type;

        public Coord(int _x, int _y)
        {
            x = _x;
            y = _y;
            type = CellType.Empty;
        }
        public Coord(int _x, int _y, CellType _type)
        {
            x = _x;
            y = _y;
            type = _type;
        }
        
        public static bool operator==(Coord a, Coord b)
        {
            return a.x == b.x && a.y == b.y;
        }
        public static bool operator !=(Coord a, Coord b)
        {
            return !(a == b);
        }



    }
    [System.Serializable]
    public class Line
    {
        public Line()
        { }

        public Line(Vector3 pointA, Vector3 pointB)
        {

            start = pointA;
            start.y = 0;
            end = pointB;
            end.y = 0;

            rise = pointA.z - pointB.z;
            run = pointA.x - pointB.x;
            c = (rise * start.x) + (run * end.z);

            float deltaX = pointA.x - pointB.x;
            float deltay = pointA.z - pointB.z;
            length = Mathf.Sqrt((deltaX * deltaX) + (deltay * deltay));
        }

        public Vector3 start;
        public Vector3 end;

        //Indices of the actual rooms
        public int StartIndex;
        public int EndIndex; 

        public float rise;
        public float run;
        public float c;

        public float length;
    }

    [System.Serializable]
    public class Triangle
    {
        public Triangle(Vector3 a, Vector3 b, Vector3 c)
        {
            pointA = a;
            pointB = b;
            pointC = c;

            AB = new Line(pointA, pointB);
            BC = new Line(pointB, pointC);
            CA = new Line(pointC, pointA);

            lines = new Line[4];
            lines[0] = AB;
            lines[1] = BC;
            lines[2] = CA;

            numPointsInCircle = 0;
            circumCircle = new Geometry.Circle();

            circumCircle = CalculateCircumcircle(this);
        }

        public Vector3 pointA;
        public Vector3 pointB;
        public Vector3 pointC;

        public Line[] lines;

        public Line AB;
        public Line BC;
        public Line CA;

        public Geometry.Circle circumCircle;
        public int numPointsInCircle;
    }

    [System.Serializable]
    public class Circle
    {
        public Circle()
        {
            center = Vector3.zero;
            radius = 1.0f;
        }

        public Circle(Vector3 position, float scale)
        {
            center = position;
            radius = scale;
        }

        public Vector3 center;
        public float radius;
    }


    public static Vector3 CalculateMidpoint(Geometry.Line line)
    {
        float x = (line.end.x + line.start.x) / 2.0f;
        float y = 0;
        float z = (line.end.z + line.start.z) / 2.0f;
        Vector3 midpoint = new Vector3(x, y, z);

        return midpoint;
    }
    public static Vector3 FindIntersection(Geometry.Line A, Geometry.Line B)
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
    public static Geometry.Circle CalculateCircumcircle(Geometry.Triangle triangle)
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
   
        Vector3[] intersectionPoints = new Vector3[3];

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
            
        float maxDistance = Mathf.Max(distanceA, distanceB, distanceC);
        return new Geometry.Circle(circlePosition, maxDistance);
    }
    public static bool PointInsideCircle(Geometry.Circle circle, Vector3 point)
    {
        float distance = Vector3.Distance(point, circle.center);

        return distance <= circle.radius;
    }
    public static float CalculateAngleInDegs(Vector3 start, Vector3 pivot, Vector3 end)
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

        return y * Geometry.kRadToDeg;
    }
    public static Vector3 FindUncommonPoint(Geometry.Triangle A, Geometry.Triangle B)
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
    public static List<Vector3> FindPointsFormingAngle(Geometry.Triangle A, Vector3 pivotPoint)
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

    // Use this for initialization
    void Start () {

	}	
	// Update is called once per frame
	void Update () {
		
	}
}
