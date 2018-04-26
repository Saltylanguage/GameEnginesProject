using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(GridGenerator))]
public class MapEditor : Editor
{
    //bool stopFlag = false;
    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();
        GridGenerator map = target as GridGenerator;
        //if (!stopFlag)
        //{
            map.CreateGrid();
            //stopFlag = true;
       // }
    }
}

