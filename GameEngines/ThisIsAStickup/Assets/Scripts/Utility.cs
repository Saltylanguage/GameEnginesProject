using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public static class Utility
{
    public static T[] ShuffleArray<T>(T[] array, int seed)
    {
        System.Random rng = new System.Random(seed);

        for (int i = 0; i < array.Length - 1; i++)
        {
            int randomIndex = rng.Next(i, array.Length);

            T tempItem = array[randomIndex];
            array[randomIndex] = array[i];
            array[i] = tempItem;
        }
        return array;
    }

    public static Vector3 GetGridSnappedPosition(Vector3 target, Vector3 cellDimensions)
    {
        float xScale = cellDimensions.x;
        float xReciprical = 1 / xScale;
        float xTemp = target.x * xReciprical;
        float x = Mathf.Round(xTemp) / xReciprical;

        float yScale = cellDimensions.y;
        float yReciprical = 1 / yScale;
        float yTemp = target.y * yReciprical;
        float y = Mathf.Round(yTemp) / yReciprical;

        float zScale = cellDimensions.z;
        float zReciprical = 1 / zScale;
        float zTemp = target.z * zReciprical;
        float z = Mathf.Round(zTemp) / zReciprical;


        return new Vector3(x, y, z);
    }

    public static void MakeStraightLines(RoomGenerator roomGen)
    {
        List<Geometry.Line> hallways = new List<Geometry.Line>();
        if (roomGen != null)
        {
            List<Geometry.Line> mst = roomGen.minimumSpanningTree;

            for (int i = 0; i < mst.Count; i++)
            {


                //3 cases:         
                //Within x range - straight vertical line will connect both rooms
                //Within y range - straight horizontal line will connect rooms
                //Diagonal(not within either range) - An L shaped line is needed to connect both rooms.


                //First we need access to both rooms.
                //Check through the list of rooms for the room where start == position.  This is RoomA
                //Check again for the room where end == position.  This is RoomB
                Vector2 positionA = new Vector2(); //mst[i].start.x, mst[i].start.z);
                Vector2 positionB = new Vector2(); //mst[i].end.x, mst[i].end.z);
                Vector2 gridSizeA = new Vector2();
                Vector2 gridSizeB = new Vector2();
                for (int j = 0; j < roomGen.allRooms.Count; j++)
                {
                    if (mst[i].start.x == roomGen.allRooms[j].transform.position.x && mst[i].start.z == roomGen.allRooms[j].transform.position.z)
                    {
                        positionA = new Vector2(mst[i].start.x, mst[i].start.z);
                        gridSizeA = new Vector2(roomGen.allRooms[j].GetComponent<GridGenerator>().gridSize.x, roomGen.allRooms[j].GetComponent<GridGenerator>().gridSize.y);
                    }
                    else if (mst[i].end.x == roomGen.allRooms[j].transform.position.x && mst[i].end.z == roomGen.allRooms[j].transform.position.z)
                    {
                        positionB = new Vector2(mst[i].end.x, mst[i].end.z);
                        gridSizeB = new Vector2(roomGen.allRooms[j].GetComponent<GridGenerator>().gridSize.x, roomGen.allRooms[j].GetComponent<GridGenerator>().gridSize.y);
                    }
                }

                //Next grab the grid size for each room
                

                //How to tell if they are within range.

                

                //First find the midpoint between RoomA and RoomB
                Vector2 midPoint = new Vector2((positionA.x + positionB.x) / 2.0f, (positionA.y + positionB.y) / 2.0f);


                if (midPoint.x + (gridSizeA.x / 2) >= positionA.x && positionA.x >= midPoint.x - (gridSizeA.x / 2))
                {
                    if (midPoint.x + (gridSizeB.x / 2) >= positionB.x && positionB.x >= midPoint.x - (gridSizeB.x / 2))
                    {
                        // if both checks return true a straight vertical line passing through the midpoint will connect both rooms
                        // draw a line from Vector2(midPoint.x,roomA.y) to Vector2(midPoint.x, roomB.y)
                        Vector3 a = new Vector3(midPoint.x, 10, positionA.y);
                        Vector3 b = new Vector3(midPoint.x, 10, positionB.y);
                        hallways.Add(new Geometry.Line(a, b));
                    }
                }

                //if either check returns false: repeat the process to check along the y axis
                else if (midPoint.y + (gridSizeA.y / 2) >= positionA.y && positionA.y >= midPoint.y - (gridSizeA.y / 2))
                {

                    if (midPoint.y + (gridSizeB.y / 2) >= positionB.y && positionB.y >= midPoint.y - (gridSizeB.y / 2))
                    {
                        //if both y checks return true a straight horizontal line passing through the midpoint will connect both rooms.
                        Vector3 a = new Vector3(midPoint.x, 10, positionA.y);
                        Vector3 b = new Vector3(midPoint.x, 10, positionB.y);
                        hallways.Add(new Geometry.Line(a, b));
                    }
                }

                //If we get here, it means the two rooms cannot be connected with a single non-diagonal line

                //We need to draw an L shaped line from the midpoints of the two rooms, but which way first?
                //Up then right != Right then Up
                //Rule: Travel along the longer line first.            

                //That means: 
                else
                {
                    float rise = positionB.y - positionA.y;
                    float run = positionB.x - positionA.x;

                    if (rise > run)
                    {
                        Vector3 temp = new Vector3(positionA.x, 10, positionB.y);
                        hallways.Add(new Geometry.Line(positionA, temp));
                        hallways.Add(new Geometry.Line(temp, positionB));
                    }
                    else if (run > rise)
                    {
                        Vector3 temp = new Vector3(positionB.x, positionA.y);
                        hallways.Add(new Geometry.Line(positionA, temp));
                        hallways.Add(new Geometry.Line(temp, positionB));
                    }

                }
                //If rise > run 
                // travel in the y direction first (either top or bottom edge depending on signage)
                //Draw a line from roomA to Vector2(roomA.xPos, roomB.yPos). Call this line temp
                //Next Draw a line from temp to roomB

                //If run > rise
                // travel in the x direction first(either left or right edge depending on signage)
                //Draw a line from roomA to Vector2(roomB.xPos, roomA.yPos). Call this line temp
                //Next Draw a line from Vector2(roomB.xPos, roomA.yPos) to roomB

                //Do this for every edge and you will have straight, non-diagonal lines connecting every room.
            }
        }
        roomGen.hallways = hallways;
    }
}


