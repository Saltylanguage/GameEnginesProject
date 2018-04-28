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
            for (int i = 0; i < roomGen.allRooms.Count; i++)
            {
                Vector3 scale = roomGen.allRooms[i].gameObject.transform.localScale;
                GridGenerator gridGen = roomGen.allRooms[i].GetComponent<GridGenerator>();

                gridGen.gameObject.GetComponent<BoxCollider>().isTrigger = true;
                gridGen.gameObject.transform.position = Utility.GetGridSnappedPosition
                                                        (gridGen.gameObject.transform.position, gridGen.transform.localScale);

            }
        }

        if (GUILayout.Button("Create Hallways"))
        {
            Utility.MakeStraightLines(roomGen);          
        }
    }


}
