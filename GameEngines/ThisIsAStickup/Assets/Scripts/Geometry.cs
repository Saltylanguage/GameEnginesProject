using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Geometry : MonoBehaviour {


    public class Line
    {
        public Line()
        { }

        public Line(Vector3 pointA, Vector3 pointB)
        {

            start = pointA;
            end = pointB;

            rise = pointA.z - pointB.z;
            run = pointA.x - pointB.x;
            c = (rise * start.x) + (run * end.z);
        }

        public Vector3 start;
        public Vector3 end;

        public float rise;
        public float run;
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

   



    // Use this for initialization
    void Start () {

	}
	
	// Update is called once per frame
	void Update () {
		
	}
}
