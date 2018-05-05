using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TileSquasher : MonoBehaviour
{


    BoxCollider box;

    // Use this for initialization
    void Start()
    {
        box = GetComponent<BoxCollider>();
    }
    // Update is called once per frame
    void Update()
    {

    }

    void OnTriggerEnter(Collider other)
    {

        RoomGenerator roomGen = FindObjectOfType<RoomGenerator>();
        if (other.tag != "Hallway")
        {
            GameObject temp = other.gameObject;            
            roomGen.roomsToDelete.Remove(temp);
            Destroy(gameObject);
        }

    }
}

