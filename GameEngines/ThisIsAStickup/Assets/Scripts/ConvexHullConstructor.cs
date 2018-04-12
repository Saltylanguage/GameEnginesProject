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
    List<Vector3> convexHullPoints = new List<Vector3>();

    Vector3 pointA;
    Vector3 pointB;
    Vector3 queryPoint;

    Vector3 convexHullPointToBeAdded;

    Vector3 endPoint;

    List<Vector3> testPoints = new List<Vector3>();

    float leftRightResult;

    int i = 0;
    int j = 0;

    int tempIndex = 0;  // temporary value to reset index when a convex hull candidate is found.

    public bool isGenerated = false;

    bool firstPass = true;

    bool stopFlag = false;


    // Use this for initialization                                       
    void Start()
    {

        //testPoints.Add(new Vector3(1, 0, 8));
        //testPoints.Add(new Vector3(-3, 0, 4));
        //testPoints.Add(new Vector3(1, 0, 1));
        //testPoints.Add(new Vector3(-1, 0, 8));
        //testPoints.Add(new Vector3(-2, 0, 0));
        //testPoints.Add(new Vector3(1, 0, 2));
        //testPoints.Add(new Vector3(-2, 0, 6));
        //testPoints.Add(new Vector3(0, 0, 0));


        //testPoints.Add(new Vector3(69, 7, 0));      // fixed
        //testPoints.Add(new Vector3(0, 92, 0));      // ok
        //testPoints.Add(new Vector3(12, 60, 0));     // ok
        //testPoints.Add(new Vector3(35, 28, 0));     // ok
        //testPoints.Add(new Vector3(42, 6, 0));      // fixed
        //testPoints.Add(new Vector3(44, 18, 0));     // ok
        //testPoints.Add(new Vector3(150, 45, 0));     // ok
        //testPoints.Add(new Vector3(52, 14, 0));     // ok
        //testPoints.Add(new Vector3(0, 0, 0));
        //testPoints.Add(new Vector3(58, 19, 0));     // fixed
        //testPoints.Add(new Vector3(66, 9, 0));     // ok
        //testPoints.Add(new Vector3(78, 78, 0));
        //testPoints.Add(new Vector3(80, 124, 0));     // ok
        //testPoints.Add(new Vector3(25, 10, 0));     // ok
        //testPoints.Add(new Vector3(18, 22, 0));
        //testPoints.Add(new Vector3(8, 10, 0));
        //testPoints.Add(new Vector3(44, 4, 0));
        //testPoints.Add(new Vector3(39, 39, 0));

        float y = 0.0f;
        for (int index = 0; index < 25; index++)
        {
            float randx = Random.Range(0, 100);
            float randz = Random.Range(0, 100);

            testPoints.Add(new Vector3(randx, y, randz));
        }

        testPoints.Sort((a, b) => a.z.CompareTo(b.z));

        SortByAngle(ref testPoints);
        //do not do dis
        //convexHullPoints = testPoints;
        GenerateConvexHull(testPoints);

    }

    public void GeneratePoints(List<Vector3> points)
    {
        for (int index = 0; index < points.Count; index++)
        {
            roomPositions.Add(points[index]);
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
        for (int i = 0; i < testPoints.Count; i++)
        {
            Gizmos.DrawSphere(testPoints[i], 2.0f);
        }
    }
    // Update is called once per frame
    void Update()
    {
        //if (!isGenerated)
        //{
        //    GenerateConvexHull(testPoints);
        //}
        DrawConvexHull();
    }



    void FindEndPoint(List<Vector3> points) //Assuming Sorted List, and first point is leftmost
    {
        endPoint = points[0];
        //for (int index = 0; index < points.Count; index++)
        //{
        //    if (points[index].x < endPoint.x)
        //    {
        //        endPoint = points[index];
        //    }
        //}
    }


    public void GenerateConvexHull(List<Vector3> points)
    {

        // Find the bottom most point and add it to the stack
        // Add p2 to the stack

        // Algorithm

        List<Vector3> convexStack = new List<Vector3>();
        convexStack.Add(points[0]);
        convexStack.Add(points[1]);


        for (int i = 2; i < points.Count; ++i)
        {
            //  Step 1 
            //  Grab next point(n) in the list,
            Vector3 temp = points[i];
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







        //while (!convexHullPoints.Contains(endPoint) || stopFlag)
        //{
        //    if (i == 0)
        //    {
        //        pointA = endPoint;
        //    }
        //    else
        //    {
        //        pointA = points[i];
        //    }
        //    if (i + 1 >= points.Count)
        //    {
        //        pointB = points[0];
        //    }
        //    else
        //    {
        //        pointB = points[i + 1];
        //    }

        //    for (j = 0; j < points.Count; j++)
        //    {
        //        if ((i != j) && (i + 1 != j))
        //        {
        //            queryPoint = points[j];

        //            leftRightResult = LeftRightCheck(pointA, pointB, queryPoint);
        //            if (leftRightResult > 0)
        //            {
        //                convexHullPointToBeAdded = points[j];
        //                pointB = points[j];

        //                tempIndex = j;
        //            }
        //        }

        //    }
        //    i++;
        //    if (convexHullPointToBeAdded == pointA || convexHullPointToBeAdded == new Vector3(0.0f, 0.0f, 0.0f))
        //    {
        //        convexHullPointToBeAdded = pointB;
        //        tempIndex = j;
        //    }
        //    convexHullPoints.Add(convexHullPointToBeAdded);
        //    i = tempIndex;

        //    if (pointB == endPoint || convexHullPoints.Contains(endPoint))
        //    {
        //        break;
        //    }
        //    firstPass = false;
        //}
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

        // Calls CaseInsensitiveComparer.Compare with the parameters reversed.

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






//public void ConstructConvexHull()
//{
//    while (!convexHullPoints.Contains(endPoint))
//    {
//        if (i >= roomPositions.Count)
//        {
//            i = roomPositions.Count - 1;
//        }
//        pointA = roomPositions[i];
//        if (i + 1 >= roomPositions.Count)
//        {
//            pointB = roomPositions[0];
//        }
//        else
//        {
//            pointB = roomPositions[i + 1];
//        }

//        for (j = 0; j < roomPositions.Count; j++)
//        {
//            if ((i != j) && (i + 1 != j))
//            {
//                queryPoint = roomPositions[j];

//                leftRightResult = LeftRightCheck(pointA, pointB, queryPoint);
//                if (leftRightResult > 0)
//                {
//                    convexHullPointToBeAdded = roomPositions[j];
//                    pointB = roomPositions[j];

//                    tempIndex = j;
//                }
//            }

//        }
//        i++;
//        if (convexHullPointToBeAdded == pointA)
//        {
//            convexHullPointToBeAdded = pointB;
//            tempIndex = j;
//        }
//        convexHullPoints.Add(convexHullPointToBeAdded);
//        i = tempIndex;

//        if (pointB == endPoint || convexHullPoints.Contains(endPoint))
//        {
//            isGenerated = true;
//            break;
//        }

//    }
//    isGenerated = true;
//}




