using System.Collections;
using System.Collections.Generic;
using UnityEngine;



public class GridGenerator : MonoBehaviour
{

    public int gridIndex;

    public Transform tilePrefab;
    public Transform obstaclePrefab;

    public float outlinePercent;

    public Grid currentGrid;

    public List<Geometry.Coord> allTileCoords;
    Queue<Geometry.Coord> shuffledTileCoords;

    float mRadialForce = 500.0f; //Newtons

    Rigidbody mRigidbody;

    private void OnCollisionStay(Collision collision)
    {
        var otherPos = collision.collider.transform.position;

        Vector3 toCollider = otherPos - transform.position;
        float distanceSqr = Vector3.Magnitude(toCollider);

        if (distanceSqr == 0)
        {
            mRigidbody.AddForce(Vector3.right * mRadialForce, ForceMode.Impulse);
            collision.rigidbody.AddForce(Vector3.left * mRadialForce, ForceMode.Impulse);
        }
        else
        {
            Vector3 directionToCol = Vector3.Normalize(toCollider);
            mRigidbody.AddForce(-directionToCol * (1.5f / distanceSqr) * mRadialForce, ForceMode.Impulse);
            collision.rigidbody.AddForce(directionToCol * (1.5f / distanceSqr) * mRadialForce, ForceMode.Impulse);
        }
    }

    private void Awake()
    {
        int x = Random.Range(2, 10);
        int y = Random.Range(2, 10);
        currentGrid.gridSize = new Geometry.Coord(x, y);
        CreateGrid();
        mRigidbody = GetComponent<Rigidbody>();
        //Time.timeScale = 3;
    }

    void Start()
    {
        mRigidbody = GetComponent<Rigidbody>();
    }
    private void Update()
    {
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

    public void CreateGrid()
    {
        System.Random prng = new System.Random(currentGrid.seed);

        int xTemp = Random.Range(2, 10);
        int yTemp = Random.Range(2, 10);
        currentGrid = new Grid(xTemp, yTemp);

        if (currentGrid != null)
        {
            //Generating Coords
            allTileCoords = new List<Geometry.Coord>();

            for (int x = 0; x < currentGrid.gridSize.x; x++)
            {
                for (int y = 0; y < currentGrid.gridSize.y; y++)
                {
                    allTileCoords.Add(new Geometry.Coord((int)transform.position.x + x, (int)transform.position.y + y));
                }
            }
            shuffledTileCoords = new Queue<Geometry.Coord>(Utility.ShuffleArray(allTileCoords.ToArray(), currentGrid.seed));

            //Create Map holder  Object
            string holderName = "Generated Grid";
            if (transform.Find(holderName))
            {
                var temp = transform.Find(holderName);
                DestroyImmediate(temp.gameObject);
            }
            Transform mapHolder = new GameObject(holderName).transform;
            mapHolder.transform.parent = transform;


            //Spawning Tiles
            for (int x = 0; x < currentGrid.gridSize.x; x++)
            {
                for (int y = 0; y < currentGrid.gridSize.y; y++)
                {
                    Vector3 tilePosition = CoordToPosition(x, y);
                    Transform newTile = Instantiate(tilePrefab, tilePosition, Quaternion.Euler(Vector3.right * 90)) as Transform;
                    newTile.localScale = Vector3.one * (1 - outlinePercent);
                    newTile.parent = mapHolder;
                    BoxCollider box = GetComponent<BoxCollider>();
                    box.size = new Vector3(currentGrid.gridSize.x - outlinePercent, 1, currentGrid.gridSize.y - outlinePercent);

                    float centerX = 0;
                    float centerY = 0;

                    if (currentGrid.gridSize.x % 2 != 0)
                    {
                        centerX = 0.5f;
                    }

                    if (currentGrid.gridSize.y % 2 != 0)
                    {
                        centerY = 0.5f;
                    }

                    box.center = new Vector3(centerX, 0, centerY);
                    //Renderer tileRenderer = newTile.GetComponent<Renderer>();
                    //Material tileMaterial = new Material(tileRenderer.sharedMaterial);

                    //if (isMajorRoom)
                    //{
                    //    tileMaterial.color = Color.blue;
                    //}
                    //else
                    //{
                    //    tileMaterial.color = Color.cyan;
                    //}

                    //tileRenderer.sharedMaterial = tileMaterial;
                }
            }


            //Spawning Obstacles
            bool[,] obstacleMap = new bool[(int)currentGrid.gridSize.x, (int)currentGrid.gridSize.y];
            int totalObstacleCount = (int)(currentGrid.gridSize.x * currentGrid.gridSize.y * currentGrid.obstaclePercent);
            int currentObstacleCount = 0;

            for (int i = 0; i < totalObstacleCount; i++)
            {
                if (shuffledTileCoords != null && shuffledTileCoords.Count > 0)
                {
                    Geometry.Coord randomCoord = GetRandomCoord();
                    obstacleMap[randomCoord.x, randomCoord.y] = true;
                    currentObstacleCount++;
                    if (randomCoord != currentGrid.gridCenter && MapIsAccessible(obstacleMap, currentObstacleCount))
                    {

                        float obstacleHeight = Mathf.Lerp(currentGrid.minObstacleHeight, currentGrid.maxObstacleHeight, (float)prng.NextDouble());
                        Vector3 obstaclePosition = CoordToPosition(randomCoord.x, randomCoord.y);
                        Transform newObstacle = Instantiate(obstaclePrefab, obstaclePosition + Vector3.up * obstacleHeight / 2, Quaternion.identity) as Transform;
                        newObstacle.parent = mapHolder;
                        newObstacle.localScale = new Vector3((1 - outlinePercent), obstacleHeight, (1 - outlinePercent));

                        //Renderer obstacleRenderer = newObstacle.GetComponent<Renderer>();
                        //Material obstacleMaterial = new Material(obstacleRenderer.sharedMaterial);
                        //float colorPercent = randomCoord.y / (float)currentGrid.gridSize.y;
                        //obstacleMaterial.color = Color.Lerp(currentGrid.foregroundColor, currentGrid.backgroundColor, colorPercent);
                        //obstacleRenderer.sharedMaterial = obstacleMaterial;
                    }
                    else
                    {
                        obstacleMap[randomCoord.x, randomCoord.y] = false;
                        currentObstacleCount--;
                    }
                }
            }
        }

    }

    bool MapIsAccessible(bool[,] obstacleMap, int currentObstacleCount)
    {
        bool[,] mapFlags = new bool[obstacleMap.GetLength(0), obstacleMap.GetLength(1)];
        Queue<Geometry.Coord> coordQueue = new Queue<Geometry.Coord>();
        coordQueue.Enqueue(currentGrid.gridCenter);
        mapFlags[currentGrid.gridCenter.x, currentGrid.gridCenter.y] = true;

        int accessibleTileCount = 1;

        while (coordQueue.Count > 0)
        {
            Geometry.Coord tile = coordQueue.Dequeue();

            for (int x = -1; x <= 1; x++)
            {
                for (int y = -1; y <= 1; y++)
                {
                    int neighborX = tile.x + x;
                    int neighborY = tile.y + y;
                    if (x == 0 || y == 0)
                    {
                        if (neighborX >= 0 && neighborX < obstacleMap.GetLength(0)
                        && neighborY >= 0 && neighborY < obstacleMap.GetLength(1))
                        {
                            if (!mapFlags[neighborX, neighborY] && !obstacleMap[neighborX, neighborY])
                            {
                                mapFlags[neighborX, neighborY] = true;
                                coordQueue.Enqueue(new Geometry.Coord(neighborX, neighborY));
                                accessibleTileCount++;
                            }
                        }
                    }
                }
            }
        }
        int targetAccessibleTileCount = (int)(currentGrid.gridSize.x * currentGrid.gridSize.y - currentObstacleCount);
        return targetAccessibleTileCount == accessibleTileCount;
    }

    Vector3 CoordToPosition(int x, int y)
    {
        return new Vector3(-currentGrid.gridSize.x / 2 + 0.5f + x, 0, -currentGrid.gridSize.y / 2 + 0.5f + y);
    }
    public Geometry.Coord GetRandomCoord()
    {
        Geometry.Coord randomCoord = shuffledTileCoords.Dequeue();
        shuffledTileCoords.Enqueue(randomCoord);
        return randomCoord;
    }
    public void SnapToGrid(Vector3 cellDimensions)
    {
        float x = Mathf.Floor(transform.position.x / cellDimensions.x) * cellDimensions.x;
        float y = Mathf.Floor(transform.position.y / cellDimensions.y) * cellDimensions.y;
        float z = Mathf.Floor(transform.position.z / cellDimensions.z) * cellDimensions.z;

        transform.position = new Vector3(x, y, z);
    }

    [System.Serializable]
    public class Grid
    {
        public Grid(int x, int y)
        {
            gridSize = new Geometry.Coord(x, y);
        }
        public Geometry.Coord gridSize;
        [Range(0, 1)]

        public float obstaclePercent;
        public int seed;
        public float minObstacleHeight;
        public float maxObstacleHeight;
        public Color foregroundColor;
        public Color backgroundColor;

        public Geometry.Coord gridCenter { get { return new Geometry.Coord(gridSize.x / 2, gridSize.y / 2); } }

    }

}
