using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(RoomGenerator))]
public class RoomGenEditor : Editor
{
    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();
        RoomGenerator roomGen = (RoomGenerator)target;

        if (GUILayout.Button("Snap To Grid"))
        {
            GameObject[] objects = GameObject.FindGameObjectsWithTag("Grid");

            GridGenerator[] allGrids = FindObjectsOfType<GridGenerator>();
            for (int i = 0; i < allGrids.Length; i++)
            {
                GridGenerator gridGen = allGrids[i].GetComponent<GridGenerator>();
                gridGen.gameObject.transform.position = Utility.GetGridSnappedPosition
                                                        (gridGen.gameObject.transform.position, gridGen.transform.localScale);
            }
        }

        if (GUILayout.Button("Create Hallways"))
        {
            Utility.MakeStraightLines(roomGen);
        }

        if(GUILayout.Button("Delete Rooms"))
        {
            for (int i = 0; i < roomGen.roomsToDelete.Count; i++) 
            {
                Destroy(roomGen.roomsToDelete[i]);
            }
        }
    }
}
