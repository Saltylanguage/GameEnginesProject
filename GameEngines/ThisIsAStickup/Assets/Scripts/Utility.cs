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

            float xScale = 0;
            float yScale = 0;
            List<Geometry.Line> mst = roomGen.minimumSpanningTree;

            for (int i = 0; i < mst.Count; i++)
            {

                //We want to grab diagonal lines and turn them into vertical and straight lines
                //3 cases:         
                //Within x range - straight vertical line will connect both rooms
                //Within y range - straight horizontal line will connect rooms
                //Diagonal(not within either range) - An L shaped line is needed to connect both rooms.

                // First 2 cases:
                // Find the midpoint between the two rooms
                // Determine if this midpoint is within the x or y range of both rooms
                // If both rooms are in the same range along 1 axis (either x or y) we can connect both of them with a straight line.
                // If they are within range along the x-axis then this line will go from (midPoint.xPos,RoomA.yPos)  to (midPoint.xPos, RoomB.yPos)
                // If they are within range along the y-axis then this line will go from (RoomA.xPos, midPoint.yPos)  to (RoomB.xPos, midPoint.yPos)

                // Last Case: We need to draw an L shaped line to connect the two rooms
                // This L consists of 2 lines (1 vertical, 1 horizontal)
                // Determine which line is longer (rise > run ?)

                // Use the start and end of the MST line as the room positions
                // Example:  Assume rise > run so we travel vertically first
                // Draw First line from RoomA.position to (RoomA.x, RoomB.y)
                // Draw Second line from (RoomA.x, RoomB.y) to RoomB.pos

                Vector2 positionA = new Vector2();
                Vector2 positionB = new Vector2();
                Vector2 gridSizeA = new Vector2();
                Vector2 gridSizeB = new Vector2();

                //We need to determine which rooms we are dealing with and grab the grid size for each room
                for (int j = 0; j < roomGen.allRooms.Count; j++)
                {
                    // Get the positions
                    Vector3 start = mst[i].start;
                    Vector3 end = mst[i].end;
                    Vector3 room = roomGen.allRooms[j].transform.position;

                    // Ignore y
                    start.y = 0.0f;
                    end.y = 0.0f;
                    room.y = 0.0f;

                    // Is start inside the room?
                    if (Vector3.Distance(start, room) < Mathf.Epsilon)
                    {
                        positionA.x = roomGen.allRooms[j].transform.position.x;
                        positionA.y = roomGen.allRooms[j].transform.position.z;
                        gridSizeA = new Vector2(roomGen.allRooms[j].GetComponent<GridGenerator>().gridSize.x, roomGen.allRooms[j].GetComponent<GridGenerator>().gridSize.y);
                        //xScale = roomGen.allRooms[j].GetComponent<GridGenerator>().gameObject.transform.localScale.x;                    
                    }
                    // Is end inside the room?
                    else if (Vector3.Distance(end, room) < Mathf.Epsilon)
                    {
                        positionB.x = roomGen.allRooms[j].transform.position.x;
                        positionB.y = roomGen.allRooms[j].transform.position.z;
                        gridSizeB = new Vector2(roomGen.allRooms[j].GetComponent<GridGenerator>().gridSize.x, roomGen.allRooms[j].GetComponent<GridGenerator>().gridSize.y);
                        //xScale = roomGen.allRooms[j].GetComponent<GridGenerator>().gameObject.transform.localScale.x;                    
                    }
                }


                //Now we should have the right room positions


                //Next find the midpoint between RoomA and RoomB
                Vector2 midPoint = new Vector2();
                midPoint.x = ((mst[i].start.x + mst[i].end.x) / 2.0f);
                midPoint.y = ((mst[i].start.z + mst[i].end.z) / 2.0f);

                float rangeA = gridSizeA.x / 2.0f;
                float rangeB = gridSizeB.x / 2.0f;
                

                bool xInRangeOfA = (positionA.x - rangeA) <= midPoint.x && midPoint.x <= (positionA.x + rangeA);
                bool xInRangeOfB = (positionB.x - rangeB) <= midPoint.x && midPoint.x <= (positionB.x + rangeB);

                if (xInRangeOfA && xInRangeOfB)
                {
                    // if both checks return true a straight vertical line passing through the midpoint will connect both rooms
                    // draw a line from Vector2(midPoint.x,roomA.y) to Vector2(midPoint.x, roomB.y)
                    positionA.x = midPoint.x;
                    positionB.x = midPoint.x;

                    hallways.Add(new Geometry.Line(positionA, positionB));
                    continue;
                }

                //rangeA = gridSizeA.y / 2.0f;
                //rangeB = gridSizeB.y / 2.0f;

                //bool yInRangeOfA = (positionA.y - rangeA) <= midPoint.y && midPoint.y <= (positionA.y + rangeA);
                //bool yInRangeOfB = (positionB.y - rangeB) <= midPoint.y && midPoint.y <= (positionB.y + rangeB);

                //if either check returns false: repeat the process to check along the y axis
                //if (yInRangeOfA && yInRangeOfB)
                //{
                //    // if both checks return true a straight vertical line passing through the midpoint will connect both rooms
                //    // draw a line from Vector2(midPoint.x,roomA.y) to Vector2(midPoint.x, roomB.y)
                //    positionA.y = midPoint.y;
                //    positionB.y = midPoint.y;

                //    hallways.Add(new Geometry.Line(positionA, positionB));
                //    continue;
                //}
                //else
                //{
                //    float rise = positionB.y - positionA.y;
                //    float run = positionB.x - positionA.x;

                //    if (Mathf.Abs(rise) > Mathf.Abs(run))
                //    {
                //        Vector3 temp = new Vector3(positionB.x, 10, positionA.y);
                //        hallways.Add(new Geometry.Line(positionA, temp));
                //        hallways.Add(new Geometry.Line(temp, positionB));
                //        continue;
                //    }
                //    else if (Mathf.Abs(rise) < Mathf.Abs(run))
                //    {
                //        Vector3 temp = new Vector3(positionB.x, 10, positionA.y);
                //        hallways.Add(new Geometry.Line(positionA, temp));
                //        hallways.Add(new Geometry.Line(temp, positionB));
                //        continue;
                //    }

                //}

            }

        }
        roomGen.hallways = hallways;
    }
}











