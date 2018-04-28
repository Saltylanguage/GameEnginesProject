using UnityEngine;
using System.Collections;

public static class Utility
{
    public static T[] ShuffleArray<T>(T[] array, int seed)
    {
        System.Random rng = new System.Random(seed);

        for(int i = 0; i < array.Length - 1; i++)
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
    


}
