using System.Collections.Generic;
using UnityEngine;

public class God : MonoBehaviour
{
    ////////////////////////////////////////////////////////////////
    // Unity Defined Variables
    ////////////////////////////////////////////////////////////////

    public GameObject[] environmentPrefabs;
    public GameObject[] consumablePrefabs;
    public GameObject ground;
    public GameObject sky;

    ////////////////////////////////////////////////////////////////
    // Enums
    ////////////////////////////////////////////////////////////////

    private enum Environments
    {
        Pipe,
        Rock,
        LedgeS,
        LedgeM,
        LedgeL,
        Cloud
    }

    private enum Consumables
    {
        Pipe,
        Silver,
        Gold
    }

    ////////////////////////////////////////////////////////////////
    // Public Properties
    ////////////////////////////////////////////////////////////////

    public static float ScrollSpeed
    {
        get
        {
            return 2 + Settings.Difficulty.GetHashCode();
        }
    }

    ////////////////////////////////////////////////////////////////
    // Private Constants
    ////////////////////////////////////////////////////////////////

    /// <summary>
    /// X length, +/- Y position, and Z position of boundary objects
    /// </summary>
    private static readonly Vector3 boundaryStats = new Vector3(100, 4.5f, 1);

    /// <summary>
    /// X distance before the first environment object
    /// </summary>
    private const float firstEnvX = 20;

    /// <summary>
    /// Amount that objects spawn ahead of the player in the x direction
    /// </summary>
    private const float envXLead = 15;

    /// <summary>
    /// Z position of environment objects
    /// </summary>
    private const float envZ = 2;

    private const float skipMultiplier = 0.5f;

    private static readonly VariableValue ySdev = new VariableValue(1.5f, 4, 0.02f);

    private static readonly EnvironmentStats[] envStats =
    {
        // Pipe
        new EnvironmentStats
        {
            XDelay = new VariableValue(3, 6, 0.03f),
            YGap = new VariableValue(0.75f, 2, 0.01f),
            YMax = 4.25f,
            YBlock = 0,
            MovementProb = new VariableValue(0.5f, 0, 0.01f),
            Movement = new Vector2(0, 0.5f),
            Speed = new VariableValue(2, 0.5f, 0.05f),
            SpawnProb = 10
        },

        // Rock
        new EnvironmentStats
        {
            XDelay = new VariableValue(3, 4, 0.02f),
            YGap =  new VariableValue(0.5f, 3, 0.01f),
            YMax = 4.25f,
            YBlock = 0,
            MovementProb = new VariableValue(1f, 0.25f, 0.015f),
            Movement = new Vector2(0, 3),
            Speed = new VariableValue(3, 0.5f, 0.02f),
            SpawnProb = 8
        },

        // LedgeS
        new EnvironmentStats
        {
            XDelay = new VariableValue(2, 5, 0.1f),
            YGap = new VariableValue(3, 5, 0.1f),
            YMax = 3.0f,
            YBlock = 2,
            MovementProb = new VariableValue(0.5f, 0, 0.01f),
            Movement = new Vector2(2, 0),
            Speed = new VariableValue(2, 0.5f, 0.05f),
            SpawnProb = 5
        },

        // LedgeM
        new EnvironmentStats
        {
            XDelay = new VariableValue(3, 8, 0.1f),
            YGap = new VariableValue(3, 5, 0.1f),
            YMax = 3.0f,
            YBlock = 5,
            MovementProb = new VariableValue(0.25f, 0, 0.01f),
            Movement = new Vector2(2, 0),
            Speed = new VariableValue(1, 0.5f, 0.05f),
            SpawnProb = 2
        },

        // LedgeL
        new EnvironmentStats
        {
            XDelay = new VariableValue(3, 8, 0.1f),
            YGap = new VariableValue(3, 5, 0.1f),
            YMax = 3.0f,
            YBlock = 10,
            MovementProb = new VariableValue(0.25f, 0, 0.01f),
            Movement = new Vector2(2, 0),
            Speed = new VariableValue(1, 0.5f, 0.05f),
            SpawnProb = 1
        },

        // Plane
        new EnvironmentStats
        {
            XDelay = new VariableValue(1),
            YGap = new VariableValue(0),
            YMax = 4.0f,
            YBlock = 0,
            MovementProb = new VariableValue(1),
            Movement = new Vector2(-100, 0),
            Speed = new VariableValue(20, 5, 0.05f),
            SpawnProb = 4
        },
    };

    private static readonly ConsumableStats[] consumableStats =
    {
        // Pipe Score
        new ConsumableStats
        {
            SpawnProb = 0,
            Score = 1,
            SpeedMultiplier = 1,
            Force = Vector2.zero
        },

        // Gold
        new ConsumableStats
        {
            SpawnProb = 10 ,
            Score = 5,
            SpeedMultiplier = 1,
            Force = new Vector2(200, 0)
        },
        
        // Silver
        new ConsumableStats
        {
            SpawnProb = 25,
            Score = 2,
            SpeedMultiplier = 1,
            Force = new Vector2(125, 0)
        },

        // Speed
        new ConsumableStats
        {
            SpawnProb = 5,
            Score = 0,
            SpeedMultiplier = 2,
            Force = Vector2.zero
        },

        // Breakable
        new ConsumableStats
        {
            SpawnProb = 15,
            Score = 0,
            SpeedMultiplier = 1,
            Force = new Vector2(-250, 0)
        }
    };

    ////////////////////////////////////////////////////////////////
    // Private Properties
    ////////////////////////////////////////////////////////////////

    private float DifficultyMultiplier
    {
        get
        {
            return this.transform.position.x * Settings.difficultyMultiers[Settings.Difficulty.GetHashCode()];
        }
    }

    public static God Instance
    {
        get
        {
            return instance;
        }
    }

    ////////////////////////////////////////////////////////////////
    // Private Fields
    ////////////////////////////////////////////////////////////////

    private GameObject[] curBoundaries;
    private GameObject[] nextBoundaries;
    private float rightmostBoundaryX;

    private List<GameObject> environmentObjs = new List<GameObject>();
    private Vector3 nextEnv = new Vector3(firstEnvX / Settings.difficultyMultiers[Settings.Difficulty.GetHashCode()], 0, envZ);
    private List<Vector2> curYBlocks = new List<Vector2>();
    private List<int> envSpawnProbs = new List<int>();
    private List<int> consumableSpawnProbs = new List<int>();
    private float consumableProb = 0.25f / Settings.difficultyMultiers[Settings.Difficulty.GetHashCode()];

    public static God instance;
    private Camera gameCamera;


    ////////////////////////////////////////////////////////////////
    // Public Methods
    ////////////////////////////////////////////////////////////////

    public static bool InCamera(Vector3 position)
    {
        Vector3 viewPos = Instance.gameCamera.WorldToViewportPoint(position);
        return 0 <= viewPos.x && viewPos.x <= 1 && 0 <= viewPos.y && viewPos.y <= 1;
    }

    public static Vector3 CameraCoords(Vector3 position)
    {
        return Instance.gameCamera.WorldToViewportPoint(position);
    }

    public static void RemoveEnvironmentObj(GameObject obj)
    {
        Destroy(obj);
        Instance.environmentObjs.Remove(obj);
    }

    public static void Skip(float magnitude)
    {
        Instance.transform.position += new Vector3(Time.deltaTime * magnitude * skipMultiplier, 0, 0);
    }

    ////////////////////////////////////////////////////////////////
    // Unity Methods
    ////////////////////////////////////////////////////////////////

    private void Start ()
    {
        instance = GameObject.FindGameObjectsWithTag("GameController")[0].GetComponent<God>();
        this.gameCamera = this.GetComponentInChildren<Camera>();

        this.curBoundaries = SpawnBoundary(0);
        this.rightmostBoundaryX = boundaryStats.x;
        this.nextBoundaries = SpawnBoundary(rightmostBoundaryX);

        for (int i = 0; i < envStats.Length; i++)
        {
            for (int j = 0; j < envStats[i].SpawnProb; j++)
            {
                envSpawnProbs.Add(i);
            }
        }

        for (int i = 0; i < consumableStats.Length; i++)
        {
            for (int j = 0; j < consumableStats[i].SpawnProb; j++)
            {
                consumableSpawnProbs.Add(i);
            }
        }
    }

	private void Update ()
    {
        this.transform.Translate(new Vector3(ScrollSpeed * Time.deltaTime, 0, 0));

        if (this.transform.position.x > this.rightmostBoundaryX)
        {
            Destroy(this.curBoundaries[0]);
            Destroy(this.curBoundaries[1]);
            this.rightmostBoundaryX += boundaryStats.x;
            this.curBoundaries = this.nextBoundaries;
            this.nextBoundaries = this.SpawnBoundary(this.rightmostBoundaryX);
        }

        if (this.transform.position.x > this.nextEnv.x - envXLead)
        {
            SpawnNextObstacle();
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

    ////////////////////////////////////////////////////////////////
    // Private Methods
    ////////////////////////////////////////////////////////////////

    private GameObject[] SpawnBoundary(float x)
    {
        GameObject[] boundaries = new GameObject[2];
        boundaries[0] = Instantiate(ground, new Vector3(x, -boundaryStats.y, boundaryStats.z), Quaternion.identity);
        boundaries[1] = Instantiate(sky, new Vector3(x, boundaryStats.y, boundaryStats.z), Quaternion.identity);
        return boundaries;
    }
    
    private void SpawnNextObstacle()
    {
        if (Random.value < consumableProb)
        {
            int index = this.consumableSpawnProbs[(int)(Random.value * this.consumableSpawnProbs.Count)];
            this.SpawnConsumable(index);
            this.nextEnv.x += ConsumableStats.XDelay;
        }
        else
        {
            int index = this.envSpawnProbs[(int)(Random.value * this.envSpawnProbs.Count)];
            EnvironmentStats stats = envStats[index];

            do
            {
                this.nextEnv.y += Utilities.NormalDist(0, ySdev.GetValue(this.DifficultyMultiplier));
                this.nextEnv.y = this.nextEnv.y > stats.YMax ? 2 * stats.YMax - this.nextEnv.y : this.nextEnv.y;
                this.nextEnv.y = this.nextEnv.y < -stats.YMax ? 2 * -stats.YMax - this.nextEnv.y : this.nextEnv.y;
            }
            while (!VerifyY(this.nextEnv.y));

            if (index == Environments.Pipe.GetHashCode())
            {
                float spacing = stats.YGap.GetValue(DifficultyMultiplier);
                this.SpawnEnv(index, -spacing);
                this.SpawnEnv(index, spacing, Quaternion.Euler(0, 0, 180));
                this.SpawnConsumable(Consumables.Pipe.GetHashCode());
            }
            else
            {
                this.SpawnEnv(index);
            }

            this.nextEnv.x += stats.XDelay.GetValue(this.DifficultyMultiplier);
        }
    }

    private void SpawnEnv(int index, float yOffset = 0, Quaternion rotation = new Quaternion())
    {
        EnvironmentStats stats = envStats[index];
        GameObject newEnv = Instantiate(this.environmentPrefabs[index], this.nextEnv + new Vector3(0, yOffset, 0), rotation);

        Obstacle newObst = newEnv.GetComponent<Obstacle>();
        newObst.ObstacleType = ObstacleType.Environment;
        if (Random.value < stats.MovementProb.GetValue(this.DifficultyMultiplier))
        {
            newObst.Movement = stats.Movement;
            newObst.Speed = stats.Speed.GetValue(this.DifficultyMultiplier);
        }

        if (stats.YBlock > 0)
        {
            this.curYBlocks.Add(new Vector2(stats.YBlock + this.nextEnv.x, this.nextEnv.y));
        }
    }

    private void SpawnConsumable(int index)
    {
        ConsumableStats stats = consumableStats[index];
        GameObject newConsumable = Instantiate(
            this.consumablePrefabs[index], 
            this.nextEnv + new Vector3(0, Random.Range(-ConsumableStats.YMax, ConsumableStats.YMax), 0), 
            Quaternion.identity);

        Obstacle newObst = newConsumable.GetComponent<Obstacle>();
        newObst.ObstacleType = ObstacleType.Consumable;
        newObst.Score = stats.Score;
        newObst.Force = stats.Force;
        newObst.SpeedMultiplier = stats.SpeedMultiplier;
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
