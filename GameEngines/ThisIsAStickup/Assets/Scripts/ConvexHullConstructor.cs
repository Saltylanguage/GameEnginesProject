using UnityEngine;
using System.Collections;
using System.Collections.Generic;

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

        testPoints.Sort((a, b) => a.x.CompareTo(b.x));


        FindEndPoint(testPoints);
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

        ret = Mathf.Sign((pointB.x - pointA.x) * (queryPoint.z - pointA.z) - (pointB.z - pointA.z) * (queryPoint.x - pointA.x));

        return ret;
    }




    public void DrawConvexHull()
    {
        Color color = Color.blue;
        for (int i = 0; i < convexHullPoints.Count - 1; i++)
        {
            if(i >= 1)
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
        if (!isGenerated)
        {
            GenerateConvexHull(testPoints);
        }
        DrawConvexHull();
    }



    void FindEndPoint(List<Vector3> points)
    {
        endPoint = points[0];
        for (int index = 0; index < points.Count; index++)
        {
            if (points[index].x < endPoint.x)
            {
                endPoint = points[index];
            }
        }
    }


    public void GenerateConvexHull(List<Vector3> points)
    {
        FindEndPoint(points);
        while (!convexHullPoints.Contains(endPoint) || stopFlag)
        {
          
            if (i >= points.Count)
            {
                i = points.Count - 1;
            }
            if (i == 0)
            {
                pointA = endPoint;
            }
            else
            {
                pointA = points[i];
            }
            if (i + 1 >= points.Count)
            {
                pointB = points[0];
            }
            else
            {
                pointB = points[i + 1];
            }

            for (j = 0; j < points.Count; j++)
            {
                if ((i != j) && (i + 1 != j))
                {
                    queryPoint = points[j];

                    leftRightResult = LeftRightCheck(pointA, pointB, queryPoint);
                    if (leftRightResult > 0)
                    {
                        convexHullPointToBeAdded = points[j];
                        pointB = points[j];

                        tempIndex = j;
                    }
                }

            }
            i++;
            if (convexHullPointToBeAdded == pointA || convexHullPointToBeAdded == new Vector3(0.0f, 0.0f, 0.0f))
            {
                convexHullPointToBeAdded = pointB;
                tempIndex = j;
            }
            convexHullPoints.Add(convexHullPointToBeAdded);
            i = tempIndex;

            if (pointB == endPoint || convexHullPoints.Contains(endPoint))
            {              
                break;
            }
            firstPass = false;
        }
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