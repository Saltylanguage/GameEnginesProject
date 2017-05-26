﻿using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class ArchitectureGenerator : MonoBehaviour {

    public RoomGenerator mRoomGenerator;


    //List<Transform> ceilingTransforms = new List<Transform>();
    //List<Transform> floorTransforms = new List<Transform>();
    //List<Transform> northWallTransforms = new List<Transform>();
    //List<Transform> eastWallTransforms = new List<Transform>();
    //List<Transform> southWallTransforms = new List<Transform>();
    //List<Transform> westWallTransforms = new List<Transform>();

    //List<GameObject> ceilings;
    //List<GameObject> floors;
    //List<GameObject> northWalls;
    //List<GameObject> eastWalls;
    //List<GameObject> southWalls;
    //List<GameObject> westWalls;

    public GameObject wallTemplate;
    public GameObject CeilingAndFloorTemplate;

    public float yOffset = 5.0f;
    public bool isGenerated = false;

    // Use this for initialization
    void Start ()
    {
        //ceilings = new List<GameObject>();
        //floors = new List<GameObject>();

        //westWalls = new List<GameObject>();
        //eastWalls = new List<GameObject>();
        //northWalls = new List<GameObject>();
        //southWalls = new List<GameObject>();    

        //for (int i = 0; i < mRoomGenerator.allRooms.Count; i++)
        //{
        //    ceilings[i].transform.position = ceilingTransforms[i].position;
        //    ceilings[i].transform.localScale = ceilingTransforms[i].localScale;
        //    ceilings[i] = Instantiate(CeilingAndFloorTemplate);


        //    floors[i] = Instantiate(CeilingAndFloorTemplate);
        //    floors[i].transform.position = floorTransforms[i].position;
        //    floors[i].transform.localScale = floorTransforms[i].localScale;            

        //    westWalls[i].transform.position =   westWallTransforms[i].position;
        //    westWalls[i].transform.localScale = westWallTransforms[i].localScale;
        //    westWalls[i] = Instantiate(wallTemplate);

        //    eastWalls[i].transform.position =   eastWallTransforms[i].position;
        //    eastWalls[i].transform.localScale = eastWallTransforms[i].localScale;
        //    eastWalls[i] = Instantiate(wallTemplate);

        //    northWalls[i].transform.position =   northWallTransforms[i].position;
        //    northWalls[i].transform.localScale = northWallTransforms[i].localScale;
        //    northWalls[i] = Instantiate(wallTemplate);

        //    southWalls[i].transform.position =   southWallTransforms[i].position;
        //    southWalls[i].transform.localScale = southWallTransforms[i].localScale;
        //    southWalls[i] = Instantiate(wallTemplate);
        //}

	}
	
    public void GenerateArchitecture()
    {             
            for (int i = 0; i < mRoomGenerator.allRooms.Count; i++)
            {

                //CEILINGS
                GameObject temp = Instantiate(CeilingAndFloorTemplate);
                temp.transform.position = new Vector3(mRoomGenerator.allRooms[i].gameObject.transform.position.x, mRoomGenerator.allRooms[i].gameObject.transform.position.y + yOffset, mRoomGenerator.allRooms[i].gameObject.transform.position.z);
                temp.transform.localScale = new Vector3(mRoomGenerator.allRooms[i].gameObject.transform.localScale.x, 0.1f, mRoomGenerator.allRooms[i].gameObject.transform.localScale.z);
                

                //FLOORS
                temp = Instantiate(CeilingAndFloorTemplate);
                temp.transform.position = new Vector3(mRoomGenerator.allRooms[i].gameObject.transform.position.x, mRoomGenerator.allRooms[i].gameObject.transform.position.y - yOffset, mRoomGenerator.allRooms[i].gameObject.transform.position.z);
                temp.transform.localScale = new Vector3(mRoomGenerator.allRooms[i].gameObject.transform.localScale.x, 0.1f, mRoomGenerator.allRooms[i].gameObject.transform.localScale.z);
                

                //WEST WALLS
                temp = Instantiate(wallTemplate);
                temp.transform.position = new Vector3(mRoomGenerator.allRooms[i].gameObject.transform.position.x - (mRoomGenerator.allRooms[i].gameObject.transform.localScale.x / 2.0f),
                                            mRoomGenerator.allRooms[i].gameObject.transform.position.y,
                                            mRoomGenerator.allRooms[i].gameObject.transform.position.z);
                temp.transform.localScale = new Vector3(0.1f, mRoomGenerator.allRooms[i].transform.localScale.y, mRoomGenerator.allRooms[i].transform.localScale.z);
                


                //EAST WALLS
                temp = Instantiate(wallTemplate);
                temp.transform.position = new Vector3(mRoomGenerator.allRooms[i].gameObject.transform.position.x + (mRoomGenerator.allRooms[i].gameObject.transform.localScale.x / 2.0f),
                                            mRoomGenerator.allRooms[i].gameObject.transform.position.y,
                                            mRoomGenerator.allRooms[i].gameObject.transform.position.z);
                temp.transform.localScale = new Vector3(0.1f, mRoomGenerator.allRooms[i].transform.localScale.y, mRoomGenerator.allRooms[i].transform.localScale.z);
            


                //NORTH WALLS
                temp = Instantiate(wallTemplate);
                temp.transform.localScale = new Vector3(0.1f, mRoomGenerator.allRooms[i].transform.localScale.y, mRoomGenerator.allRooms[i].transform.localScale.x);
                temp.transform.rotation = Quaternion.Euler(mRoomGenerator.allRooms[i].transform.rotation.x, mRoomGenerator.allRooms[i].transform.rotation.y + 90, mRoomGenerator.allRooms[i].transform.rotation.z);
                temp.transform.position = new Vector3(mRoomGenerator.allRooms[i].gameObject.transform.position.x,
                                            mRoomGenerator.allRooms[i].gameObject.transform.position.y,
                                            mRoomGenerator.allRooms[i].gameObject.transform.position.z + (mRoomGenerator.allRooms[i].transform.localScale.z / 2.0f));
                
                //SOUTH WALLS
                temp = Instantiate(wallTemplate);
                temp.transform.localScale = new Vector3(0.1f, mRoomGenerator.allRooms[i].transform.localScale.y, mRoomGenerator.allRooms[i].transform.localScale.x);
                temp.transform.rotation = Quaternion.Euler(mRoomGenerator.allRooms[i].transform.rotation.x, mRoomGenerator.allRooms[i].transform.rotation.y + 90, mRoomGenerator.allRooms[i].transform.rotation.z);
                temp.transform.position = new Vector3(mRoomGenerator.allRooms[i].gameObject.transform.position.x,
                                            mRoomGenerator.allRooms[i].gameObject.transform.position.y,
                                            mRoomGenerator.allRooms[i].gameObject.transform.position.z - (mRoomGenerator.allRooms[i].transform.localScale.z / 2.0f));
            

            Destroy(mRoomGenerator.allRooms[i]);
            isGenerated = true;

            }
        }

    void Update()
    {

    }

}

    // Update is called once per frame

