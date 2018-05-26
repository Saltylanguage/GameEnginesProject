using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

public class MapGenerator : MonoBehaviour
{

    int[,] map;

    public int width;
    public int height;

    [Range(0, 100)]
    public int randomFillPercent;
    private float actualFillPercent;

    [Range(0, 1000)]
    public int regionSizeThreshold;

    int previousFramesFillPercent;
    int previousFramesSizeThreshold;

    public string seed;
    public bool useRandomSeed;

    public int smoothIterations;

    void Start()
    {
        actualFillPercent = randomFillPercent * 0.625f;
        GenerateMap();

        previousFramesFillPercent = randomFillPercent;
        previousFramesSizeThreshold = regionSizeThreshold;
    }

    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            useRandomSeed = true;
            GenerateMap();
        }

        if (previousFramesFillPercent != randomFillPercent)
        {
            useRandomSeed = false;
            previousFramesFillPercent = randomFillPercent;
            actualFillPercent = randomFillPercent * 0.625f;
            GenerateMap();
        }
        if (previousFramesSizeThreshold != regionSizeThreshold)
        {
            useRandomSeed = false;
            previousFramesSizeThreshold = regionSizeThreshold;
            GenerateMap();
        }
    }

    void GenerateMap()
    {
        map = new int[width, height];
        RandomFillMap();

        for (int i = 0; i < smoothIterations; i++)
        {
            SmoothMap();
        }

        ProcessMap(1);
        ProcessMap(0);

        int borderSize = 5;
        int[,] borderedMap = new int[width + borderSize * 2, height + borderSize * 2];

        for (int x = 0; x < borderedMap.GetLength(0); x++)
        {
            for (int y = 0; y < borderedMap.GetLength(1); y++)
            {
                if (x >= borderSize && x < width + borderSize && y >= borderSize && y < height + borderSize)
                {
                    borderedMap[x, y] = map[x - borderSize, y - borderSize];
                }
                else
                {
                    borderedMap[x, y] = 1;
                }
            }

        }

        MeshGenerator meshGen = GetComponent<MeshGenerator>();

        meshGen.GenerateMesh(borderedMap, 1);
    }

    void ProcessMap(int tileType)
    {
        List<List<Coord>> regions = GetRegions(tileType);
        int thresholdSize = regionSizeThreshold;

        List<Room> survivingRooms = new List<Room>();

        foreach (List<Coord> region in regions)
        {
            if (region.Count < thresholdSize)
            {
                foreach (Coord tile in region)
                {
                    map[tile.tileX, tile.tileY] = (tileType - 1) * -1;
                }
            }
            else
            {
                if (tileType == 0)
                {
                    survivingRooms.Add(new Room(region, map));
                }

            }
        }

        survivingRooms.Sort();
        if (survivingRooms.Count > 0)
        {
            survivingRooms[0].isMainRoom = true;
            survivingRooms[0].isAccessibleFromMainRoom = true;
        }

        ConnectClosestRoom(survivingRooms);

    }


    void ConnectClosestRoom(List<Room> allRooms, bool forceAccessibilityFromMainRoom = false)
    {
        List<Room> roomListA = new List<Room>();
        List<Room> roomListB = new List<Room>();

        if (forceAccessibilityFromMainRoom)
        {
            foreach (Room room in allRooms)
            {
                if (room.isAccessibleFromMainRoom)
                {
                    roomListB.Add(room);
                }
                else
                {
                    roomListA.Add(room);
                }
            }
        }
        else
        {
            roomListA = allRooms;
            roomListB = allRooms;
        }

        int bestDistance = 0;
        Coord bestTileA = new Coord();
        Coord bestTileB = new Coord();
        Room bestRoomA = new Room();
        Room bestRoomB = new Room();

        bool possibleCOnnectionFound = false;

        foreach (Room roomA in roomListA)
        {
            if (!forceAccessibilityFromMainRoom)
            {
                possibleCOnnectionFound = false;
                if (roomA.connectedRooms.Count > 0)
                {
                    continue;
                }
            }

            foreach (Room roomB in roomListB)
            {
                if (roomA == roomB || roomA.IsConnected(roomB))
                {
                    continue;
                }
                if (roomA.IsConnected(roomB))
                {
                    possibleCOnnectionFound = false;
                    break;
                }

                for (int tileIndexA = 0; tileIndexA < roomA.edgeTiles.Count; tileIndexA++)
                {
                    for (int tileIndexB = 0; tileIndexB < roomB.edgeTiles.Count; tileIndexB++)
                    {
                        Coord tileA = roomA.edgeTiles[tileIndexA];
                        Coord tileB = roomB.edgeTiles[tileIndexB];

                        int distanceBetweenRooms = (int)(Mathf.Pow(tileA.tileX - tileB.tileX, 2) + Mathf.Pow(tileA.tileY - tileB.tileY, 2));

                        if (distanceBetweenRooms < bestDistance || !possibleCOnnectionFound)
                        {
                            bestDistance = distanceBetweenRooms;
                            possibleCOnnectionFound = true;
                            bestTileA = tileA;
                            bestTileB = tileB;
                            bestRoomA = roomA;
                            bestRoomB = roomB;
                        }
                    }
                }
            }

            if (possibleCOnnectionFound && !forceAccessibilityFromMainRoom)
            {
                CreatePassage(bestRoomA, bestRoomB, bestTileA, bestTileB);
            }
        }

        if (possibleCOnnectionFound && forceAccessibilityFromMainRoom)
        {
            CreatePassage(bestRoomA, bestRoomB, bestTileA, bestTileB);
            ConnectClosestRoom(allRooms, true);
        }

        if (!forceAccessibilityFromMainRoom)
        {
            ConnectClosestRoom(allRooms, true);
        }
    }

    void CreatePassage(Room roomA, Room roomB, Coord tileA, Coord tileB)
    {
        Room.ConnectRooms(roomA, roomB);
        Debug.DrawLine(CoordToWorldPoint(tileA), CoordToWorldPoint(tileB), Color.green, 3);

        List<Coord> line = GetLine(tileA, tileB);
        foreach (Coord c in line)
        {
            DrawCircle(c, 2);
        }

    }

    void DrawCircle(Coord c, int r)
    {
        for (int x = -r; x <= r; x++)
        {
            for (int y = -r; y <= r; y++)
            {
                if (x * x + y * y <= r * r)
                {
                    int drawX = c.tileX + x;
                    int drawY = c.tileY + y;
                    if (IsInMapRange(drawX, drawY))
                    {
                        map[drawX, drawY] = 0;
                    }
                }
            }

        }
    }

    List<Coord> GetLine(Coord start, Coord end)
    {
        List<Coord> line = new List<Coord>();

        int x = start.tileX;
        int y = start.tileY;

        int deltaX = end.tileX - start.tileX;
        int deltaY = end.tileY - start.tileY;

        bool inverted = false;
        int step = Math.Sign(deltaX);
        int gradientStep = Math.Sign(deltaY);

        int longest = Mathf.Abs(deltaX);
        int shortest = Mathf.Abs(deltaY);

        if (longest < shortest)
        {
            inverted = true;
            longest = Mathf.Abs(deltaY);
            shortest = Mathf.Abs(deltaX);

            step = Math.Sign(deltaY);
            gradientStep = Math.Sign(deltaX);
        }

        int gradientAccumulation = longest / 2;
        for (int i = 0; i < longest; i++)
        {
            line.Add(new Coord(x, y));

            if (inverted)
            {
                y += step;
            }
            else
            {
                x += step;
            }

            gradientAccumulation += shortest;

            if (gradientAccumulation >= longest)
            {
                if (inverted)
                {
                    x += gradientStep;
                }
                else
                {
                    y += gradientStep;
                }

                gradientAccumulation -= longest;
            }

        }

        return line;
    }


    Vector3 CoordToWorldPoint(Coord tile)
    {
        return new Vector3(-width / 2 + 0.5f + tile.tileX, 2, -height / 2 + 0.5f + tile.tileY);
    }


    List<List<Coord>> GetRegions(int tileType)
    {
        List<List<Coord>> regions = new List<List<Coord>>();

        int[,] closedList = new int[width, height];

        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                if (closedList[x, y] == 0 && map[x, y] == tileType)
                {
                    List<Coord> newRegion = GetRegionTiles(x, y);
                    regions.Add(newRegion);

                    foreach (Coord tile in newRegion)
                    {
                        closedList[tile.tileX, tile.tileY] = 1;
                    }
                }
            }
        }
        return regions;
    }


    List<Coord> GetRegionTiles(int startX, int startY)
    {
        List<Coord> tiles = new List<Coord>();

        int[,] closedList = new int[width, height];

        int tileType = map[startX, startY];
        Queue<Coord> openList = new Queue<Coord>();

        openList.Enqueue(new Coord(startX, startY));
        closedList[startX, startY] = 1;

        while (openList.Count > 0)
        {
            Coord tile = openList.Dequeue();
            tiles.Add(tile);

            for (int x = tile.tileX - 1; x <= tile.tileX + 1; x++)
            {
                for (int y = tile.tileY - 1; y <= tile.tileY + 1; y++)
                {
                    if (IsInMapRange(x, y) && (y == tile.tileY || x == tile.tileX))
                    {
                        if (closedList[x, y] == 0 && map[x, y] == tileType)
                        {
                            closedList[x, y] = 1;
                            openList.Enqueue(new Coord(x, y));
                        }
                    }
                }
            }
        }

        return tiles;
    }



    bool IsInMapRange(int x, int y)
    {
        return x >= 0 && x < width && y >= 0 && y < height;
    }
    void RandomFillMap()
    {
        if (useRandomSeed)
        {
            seed = Time.time.ToString();
        }

        System.Random pseudoRandom = new System.Random(seed.GetHashCode());

        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                if (x == 0 || x == width - 1 || y == 0 || y == height - 1)
                {
                    map[x, y] = 1;
                }
                else
                {
                    map[x, y] = (pseudoRandom.Next(0, 100) < actualFillPercent) ? 1 : 0;
                }
            }
        }
    }


    void SmoothMap()
    {
        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                int neighborWallCount = GetSurroundingWallCount(x, y);

                if (neighborWallCount > 4)
                {
                    map[x, y] = 1;
                }
                else if (neighborWallCount < 4)
                {
                    map[x, y] = 0;
                }
            }
        }
    }



    int GetSurroundingWallCount(int gridX, int gridY)
    {
        int wallCount = 0;

        for (int neighborX = gridX - 1; neighborX <= gridX + 1; neighborX++)
        {
            for (int neighborY = gridY - 1; neighborY <= gridY + 1; neighborY++)
            {
                if (neighborX >= 0 && neighborX < width && neighborY >= 0 && neighborY < height)
                {
                    if (neighborX != gridX || neighborY != gridY)
                    {
                        wallCount += map[neighborX, neighborY];
                    }

                }
                else
                {
                    wallCount++;
                }
            }
        }

        return wallCount;
    }



    struct Coord
    {
        public int tileX;
        public int tileY;

        public Coord(int x, int y)
        {
            tileX = x;
            tileY = y;
        }
    }

    class Room : IComparable<Room>
    {
        public List<Coord> tiles;
        public List<Coord> edgeTiles;
        public List<Room> connectedRooms;
        public int roomSize;
        public bool isAccessibleFromMainRoom;
        public bool isMainRoom;

        public Room() { }

        public Room(List<Coord> roomTiles, int[,] map)
        {
            tiles = roomTiles;
            roomSize = tiles.Count;
            connectedRooms = new List<Room>();

            edgeTiles = new List<Coord>();

            foreach (Coord tile in tiles)
            {
                for (int x = tile.tileX - 1; x <= tile.tileX + 1; x++)
                {
                    for (int y = tile.tileY - 1; y <= tile.tileY + 1; y++)
                    {
                        if (x == tile.tileX || y == tile.tileY)
                        {
                            if (map[x, y] == 1)
                            {
                                edgeTiles.Add(tile);
                            }
                        }
                    }
                }
            }
        }

        public void SetAccessibleFromMainRoom()
        {
            if (!isAccessibleFromMainRoom)
            {
                isAccessibleFromMainRoom = true;
                foreach (Room connectedRoom in connectedRooms)
                {
                    connectedRoom.SetAccessibleFromMainRoom();
                }
            }
        }

        public static void ConnectRooms(Room roomA, Room roomB)
        {
            if (roomA.isAccessibleFromMainRoom)
            {
                roomB.SetAccessibleFromMainRoom();
            }
            else if (roomB.isAccessibleFromMainRoom)
            {
                roomA.SetAccessibleFromMainRoom();
            }
            roomA.connectedRooms.Add(roomB);
            roomB.connectedRooms.Add(roomA);
        }

        public bool IsConnected(Room otherRoom)
        {
            return connectedRooms.Contains(otherRoom);
        }

        public int CompareTo(Room otherRoom)
        {           
            return otherRoom.roomSize.CompareTo(roomSize);
        }

    }

}


