using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class ConvexHullConstructor : MonoBehaviour
{

    // public RoomGenerator roomGen;
    //List<Vector3> points = new List<Vector3>();

    List<Vector3> convexHullPoints = new List<Vector3>();

    public GameObject cube;
    //List<GameObject> objectList = new List<GameObject>();

    Vector3 pointA;
    Vector3 pointB;
    Vector3 queryPoint;

    Vector3 convexHullPointToBeAdded;

    Vector3 endPoint;

    List<Vector3> testPoints = new List<Vector3>();

    float leftRightResult;

    int pointAIndex = 0;  // index for first point in separating line
    int pointBIndex = 0;  // index for second point in separating line

    int BIndexTemp = 0;  // temporary value to reset index when a convex hull candidate is found.

    float LeftRightCheck(Vector3 pointA, Vector3 pointB, Vector3 queryPoint)
    {
        float ret = 0;

        ret = Mathf.Sign((pointB.x - pointA.x) * (queryPoint.y - pointA.y) - (pointB.y - pointA.y) * (queryPoint.x - pointA.x));

        //Debug.Log("Result: " + ret);

        if (ret > 0)
        {
            Debug.Log("Left");
        }
        else if (ret < 0)
        {
            Debug.Log("Right");
        }
        else if (ret == 0)
        {
            Debug.Log("Same");
        }

        return ret;
    }



    void ConstructConvexHull()
    {
        for (pointAIndex = 0; pointAIndex < testPoints.Count + 1; pointAIndex++)
        {
            
            pointA = testPoints[pointAIndex];   // set the first point to wherever we are in the list of points 
            //(Note: this value will be reset to point B's index when a convex hull candidate is found)
            // In other words, the search will start from the last convex hull point we have found. 


            // because we are looping around the list of points it is possible to arrive at the end of the list and need to keep going
            // in fact it is necessary to arrive back at the starting point.
            //therefore whenever the index is out of range it will need to be reset to search back at the beginning of the list.
            if (pointAIndex + 1 >= testPoints.Count)  
            {
                pointB = testPoints[0];
            }
            else
            {
                pointB = testPoints[pointAIndex + 1];  // general case to setting pointB
            }


            // begin searching through the points to find a convex hull candidate
            for (pointBIndex = 0; pointBIndex < testPoints.Count; pointBIndex++)
            {
                if ((pointAIndex != pointBIndex) && (pointAIndex + 1 != pointBIndex))
                {
                    // the query point is the current point we are going to test to see if it falls on the left or right
                    // of the line separating created by pointA and pointB that separates the world.
                    queryPoint = testPoints[pointBIndex];  
                    Debug.Log("pointA = " + pointAIndex + " pointB = " + (pointAIndex + 1) + " queryPoint = " + pointBIndex);
                    
                    //begin left/right check.
                    leftRightResult = LeftRightCheck(pointA, pointB, queryPoint);
                    if (leftRightResult > 0) // greater than zero represents a point which left of the separation line
                    {
                        //any point that falls on the left is a viable candidate
                        convexHullPointToBeAdded = testPoints[pointBIndex]; // this could be the point to add to the convex hull 
                        pointB = testPoints[pointBIndex]; 
                        // since we have found a candidate, we must now test which points fall to the left of the line from pointA to the candidate
                        //if we find another point that falls to the left of this line, that point becomes our new candidate and we must update
                        // pointB again.
                        BIndexTemp = pointBIndex; // track where this point is in the list to start searching from this point next iteration
                    }
                }
            }

            // ensure that if everything is to the right, we still have a valid point for the convex hull
            if (convexHullPointToBeAdded == new Vector3(0.0f, 0.0f, 0.0f))
            {
                convexHullPointToBeAdded = pointB;
            }

            //By this point we have ensured that we are correctly grabbing the next valid convex hull point.  
            //All other candidates have been compared and disqualified

            pointB = convexHullPointToBeAdded; // set pointB of the new separating line to the newest point added to the convex hull.
            convexHullPoints.Add(convexHullPointToBeAdded); // add the point to the list of convex hull points
            pointAIndex = BIndexTemp - 1; // set the start point for the next search
            pointBIndex = 0; // reset the pointB start point so we are comparing against all points for left/right positioning


            // break condition, once we have the endpoint in the list we know that the convex hull is complete as we have arrived back at
            // the beginning
            if (pointB == endPoint || convexHullPoints.Contains(endPoint)) 
            {
                break;
            }

        }

        //Debug.Log("Number of Convex Hull Points: " + convexHullPoints.Count);
    }

    // Use this for initialization
    void Start()
    {

        testPoints.Add(new Vector3(18, 22, 0));
        testPoints.Add(new Vector3(25, 10, 0));
        testPoints.Add(new Vector3(35, 28, 0));
        testPoints.Add(new Vector3(42, 6, 0));
        testPoints.Add(new Vector3(44, 18, 0));
        testPoints.Add(new Vector3(50, 45, 0));
        testPoints.Add(new Vector3(58, 19, 0));
        testPoints.Add(new Vector3(66, 29, 0));
        testPoints.Add(new Vector3(69, 7, 0));
        testPoints.Add(new Vector3(80, 24, 0));
        //testPoints.Add(new Vector3(0, 92, 0));
        //testPoints.Add(new Vector3(12, 60, 0));
        //testPoints.Add(new Vector3(180, 1, 0));
        //testPoints.Add(new Vector3(52, 14, 0));
        //testPoints.Add(new Vector3(8, 80, 0));
        //testPoints.Add(new Vector3(44, 4, 0));
        //testPoints.Add(new Vector3(39, 39, 0));


        endPoint = testPoints[0];

        Debug.Log("Convex Hull Constructor Reporting in.");
        Debug.Log("Number of Test Points: " + testPoints.Count);

        ConstructConvexHull();

    }

    void DrawConvexHull()
    {
        for (int i = 0; i < convexHullPoints.Count - 1; i++)
        {
            Debug.DrawLine(convexHullPoints[i], convexHullPoints[i + 1], Color.green);
        }
        Debug.DrawLine(convexHullPoints[convexHullPoints.Count - 1], convexHullPoints[0], Color.green);
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
        DrawConvexHull();
    }
}




//void JarvisMarch()
//{
//    endPoint = roomGen.allRooms[0].transform.position;
//    currentPoint = roomGen.allRooms[1].transform.position;

//    for (int i = 0; i < roomGen.allRooms.Count; i++)
//    {
//        points.Add(roomGen.allRooms[i].transform.position);
//    }


//    while (currentPoint != endPoint)
//    {

//    }

