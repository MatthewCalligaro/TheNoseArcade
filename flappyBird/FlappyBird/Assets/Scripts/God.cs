using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class God : MonoBehaviour
{
    public GameObject Ground;
    public GameObject Sky;
    public GameObject ObstacleBottom;
    public GameObject ObstacleTop;

    public const float ScrollSpeed = 3;

    public const float BoundaryY = 4.5f;
    public const float BoundaryZ = 1;
    public const float BoundaryLength = 100;

    public const float firstObstacleX = 15;
    public const float ObstacleZ = 2;
    public const float ObstacleXLead = 15;
    public const int MaxObstacles = 25*2;

    public const float ObstacleXSpaceMax = 5;
    public const float ObstacleXSpaceMin = 3;
    public const float ObstacleXSpaceMultiplier = 0.01f;
    public float ObstacleXSpace
    {
        get
        {
            return Mathf.Max(ObstacleXSpaceMax - this.transform.position.x * ObstacleXSpaceMultiplier, ObstacleXSpaceMin);
        }
    }

    public const float ObstacleYSpaceMax = 4;
    public const float ObstacleYSpaceMin = 1.5f;
    public const float ObstacleYSpaceMultiplier = 0.02f;
    public float ObstacleYSpace
    {
        get
        {
            return Mathf.Max(ObstacleYSpaceMax - this.transform.position.x * ObstacleYSpaceMultiplier, ObstacleYSpaceMin);
        }
    }

    public const float ObstacleYSdevMin = 1.5f;
    public const float ObstacleYSdevMax = 4;
    public const float ObstacleYSdevMultiplier = 0.02f;
    public float ObstacleYSdev
    {
        get
        {
            return Mathf.Min(ObstacleYSdevMin + this.transform.position.x * ObstacleYSdevMultiplier, ObstacleYSdevMax);
        }
    }

    public static God instance;
    public static God Instance
    {
        get
        {
            return instance;
        }
    }

    GameObject[] curBoundaries;
    GameObject[] nextBoundaries;
    float rightmostBoundaryX;

    GameObject[] obstacles = new GameObject[MaxObstacles];
    int obstacleIndex = 0;
    float rightmostObstacleX = firstObstacleX - ObstacleXSpaceMax;
    float lastObstacleY = 0;
    Camera camera;

    public static bool InCamera(Vector3 position)
    {
        Vector3 viewPos = Instance.camera.WorldToViewportPoint(position);
        return 0 <= viewPos.x && viewPos.x <= 1 && 0 <= viewPos.y && viewPos.y <= 1;
    }

    private void Start ()
    {
        instance = GameObject.FindGameObjectsWithTag("GameController")[0].GetComponent<God>();
        camera = this.GetComponentInChildren<Camera>();

        this.curBoundaries = SpawnBoundary(0);
        this.rightmostBoundaryX = BoundaryLength;
        this.nextBoundaries = SpawnBoundary(rightmostBoundaryX);
    }

	private void Update ()
    {
        this.transform.Translate(new Vector3(ScrollSpeed * Time.deltaTime, 0, 0));

        if (this.transform.position.x > this.rightmostBoundaryX)
        {
            Destroy(this.curBoundaries[0]);
            Destroy(this.curBoundaries[1]);
            this.rightmostBoundaryX += BoundaryLength;
            this.curBoundaries = this.nextBoundaries;
            this.nextBoundaries = this.SpawnBoundary(this.rightmostBoundaryX);
        }

        if (this.transform.position.x > this.rightmostObstacleX - ObstacleXLead)
        {
            this.rightmostObstacleX += ObstacleXSpace;
            SpawnObstacles(this.rightmostObstacleX);
        }
    }

    private GameObject[] SpawnBoundary(float xOffset)
    {
        GameObject[] boundaries = new GameObject[2];
        boundaries[0] = Instantiate(Ground, new Vector3(xOffset, -BoundaryY, BoundaryZ), new Quaternion());
        boundaries[1] = Instantiate(Sky, new Vector3(xOffset, BoundaryY, BoundaryZ), new Quaternion());
        return boundaries;
    }
    
    private void SpawnObstacles(float xOffset)
    {
        if (this.obstacles[this.obstacleIndex] != null)
        {
            Destroy(this.obstacles[this.obstacleIndex]);
            Destroy(this.obstacles[this.obstacleIndex + 1]);
        }

        float spacing = ObstacleYSpace / 2;
        float y = this.lastObstacleY + Utilities.NormalDist(0, ObstacleYSdev);
        y = y > BoundaryY ? 2 * BoundaryY - y : y;
        y = y < -BoundaryY ? 2 * -BoundaryY - y : y;

        this.obstacles[this.obstacleIndex] = Instantiate(ObstacleBottom, new Vector3(xOffset, y - spacing, ObstacleZ), new Quaternion());
        this.obstacles[this.obstacleIndex + 1] = Instantiate(ObstacleTop, new Vector3(xOffset, y + spacing, ObstacleZ), new Quaternion());

        this.obstacleIndex = (this.obstacleIndex + 2) % MaxObstacles;
    }
}
