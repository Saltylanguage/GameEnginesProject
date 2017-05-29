using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class ArchitectureGenerator : MonoBehaviour
{

    public RoomGenerator mRoomGenerator;

    public GameObject wallTemplate;
    public GameObject CeilingAndFloorTemplate;

    public float yOffset = 5.0f;
    public bool isGenerated = false;

    int counter = 0;

    // Use this for initialization
    void Start()
    {
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


            //Destroy(mRoomGenerator.allRooms[i]);
            isGenerated = true;

        }
    }

    void Update()
    {

    }

}

// Update is called once per frame


