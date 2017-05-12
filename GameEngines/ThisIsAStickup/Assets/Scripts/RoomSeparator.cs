using UnityEngine;
using System.Collections;

public class RoomSeparator : MonoBehaviour
{


    

    // Use this for initialization
    private void Start()
    {

    }

    // Update is called once per frame
    private void Update()
    {

    }

    //private void OnTriggerStay(Collider other)
    //{
    //    if (GetComponentInChildren<Rigidbody>().velocity == Vector3.zero)
    //    {
    //        separateFlag = true;
    //    }

    //    if (separateFlag)
    //    {
    //        Vector3 offset = (transform.position - other.transform.position) * 0.10f;
    //        offset.y = 0;

    //        transform.position += offset;
    //    }

    //    separateFlag = false;
    //}

    //private void SeparateRooms()
    //{
    //    Debug.Log("Collision Detected");



    //    float xOffset = Random.Range(0.1f, 0.8f);
    //    float zOffset = Random.Range(0.1f, 0.4f);
    //    transform.position = new Vector3(transform.position.x + xOffset, transform.position.y, transform.position.z + zOffset);
    //    //jitter until they are no longer colliding
    //}
}



