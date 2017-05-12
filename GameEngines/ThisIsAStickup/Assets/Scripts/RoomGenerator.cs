using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class RoomGenerator : MonoBehaviour
{



    public static float radius = 100;
    public int numObjects = 25;

    public float xThreshold;
    public float yThreshold;

    public static List<Vector2> randomPoints = new List<Vector2>();
    public static List<float> xRange = new List<float>();
    public static List<float> zRange = new List<float>();

    public GameObject roomTemplate;
    public List<GameObject> allRooms;

    public List<GameObject> mainRooms;
    public List<GameObject> roomsToDelete;

    bool separateFlag = false;


    public static Vector2 GetRandomPoint()
    {
        Vector2 ret = new Vector2();

        ret = Random.insideUnitCircle;
        ret *= radius;

        return ret;
    }

    public static void CreateRandomPoints(int count)
    {
        for (int i = 0; i < count; i++)
        {
            randomPoints.Add(GetRandomPoint());
            xRange.Add(Random.Range(10, 50));
            zRange.Add(Random.Range(10, 50));
        }
    }

    void Awake()
    {
        allRooms = new List<GameObject>(new GameObject[numObjects]);
        CreateRandomPoints(numObjects);
        float xSum = 0;
        float zSum = 0;
        for (int i = 0; i < numObjects; i++)
        {
            allRooms[i] = Instantiate(roomTemplate);

            allRooms[i].transform.localScale = new Vector3(xRange[i], 10, zRange[i]);
            allRooms[i].transform.position = new Vector3(randomPoints[i].x, 10, randomPoints[i].y);

            xSum += allRooms[i].transform.localScale.x;
            zSum += allRooms[i].transform.localScale.z;

        }

        float xMean = (xSum / numObjects) * 0.75f;
        float zMean = (zSum / numObjects) * 0.75f;

        for (int i = 0; i < numObjects; i++)
        {
            if (allRooms[i].transform.localScale.x >= xMean && allRooms[i].transform.localScale.z >= zMean)
            {
                mainRooms.Add(allRooms[i]);
            }
            else
            {
                roomsToDelete.Add(allRooms[i]);
            }
        }

        for (int i = 0; i < mainRooms.Count; i++)
        {
            mainRooms[i].GetComponentInChildren<MeshRenderer>().material.color = Color.red;
            mainRooms[i].transform.position += new Vector3(0, 1f, 0);
        }

        for (int i = 0; i < roomsToDelete.Count; i++)
        {
            //Destroy(roomsToDelete[i]);
        }
    }

    // Use this for initialization
    void Start()
    {


    }

    void Update()
    {
        BoxCollider temp;


        for (int i = 0; i < allRooms.Count; i++)
        {
            temp = allRooms[i].GetComponentInChildren<BoxCollider>();
            for (int j = 0; j < allRooms.Count; j++)
            {
                if (i == j)
                {
                    continue;
                }
                if (temp.bounds.Intersects(allRooms[j].GetComponentInChildren<BoxCollider>().bounds))
                {
                    if (allRooms[i].GetComponentInChildren<Rigidbody>().velocity == Vector3.zero)
                    {
                        Debug.Log("Intersecting when Velocity is zero.");
                        separateFlag = true;
                    }

                    if (separateFlag)
                    {
                        Debug.Log("X: " + allRooms[i].transform.position.x + "Y: " + allRooms[i].transform.position.y + "Z: " + allRooms[i].transform.position.z);

                        float randomXOffset = Random.Range(1, 5);

                        float randomZOffset = Random.Range(1, 5);

                        if(allRooms[i].transform.position.x <= allRooms[j].transform.position.x)
                        {
                            randomXOffset *= -1;                            
                        }
                        if (allRooms[i].transform.position.z <= allRooms[j].transform.position.z)
                        {
                            randomZOffset *= -1;
                        }

                        allRooms[i].transform.position += new Vector3(randomXOffset, 0, randomZOffset);
                    }

                    separateFlag = false;
                }
            }
        }
    }


}





