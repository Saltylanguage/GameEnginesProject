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

        Destroy(gameObject);

    }
}

