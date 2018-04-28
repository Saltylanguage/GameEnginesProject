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
                //// gridGen.SnapToGrid(scale);
                // if (gridGen.gridSize.x % 2 != 0)
                // {
                //     float x = gridGen.transform.position.x + 0.5f;
                //     float y = gridGen.transform.position.y;
                //     float z = gridGen.transform.position.z;
                //     gridGen.transform.position = new Vector3(x, y, z);
                // }
                // if (gridGen.gridSize.y % 2 != 0)
                // {
                //     float x = gridGen.transform.position.x;
                //     float y = gridGen.transform.position.y;
                //     float z = gridGen.transform.position.z + 0.5f;
                //     gridGen.transform.position = new Vector3(x, y, z);
                // }
                gridGen.gameObject.GetComponent<BoxCollider>().isTrigger = true;
                gridGen.gameObject.transform.position = Utility.GetGridSnappedPosition
                                                        (gridGen.gameObject.transform.position, gridGen.transform.localScale);
              
            }
        }
    }
}
