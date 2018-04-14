using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TriangleGenerator : MonoBehaviour
{


    public ConvexHullConstructor convexGen;
    public Triangulator triangulator;
    bool flag = false;

    // Use this for initialization
    void Start()
    {
        convexGen = new ConvexHullConstructor();
        //convexGen.GenerateConvexHull();
      
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            triangulator.RunTriangulator();
        }        
    }
}
