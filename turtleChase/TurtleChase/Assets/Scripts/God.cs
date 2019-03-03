using System.Collections.Generic;
using UnityEngine;

public class God : MonoBehaviour
{
    public GameObject[] EnvironmentPrefabs;
    public GameObject[] ScorePrefabs;

    public GameObject Ground;
    public GameObject Sky;

    private enum Environments
    {
        PipeBottom,
        PipeTop,
        Rock,
        LedgeS,
        LedgeM,
        LedgeL,
        Cloud
    }

    private enum Scores
    {
        Pipe,
        Silver,
        Gold
    }

    public static float ScrollSpeed
    {
        get
        {
            return 2 + Settings.Difficulty.GetHashCode();
        }
    }

    /// <summary>
    /// X length, +/- Y position, and Z position of boundary objects
    /// </summary>
    private static readonly Vector3 BoundaryStats = new Vector3(100, 4.5f, 1);

    /// <summary>
    /// X distance before the first environment object
    /// </summary>
    private const float FirstEnvX = 20;

    /// <summary>
    /// Amount that objects spawn ahead of the player in the x direction
    /// </summary>
    private const float EnvXLead = 15;

    /// <summary>
    /// Z position of environment objects
    /// </summary>
    private const float EnvZ = 2;

    private static readonly VariableDistance YSdev = new VariableDistance(1.5f, 4, 0.02f);

    private static readonly EnvironmentStats[] envStats =
    {
        // Pipe Bottom
        new EnvironmentStats
        {
            XDelay = new VariableDistance(3, 6, 0.03f),
            YGap = new VariableDistance(0.75f, 2, 0.01f),
            YMax = 4.25f,
            YBlock = 0
        },

        // Pipe Top
        new EnvironmentStats
        {
            XDelay = new VariableDistance(3, 6, 0.03f),
            YGap = new VariableDistance(0.75f, 2, 0.01f),
            YMax = 4.25f,
            YBlock = 0
        },

        // Rock
        new EnvironmentStats
        {
            XDelay = new VariableDistance(3, 4, 0.02f),
            YGap =  new VariableDistance(0.5f, 3, 0.01f),
            YMax = 4.25f,
            YBlock = 0
        },

        // LedgeS
        new EnvironmentStats
        {
            XDelay = new VariableDistance(2, 5, 0.1f),
            YGap = new VariableDistance(3, 5, 0.1f),
            YMax = 3.0f,
            YBlock = 2
        },

        // LedgeM
        new EnvironmentStats
        {
            XDelay = new VariableDistance(3, 8, 0.1f),
            YGap = new VariableDistance(3, 5, 0.1f),
            YMax = 3.0f,
            YBlock = 5
        },

        // LedgeL
        new EnvironmentStats
        {
            XDelay = new VariableDistance(3, 8, 0.1f),
            YGap = new VariableDistance(3, 5, 0.1f),
            YMax = 3.0f,
            YBlock = 10
        }
    };

    private static readonly float[] DifficultyDecays = { 1.0f, 1.5f, 2.0f };
    private float Decay
    {
        get
        {
            return this.transform.position.x * DifficultyDecays[Settings.Difficulty.GetHashCode()];
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

    private GameObject[] curBoundaries;
    private GameObject[] nextBoundaries;
    private float rightmostBoundaryX;

    private List<GameObject> environmentObjs = new List<GameObject>();
    private Vector3 nextEnv = new Vector3(FirstEnvX / DifficultyDecays[Settings.Difficulty.GetHashCode()], 0, EnvZ);
    private List<Vector2> curYBlocks = new List<Vector2>();

    private Camera gameCamera;

    public static bool InCamera(Vector3 position)
    {
        Vector3 viewPos = Instance.gameCamera.WorldToViewportPoint(position);
        return 0 <= viewPos.x && viewPos.x <= 1 && 0 <= viewPos.y && viewPos.y <= 1;
    }

    public static void RemoveEnvironmentObj(GameObject obj)
    {
        Destroy(obj);
        Instance.environmentObjs.Remove(obj);
    }

    private void Start ()
    {
        instance = GameObject.FindGameObjectsWithTag("GameController")[0].GetComponent<God>();
        this.gameCamera = this.GetComponentInChildren<Camera>();

        this.curBoundaries = SpawnBoundary(0);
        this.rightmostBoundaryX = BoundaryStats.x;
        this.nextBoundaries = SpawnBoundary(rightmostBoundaryX);
    }

	private void Update ()
    {
        this.transform.Translate(new Vector3(ScrollSpeed * Time.deltaTime, 0, 0));

        if (this.transform.position.x > this.rightmostBoundaryX)
        {
            Destroy(this.curBoundaries[0]);
            Destroy(this.curBoundaries[1]);
            this.rightmostBoundaryX += BoundaryStats.x;
            this.curBoundaries = this.nextBoundaries;
            this.nextBoundaries = this.SpawnBoundary(this.rightmostBoundaryX);
        }

        if (this.transform.position.x > this.nextEnv.x - EnvXLead)
        {
            SpawnEnv();
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.GetComponent<Obstacle>() != null && 
            collision.gameObject.GetComponent<Obstacle>().ObstacleType != ObstacleType.Boundary)
        {
            Destroy(collision.gameObject);
            this.environmentObjs.Remove(collision.gameObject);
        }
    }


    private GameObject[] SpawnBoundary(float x)
    {
        GameObject[] boundaries = new GameObject[2];
        boundaries[0] = Instantiate(Ground, new Vector3(x, -BoundaryStats.y, BoundaryStats.z), new Quaternion());
        boundaries[1] = Instantiate(Sky, new Vector3(x, BoundaryStats.y, BoundaryStats.z), new Quaternion());
        return boundaries;
    }
    
    private void SpawnEnv()
    {
        int index = (int)(Random.value * (EnvironmentPrefabs.Length - 2));
        EnvironmentStats stats = envStats[index];

        do
        {
            this.nextEnv.y += Utilities.NormalDist(0, YSdev.GetDistance(this.Decay));
            this.nextEnv.y = this.nextEnv.y > stats.YMax ? 2 * stats.YMax - this.nextEnv.y : this.nextEnv.y;
            this.nextEnv.y = this.nextEnv.y < -stats.YMax ? 2 * -stats.YMax - this.nextEnv.y : this.nextEnv.y;
        }
        while (!VerifyY(this.nextEnv.y));

        if (index <= 1)
        {
            float spacing = stats.YGap.GetDistance(Decay);
            this.environmentObjs.Add(Instantiate(
                this.EnvironmentPrefabs[Environments.PipeBottom.GetHashCode()],
                new Vector3(this.nextEnv.x, this.nextEnv.y - spacing, this.nextEnv.z),
                new Quaternion()));

            this.environmentObjs.Add(Instantiate(
                 this.EnvironmentPrefabs[Environments.PipeTop.GetHashCode()],
                 new Vector3(this.nextEnv.x, this.nextEnv.y + spacing, this.nextEnv.z),
                 new Quaternion()));

            this.environmentObjs.Add(Instantiate(
                this.ScorePrefabs[Scores.Pipe.GetHashCode()],
                new Vector3(this.nextEnv.x, 0, this.nextEnv.z),
                new Quaternion()));
        }
        else
        {
            GameObject newEnv = Instantiate(this.EnvironmentPrefabs[index], this.nextEnv, new Quaternion());
            newEnv.GetComponent<Obstacle>().Speed = Random.value > 0.6 ? 1 : 0;
            this.environmentObjs.Add(newEnv);
        }

        if (stats.YBlock > 0)
        {
            this.curYBlocks.Add(new Vector2(stats.YBlock + this.nextEnv.x, this.nextEnv.y));
        }

        this.nextEnv.x += stats.XDelay.GetDistance(this.Decay);
    }

    private bool VerifyY(float y)
    {
        for (int i = 0; i < this.curYBlocks.Count; ++i)
        {
            if (this.transform.position.x > this.curYBlocks[i].x)
            {
                this.curYBlocks.RemoveAt(i);
                i--;
            }
            else
            {
                if (Mathf.Abs(y - this.curYBlocks[i].y) < 1.0f)
                {
                    return false;
                }
            }
        }

        return true;
    }
}
