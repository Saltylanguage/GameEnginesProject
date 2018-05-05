using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class ColorUpdater : MonoBehaviour
{

    Color color;

    // Use this for initialization
    void Start()
    {
        color = gameObject.GetComponent<MeshRenderer>().material.color;
    }

    // Update is called once per frame
    void Update()
    {

        if (gameObject.GetComponent<GridGenerator>().tileType == Geometry.CellType.MajorRoom)
        { color = Color.blue; }
        if (gameObject.GetComponent<GridGenerator>().tileType == Geometry.CellType.MinorRoom)
        { color = Color.cyan; }
        if (gameObject.GetComponent<GridGenerator>().tileType == Geometry.CellType.Hallway)
        { color = Color.green; }


    }
}
