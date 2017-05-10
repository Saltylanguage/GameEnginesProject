﻿using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class RoomGenerator : MonoBehaviour
{


    public static float radius = 100;
    public int numObjects = 100;

    public float xThreshold;
    public float yThreshold;

    

    public static List<Vector2> randomPoints = new List<Vector2>();
    public static List<float> xRange = new List<float>();
    public static List<float> zRange = new List<float>();


    public GameObject roomTemplate;
    public List<GameObject> allRooms = new List<GameObject>();

    public List<GameObject> mainRooms;


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

        float xMean = (xSum / numObjects) * 1.25f ;
        float zMean = (zSum / numObjects) * 1.25f;

        for(int i = 0; i < numObjects; i++)
        {
            if (allRooms[i].transform.localScale.x >= xMean || allRooms[i].transform.localScale.z >= zMean)
            {
                mainRooms.Add(allRooms[i]);
            }
        }

        for(int i = 0; i < mainRooms.Count; i++)
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
        for (int i = 0; i < numObjects; i++)
        {
            Rigidbody temp = allRooms[i].GetComponentInChildren<Rigidbody>();
            temp.velocity = Vector3.zero;
        
            //if (temp.velocity.sqrMagnitude >=100)
            //{
            //    temp.AddForce(-temp.velocity * 0.999f);
            //}
            //else
            //{

            //}
        }
    }


}
