using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Geometry : MonoBehaviour {


    public struct Line
    {
        public Line(Vector3 pointA, Vector3 pointB)
        {
            a = pointA.z - pointB.z;
            b = pointA.x - pointB.x;
            c = (a * pointA.x) + (b * pointB.z);

            start = pointA;
            end = pointB;
        }

        public Vector3 start;
        public Vector3 end;

        public float a;
        public float b;
        public float c;
    }

    public class Triangle
    {
        public Triangle(Vector3 a, Vector3 b, Vector3 c)
        {
            pointA = a;
            pointB = b;
            pointC = c;
            CalculateLines();

            minXY.x = Mathf.Min(Mathf.Min(pointA.x, pointB.x), pointC.x);
            minXY.y = Mathf.Min(Mathf.Min(pointA.z, pointB.z), pointC.z);
        }

        public Vector3 pointA;
        public Vector3 pointB;
        public Vector3 pointC;

        public Line[] lines = new Line[3];

        public Line AB;
        public Line BC;
        public Line CA;

        public Vector2 minXY;
        public Vector2 maxXY;

        public void CalculateLines()
        {
            AB = new Line(pointA, pointB);
            BC = new Line(pointB, pointC);
            CA = new Line(pointC, pointA);

            lines[0] = AB;
            lines[1] = BC;
            lines[2] = CA;
        }
    }

    // Use this for initialization
    void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}
}
