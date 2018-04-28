using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class RoomGenerator : MonoBehaviour
{
    public static float radius = 300;
    public int numObjects = 25;

    public float xThreshold;
    public float yThreshold;

    public static List<Vector3> randomPoints = new List<Vector3>();
    public static List<float> xRange = new List<float>();
    public static List<float> zRange = new List<float>();

    public GameObject roomTemplate;

    public List<GameObject> allRooms;

    public List<GameObject> mainRooms;
    public List<GameObject> roomsToDelete;

    public List<Vector3> roomPositions;


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
                roomTemplate.GetComponent<GridGenerator>().gridSize.x = xRange[i];
                roomTemplate.GetComponent<GridGenerator>().gridSize.y = zRange[i];
                allRooms[i] = Instantiate(roomTemplate.gameObject);


                allRooms[i].transform.localScale = new Vector3(10, 1, 10);
                allRooms[i].transform.position = new Vector3(randomPoints[i].x, 10, randomPoints[i].y);

                zSum += allRooms[i].GetComponent<GridGenerator>().gridSize.y;
                xSum += allRooms[i].GetComponent<GridGenerator>().gridSize.x;
            }
        }

        float xMean = (xSum / numObjects) * 0.75f;
        float zMean = (zSum / numObjects) * 0.75f;

        for (int i = 0; i < numObjects; i++)
        {
            if (allRooms[i].GetComponent<GridGenerator>().gridSize.x >= xMean && allRooms[i].GetComponent<GridGenerator>().gridSize.y >= zMean)
            {
                for (int j = 0; j < allRooms[i].GetComponent<GridGenerator>().allTileCoords.Count; j++)
                {
                    allRooms[i].GetComponent<GridGenerator>().allTileCoords[j].type = Geometry.CellType.MajorRoom;
                }
                mainRooms.Add(allRooms[i]);
            }
            else
            {
                roomsToDelete.Add(allRooms[i]);
            }
        }

        for (int i = 0; i < mainRooms.Count; i++)
        {
            mainRooms[i].GetComponentInChildren<MeshRenderer>().material.color = Color.red;
        }
    }

    // Use this for initialization
    void Start()
    {


    }

    void Update()
    {
        //SeparateRooms();
    }

    private void FixedUpdate()
    {
        //SeparateRooms();
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


    public void CreateHallway(Geometry.Line connection)
    {
        // 1- first get the midpoint between the two room positions.
        // 2 -check if this midpoint is within the boundaries of both rooms along either the x or y axis
        //   a) if in x boundary: draw a vertical line between rooms at the midpoint
        //   b) if in y boundary: draw a horizontal line between rooms at the midpoint
        //   c) if in nether boundary: draw a horizontal line from each room's midpoint to it's counterpart's midpoint along 1 axis

    }




}





