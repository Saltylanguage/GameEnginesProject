using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Geometry : MonoBehaviour {


    public struct Line
    {
        public Line(Vector2 pointA, Vector2 pointB)
        {
            a = pointA.y - pointB.y;
            b = pointA.x - pointB.x;
            c = a * pointA.x + b * pointB.y;

            start = pointA;
            end = pointB;
        }

        public Vector2 start;
        public Vector2 end;

        public float a;
        public float b;
        public float c;
    }

    public class Triangle
    {
        public Triangle(Vector2 a, Vector2 b, Vector2 c)
        {
            pointA = a;
            pointB = b;
            pointC = c;
            CalculateLines();

            minXY.x = Mathf.Min(Mathf.Min(pointA.x, pointB.x), pointC.x);
            minXY.y = Mathf.Min(Mathf.Min(pointA.y, pointB.y), pointC.y);
        }

        public Vector2 pointA;
        public Vector2 pointB;
        public Vector2 pointC;

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
        }
    }

    // Use this for initialization
    void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}
}
