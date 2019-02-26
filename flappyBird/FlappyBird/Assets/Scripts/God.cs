using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class God : MonoBehaviour
{
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
            return Mathf.Max(ObstacleXSpaceMax - transform.position.x * ObstacleXSpaceMultiplier, ObstacleXSpaceMin);
        }
    }

    public const float ObstacleYSpaceMax = 4;
    public const float ObstacleYSpaceMin = 1.5f;
    public const float ObstacleYSpaceMultiplier = 0.02f;
    public float ObstacleYSpace
    {
        get
        {
            return Mathf.Max(ObstacleYSpaceMax - transform.position.x * ObstacleYSpaceMultiplier, ObstacleYSpaceMin);
        }
    }

    public const float ObstacleYSdevMin = 1.5f;
    public const float ObstacleYSdevMax = 4;
    public const float ObstacleYSdevMultiplier = 0.02f;
    public float ObstacleYSdev
    {
        get
        {
            return Mathf.Min(ObstacleYSdevMin + transform.position.x * ObstacleYSdevMultiplier, ObstacleYSdevMax);
        }
    }

    public GameObject Ground;
    public GameObject Sky;
    public GameObject ObstacleBottom;
    public GameObject ObstacleTop;

    GameObject[] curBoundaries;
    GameObject[] nextBoundaries;
    float rightmostBoundaryX;

    GameObject[] obstacles = new GameObject[MaxObstacles];
    int obstacleIndex = 0;
    float rightmostObstacleX = firstObstacleX - ObstacleXSpaceMax;
    float lastObstacleY = 0;

	private void Start ()
    {
        curBoundaries = SpawnBoundary(0);
        rightmostBoundaryX = BoundaryLength;
        nextBoundaries = SpawnBoundary(rightmostBoundaryX);
    }

	private void Update ()
    {
        transform.Translate(new Vector3(God.ScrollSpeed * Time.deltaTime, 0, 0));

        if (transform.position.x > rightmostBoundaryX)
        {
            Destroy(curBoundaries[0]);
            Destroy(curBoundaries[1]);
            rightmostBoundaryX += BoundaryLength;
            curBoundaries = nextBoundaries;
            nextBoundaries = SpawnBoundary(rightmostBoundaryX);
        }

        if (transform.position.x > rightmostObstacleX - ObstacleXLead)
        {
            rightmostObstacleX += ObstacleXSpace;
            SpawnObstacles(rightmostObstacleX);
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
        if (obstacles[obstacleIndex] != null)
        {
            Destroy(obstacles[obstacleIndex]);
            Destroy(obstacles[obstacleIndex + 1]);
        }

        float spacing = ObstacleYSpace / 2;
        float y = lastObstacleY + Utilities.NormalDist(0, ObstacleYSdev);
        y = y > BoundaryY ? 2 * BoundaryY - y : y;
        y = y < -BoundaryY ? 2 * -BoundaryY - y : y;

        obstacles[obstacleIndex] = Instantiate(ObstacleBottom, new Vector3(xOffset, y - spacing, ObstacleZ), new Quaternion());
        obstacles[obstacleIndex + 1] = Instantiate(ObstacleTop, new Vector3(xOffset, y + spacing, ObstacleZ), new Quaternion());

        obstacleIndex = (obstacleIndex + 2) % MaxObstacles;
    }
}
