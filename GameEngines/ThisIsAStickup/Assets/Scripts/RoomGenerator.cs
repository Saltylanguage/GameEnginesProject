﻿using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class RoomGenerator : MonoBehaviour
{
    public static float radius = 100;
    public int numObjects = 25;

    public float xThreshold;
    public float yThreshold;

    public static List<Vector3> randomPoints = new List<Vector3>();
    public static List<float> xRange = new List<float>();
    public static List<float> zRange = new List<float>();

    public GameObject roomTemplate;
    public List<GameObject> allRooms;

    public List<GameObject> mainRooms;
    public List<GameObject> roomsToDelete;

    public List<Vector3> roomPositions;


    public bool separateFlag = false;
    public bool isDone = false;
    public bool done = false;

    ArchitectureGenerator archGen;    
    ConvexHullConstructor convexGen;

    public static Vector3 GetRandomPoint()
    {
        Vector3 ret = new Vector3();

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

        archGen = GetComponent<ArchitectureGenerator>();
        convexGen = GetComponent<ConvexHullConstructor>();
        allRooms = new List<GameObject>(new GameObject[numObjects]);
        CreateRandomPoints(numObjects);
        float xSum = 0;
        float zSum = 0;
        for (int i = 0; i < numObjects; i++)
        {
            allRooms[i] = Instantiate(roomTemplate);
            

            allRooms[i].transform.localScale = new Vector3(xRange[i], 10, zRange[i]);
            allRooms[i].transform.position = new Vector3(randomPoints[i].x, 10, randomPoints[i].z);

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
    }

    // Use this for initialization
    void Start()
    {


    }

    void Update()
    {
        BoxCollider temp;

        done = true;
        for (int i = 0; i < allRooms.Count; i++)
        {
            temp = allRooms[i].GetComponentInChildren<BoxCollider>();
            if(temp)
            {
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
                            separateFlag = true;
                        }

                        if (separateFlag)
                        {
                            float randomXOffset = Random.Range(1, 5);

                            float randomZOffset = Random.Range(1, 5);

                            if (allRooms[i].transform.position.x <= allRooms[j].transform.position.x)
                            {
                                randomXOffset *= -1;
                            }
                            if (allRooms[i].transform.position.z <= allRooms[j].transform.position.z)
                            {
                                randomZOffset *= -1;
                            }

                            allRooms[i].transform.position += new Vector3(randomXOffset, 0, randomZOffset);
                            done = false;
                        }

                        separateFlag = false;
                    }
                }
            }
        }

        if (done && !convexGen.isGenerated)
        {

            //SetRoomPositions();
            //convexGen.GeneratePoints(roomPositions);
            //convexGen.GenerateConvexHull(roomPositions);
            //archGen.GenerateArchitecture();
            //Destroy(this.gameObject);
        }

        //convexGen.DrawConvexHull();

    }

    void SetRoomPositions()
    {
        for(int i = 0; i < allRooms.Count; i++)
        {
            roomPositions.Add(allRooms[i].transform.position);
        }
    }


}





