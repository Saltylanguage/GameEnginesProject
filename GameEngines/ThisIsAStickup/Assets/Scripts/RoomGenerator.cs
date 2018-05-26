using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class RoomGenerator : MonoBehaviour
{
    public static float radius = 10;
    public int numObjects = 25;

    public float xThreshold;
    public float yThreshold;

    public static List<Vector3> randomPoints = new List<Vector3>();
    public static List<int> xRange = new List<int>();
    public static List<int> zRange = new List<int>();

    public GameObject roomTemplate;
    public GameObject hallwayTemplate;

    public List<GameObject> allRooms;

    public List<GameObject> mainRooms;
    public List<GameObject> roomsToDelete;

    public List<Vector3> roomPositions;

    public List<Geometry.Line> minimumSpanningTree;
    public List<Geometry.Line> hallways;

    public bool separateFlag = false;
    public bool isDone = false;
    public bool done = false;

    ArchitectureGenerator archGen;
    ConvexHullConstructor convexGen;

    public static Vector3 GetRandomPoint()
    {
        Vector3 ret = new Vector3();

        ret = Random.insideUnitCircle;
        ret *= radius;

        return ret;
    }

    public static void CreateRandomPoints(int count)
    {
        for (int i = 0; i < count; i++)
        {
            randomPoints.Add(GetRandomPoint());
            int tempX = (Random.Range(2, 10));
            if (tempX % 2 != 0)
            {
                tempX++;
            }
            xRange.Add(tempX);

            int tempZ = (Random.Range(2, 10));
            if (tempZ % 2 != 0)
            {
                tempZ++;
            }
            zRange.Add(tempZ);
        }
    }

    void Awake()
    {       
        archGen = GetComponent<ArchitectureGenerator>();
        convexGen = GetComponent<ConvexHullConstructor>();
        allRooms = new List<GameObject>(new GameObject[numObjects]);
        CreateRandomPoints(numObjects);
        float xSum = 0;
        float zSum = 0;
        for (int i = 0; i < numObjects; i++)
        {

            if (allRooms != null)
            {
                roomTemplate.GetComponent<GridGenerator>().currentGrid.gridSize.x = xRange[i];
                roomTemplate.GetComponent<GridGenerator>().currentGrid.gridSize.y = zRange[i];
                allRooms[i] = Instantiate(roomTemplate.gameObject);
                allRooms[i].transform.parent = transform;
            
                allRooms[i].transform.position = new Vector3(randomPoints[i].x, 0, randomPoints[i].y);

                zSum += allRooms[i].GetComponent<GridGenerator>().currentGrid.gridSize.y;
                xSum += allRooms[i].GetComponent<GridGenerator>().currentGrid.gridSize.x;
            }
            minimumSpanningTree = GetComponent<Triangulator>().minimumSpanningTree;
        }

        float xMean = (xSum / numObjects) * 0.75f;
        float zMean = (zSum / numObjects) * 0.75f;

        for (int i = 0; i < numObjects; i++)
        {
            bool biggerThanAverage = allRooms[i].GetComponent<GridGenerator>().currentGrid.gridSize.x >= xMean && allRooms[i].GetComponent<GridGenerator>().currentGrid.gridSize.y >= zMean;
            if (biggerThanAverage)
            {
                for (int j = 0; j < allRooms[i].GetComponent<GridGenerator>().allTileCoords.Count; j++)
                {
                    allRooms[i].GetComponent<GridGenerator>().allTileCoords[j].type = Geometry.CellType.MajorRoom;  
                }
                mainRooms.Add(allRooms[i]);
            }
            else
            {
                for (int j = 0; j < allRooms[i].GetComponent<GridGenerator>().allTileCoords.Count; j++)
                {
                    allRooms[i].GetComponent<GridGenerator>().allTileCoords[j].type = Geometry.CellType.MinorRoom;
                }
                roomsToDelete.Add(allRooms[i]);
            }
        }

        for (int i = 0; i < mainRooms.Count; i++)
        {
            mainRooms[i].GetComponentInChildren<MeshRenderer>().material.color = Color.red;
        }
    }

    // Use this for initialization


    void Update()
    {
        DrawLines(hallways, Color.blue);
    }

    public bool SeparateRooms()
    {
        BoxCollider temp;
        for (int i = 0; i < allRooms.Count; i++)
        {
            temp = allRooms[i].GetComponentInChildren<BoxCollider>();
            if (temp)
            {
                for (int j = 0; j < allRooms.Count; j++)
                {
                    if (i == j)
                    {
                        continue;
                    }
                    if (temp.bounds.Intersects(allRooms[j].GetComponentInChildren<BoxCollider>().bounds))
                    {
                        if (allRooms[i].GetComponentInChildren<Rigidbody>().velocity == Vector3.zero)
                        {
                            separateFlag = true;
                        }

                        if (separateFlag)
                        {
                            float randomXOffset = Random.Range(1, 5);

                            float randomZOffset = Random.Range(1, 5);

                            if (allRooms[i].transform.position.x <= allRooms[j].transform.position.x)
                            {
                                randomXOffset *= -1;
                            }
                            if (allRooms[i].transform.position.z <= allRooms[j].transform.position.z)
                            {
                                randomZOffset *= -1;
                            }

                            allRooms[i].transform.position += new Vector3(randomXOffset, 0, randomZOffset);
                            done = false;
                        }
                        SetRoomPositions();
                        separateFlag = false;
                    }
                }
            }
        }
        return true;
    }
    public void SetRoomPositions()
    {
        for (int i = 0; i < mainRooms.Count; i++)
        {
            roomPositions.Add(mainRooms[i].transform.position);
        }
    }

    public void DrawLines(List<Geometry.Line> lines, Color color)
    {
        if (lines != null && lines.Count > 0)
        {
            for (int i = 0; i < lines.Count; i++)
            {
                Debug.DrawLine(lines[i].start, lines[i].end, color);
            }
        }
    }

}





