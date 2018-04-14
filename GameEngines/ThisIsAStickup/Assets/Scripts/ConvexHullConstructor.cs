using UnityEngine;
using System.Collections;
using System.Collections.Generic;

// Initial Conditions
// Sort the list of points
// Find the bottom most point and add it to the stack
// Add p2 to the stack

// Algorithm

//  Step 1 
//  Grab next point(n) in the list,

//  Step 2
//  Make a line with the top 2 points from the stack, and another line with point(n) and the top stack.  If cross product is negative left turn, otherwise right turn 

//  Step 3
//  if left add to stack and move to next point
//  if right pop and repeat step 2

//Do this until the end of the list


public class ConvexHullConstructor : MonoBehaviour
{
    List<Vector3> roomPositions = new List<Vector3>();

    public List<Vector3> allPoints = new List<Vector3>();
    public List<Vector3> convexHullPoints = new List<Vector3>();

    public List<Geometry.Triangle> triangleList;
   
    Vector3 endPoint;

    public bool isGenerated = false;
    bool stopFlag = false;

    // Use this for initialization                                       
    void Start()
    {
        float y = 0.0f;
        for (int index = 0; index < 25; index++)
        {
            float randx = Random.Range(0, 100);
            float randz = Random.Range(0, 100);

            allPoints.Add(new Vector3(randx, y, randz));
        }

        allPoints.Sort((a, b) => a.z.CompareTo(b.z));

        SortByAngle(ref allPoints);
        GenerateConvexHull();

        Triangulator triangulator = Instantiate<Triangulator>(new Triangulator());

        triangulator.allPoints = allPoints;
        triangulator.ConvexHullPoints = convexHullPoints;

        triangulator.innerPoints = allPoints;

        for (int i = 0; i < triangulator.innerPoints.Count; i++)
        {
            if (convexHullPoints.Contains(triangulator.innerPoints[i]))
            {
                triangulator.innerPoints.Remove(triangulator.innerPoints[i]);
            }
        }

        triangulator.RunTriangulator();

        triangleList = triangulator.triangles;
    }

    public void GeneratePoints()
    {
        for (int index = 0; index < allPoints.Count; index++)
        {
            roomPositions.Add(allPoints[index]);
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

    public void DrawConvexHull()
    {
        Color color = Color.blue;
        for (int i = 0; i < convexHullPoints.Count - 1; i++)
        {
            if (i >= 1)
            {
                color = Color.green;
            }
            Debug.DrawLine(convexHullPoints[i], convexHullPoints[i + 1], color);
        }
        Debug.DrawLine(convexHullPoints[convexHullPoints.Count - 1], convexHullPoints[0], Color.red);
    }

    void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        for (int i = 0; i < allPoints.Count; i++)
        {
            Gizmos.DrawSphere(allPoints[i], 2.0f);
        }
    }
    // Update is called once per frame
    void Update()
    {
        //if (!isGenerated)
        //{
        //    GenerateConvexHull(allPoints);
        //}
        DrawConvexHull();
    }



    void FindEndPoint(List<Vector3> points) //Assuming Sorted List, and first point is leftmost
    {
        endPoint = points[0];
    }


    public void GenerateConvexHull()
    {

        // Find the bottom most point and add it to the stack
        // Add p2 to the stack

        // Algorithm

        List<Vector3> convexStack = new List<Vector3>();        
        convexStack.Add(allPoints[0]);
        convexStack.Add(allPoints[1]);


        for (int i = 2; i < allPoints.Count; ++i)
        {
            //  Step 1 
            //  Grab next point(n) in the list,
            Vector3 temp = allPoints[i];
            Vector3 a = convexStack[convexStack.Count - 2];
            Vector3 b = convexStack[convexStack.Count - 1];
            
            float result = LeftRightCheck(a, b, temp);

            if(result > 0)
            {
                convexStack.Add(temp);
            }
            else if (result < 0)
            {
                convexStack.RemoveAt(convexStack.Count - 1);
                --i;
            }
            else
            {
                if(Vector3.SqrMagnitude(b - a) < Vector3.SqrMagnitude(temp - a))
                {
                    convexStack.RemoveAt(convexStack.Count - 1);
                    convexStack.Add(temp);
                }
            }

        }

        //  Step 2
        //  Make a line with the top 2 points from the stack, and another line with point(n) and the top stack.  If cross product is negative left turn, otherwise right turn 

        //  Step 3
        //  if left add to stack and move to next point
        //  if right pop and repeat step 2

        convexHullPoints = convexStack;
        isGenerated = true;
    }


    void SetStopFlagToTrue(List<Vector3> points)
    {
        for (int index = 0; index < points.Count; index++)
        {
            for (int jIndex = 0; jIndex < points.Count; jIndex++)
            {
                if (index != jIndex)
                {
                    if (points[index] == points[jIndex])
                    {
                        stopFlag = true;
                    }
                }

            }
        }
    }

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

    public void SortByAngle(ref List<Vector3> points)
    {
        //Find Angle based on initial point and Z Axis
        var startPoint = points[0];
        AngleComparer comparer = new AngleComparer(startPoint);

        var zAxis = -Vector3.forward;


        points.Sort(1, points.Count - 1, comparer);
    }

}





