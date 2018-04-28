using System.Collections;
using System.Collections.Generic;
using UnityEngine;



public class GridGenerator : MonoBehaviour
{

    public Transform tilePrefab;
    public Transform obstaclePrefab;
    public Vector2 gridSize;

    public int seed = 10;
    public int obstacleCount = 10;

    [Range(0, 1)]
    public float outLinePercent;

    public List<Geometry.Coord> allTileCoords;
    Queue<Geometry.Coord> shuffledTileCoords;

    float mRadialForce = 10000.0f; //Newtons

    bool isMajorRoom = true;

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
        CreateGrid();
        mRigidbody = GetComponent<Rigidbody>();
        //Time.timeScale = 3;
    }

    void Start()
    {

    }

    public void CreateGrid()
    {
        allTileCoords = new List<Geometry.Coord>();

        for (int x = 0; x < gridSize.x; x++)
        {
            for (int y = 0; y < gridSize.y; y++)
            {
                allTileCoords.Add(new Geometry.Coord((int)transform.position.x + x, (int)transform.position.y + y,
                isMajorRoom ? Geometry.CellType.MajorRoom : Geometry.CellType.MinorRoom));
            }
        }
        shuffledTileCoords = new Queue<Geometry.Coord>(Utility.ShuffleArray(allTileCoords.ToArray(), seed));

        string holderName = "Generated Map";
        if (transform.Find(holderName))
        {
            var temp = transform.Find(holderName);

            if (temp != null)
            {
                DestroyImmediate(temp.gameObject);
            }
        }
        Transform mapHolder = new GameObject(holderName).transform;

        mapHolder.parent = transform;

        for (int x = 0; x < gridSize.x; x++)
        {
            for (int y = 0; y < gridSize.y; y++)
            {
                Vector3 tilePosition = CoordToPosition(x, y);
                Transform newTile = Instantiate(tilePrefab, tilePosition, Quaternion.Euler(Vector3.right * 90)) as Transform;
                if (allTileCoords[x * y + y].type == Geometry.CellType.MajorRoom)
                {
                    newTile.GetComponentInChildren<MeshRenderer>().material.color = Color.red;
                }
                newTile.localScale = Vector3.one * (1 - outLinePercent);
                newTile.parent = mapHolder;
                BoxCollider box = GetComponent<BoxCollider>();
                box.size = new Vector3(gridSize.x, 1, gridSize.y);
            }
        }

        for (int i = 0; i < obstacleCount; i++)
        {
            if (shuffledTileCoords != null && shuffledTileCoords.Count > 0)
            {
                Geometry.Coord randomCoord = GetRandomCoord();
                Vector3 obstaclePosition = CoordToPosition(randomCoord.x, randomCoord.y);
                Transform newObstacle = Instantiate(obstaclePrefab, obstaclePosition + Vector3.up * 0.5f, Quaternion.identity) as Transform;
                newObstacle.localScale = Vector3.one * (1 - outLinePercent);
                newObstacle.parent = mapHolder;
            }
        }
    }

    Vector3 CoordToPosition(int x, int y)
    {
        return new Vector3(-gridSize.x / 2 + 0.5f + x, 0, -gridSize.y / 2 + 0.5f + y);
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
}
