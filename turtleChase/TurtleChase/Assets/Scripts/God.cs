using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Controls the camera and procedurally generates the level
/// </summary>
public class God : MonoBehaviour
{
    ////////////////////////////////////////////////////////////////
    // Unity-Assigned Properties
    ////////////////////////////////////////////////////////////////

    public GameObject[] environmentPrefabs;
    public GameObject[] consumablePrefabs;
    public GameObject ground;
    public GameObject sky;



    ////////////////////////////////////////////////////////////////
    // Enums
    ////////////////////////////////////////////////////////////////

    /// <summary>
    /// The environment objects which can be added to the level
    /// </summary>
    private enum Environments
    {
        Pipe,
        Rock,
        LedgeS,
        LedgeM,
        LedgeL,
        Cloud
    }

    /// <summary>
    /// The consumable objects which can be added to the level
    /// </summary>
    private enum Consumables
    {
        Pipe,
        Silver,
        Gold,
        Speed, 
        Knockback
    }



    ////////////////////////////////////////////////////////////////
    // Public Properties
    ////////////////////////////////////////////////////////////////

    /// <summary>
    /// The speed at which the camera scrolls in the positive X direction
    /// </summary>
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
    /// X position at which the first environment object is spawned
    /// </summary>
    private const float firstEnvX = 20;

    /// <summary>
    /// Amount that objects spawn ahead of the player in the X direction
    /// </summary>
    private const float envXLead = 15;

    /// <summary>
    /// Z position of environment objects
    /// </summary>
    private const float envZ = 2;

    /// <summary>
    /// Multiplier to the speed at which the camera skips forward to catch up with the player
    /// </summary>
    private const float skipMultiplier = 0.5f;

    /// <summary>
    /// Standard deviation for the Markov chain used to determine the Y position of environment objects
    /// </summary>
    private static readonly VariableValue ySdev = new VariableValue(1.5f, 4, 0.02f);

    /// <summary>
    /// Stats defining environment objects
    /// </summary>
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

    /// <summary>
    /// Stats defining consumable objects
    /// </summary>
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
            SpawnProb = 10,
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

        // Knockback
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

    private static God Instance
    {
        get
        {
            return instance;
        }
    }



    ////////////////////////////////////////////////////////////////
    // Private Fields
    ////////////////////////////////////////////////////////////////

    /// <summary>
    /// The current left sky and ground boundaries
    /// </summary>
    private GameObject[] leftBoundaries;

    /// <summary>
    /// the current right sky and ground boundaries
    /// </summary>
    private GameObject[] rightBoundaries;

    /// <summary>
    /// The X position of the right sky and ground boundaries
    /// </summary>
    private float rightBoundaryX;

    /// <summary>
    /// List of all environment and consumable objects currently in play
    /// </summary>
    private List<GameObject> environmentObjs = new List<GameObject>();

    /// <summary>
    /// Position of the next environment object to be spawned
    /// </summary>
    private Vector3 nextEnv = new Vector3(firstEnvX / Settings.difficultyMultiers[Settings.Difficulty.GetHashCode()], 0, envZ);

    /// <summary>
    /// List of Y positions which are blocked by a horizontal environment piece as (rightmost X position, blocked Y position)
    /// </summary>
    private List<Vector2> curYBlocks = new List<Vector2>();

    /// <summary>
    /// List of Environments indicies (with repetitions) from which new environment objects are drawn
    /// </summary>
    private List<int> envSpawnProbs = new List<int>();

    /// <summary>
    /// List of Consumables indices (with repetitions) from which new consumable objects are drawn
    /// </summary>
    private List<int> consumableSpawnProbs = new List<int>();

    /// <summary>
    /// Probability of spawning a consumable
    /// </summary>
    private float consumableProb = 0.25f / Settings.difficultyMultiers[Settings.Difficulty.GetHashCode()];

    /// <summary>
    /// Static reference to self to allow other classes to statically call God (since there is only one God at a time)
    /// </summary>
    private static God instance;

    /// <summary>
    /// Reference to the Camera child of God
    /// </summary>
    private Camera gameCamera;



    ////////////////////////////////////////////////////////////////
    // Public Methods
    ////////////////////////////////////////////////////////////////

    /// <summary>
    /// Determines whether a point in world coordinates is in the Camera's viewport
    /// </summary>
    /// <param name="position">A point in world coordinates</param>
    /// <returns>True if the point is in the Camera's viewport</returns>
    public static bool InCamera(Vector3 position)
    {
        Vector3 viewPos = Instance.gameCamera.WorldToViewportPoint(position);
        return 0 <= viewPos.x && viewPos.x <= 1 && 0 <= viewPos.y && viewPos.y <= 1;
    }

    /// <summary>
    /// Converts a point in world coordinates to camera coordinates
    /// </summary>
    /// <param name="position">A point in world coordinates</param>
    /// <returns>The corresponding point in camera coordinates</returns>
    public static Vector3 CameraCoords(Vector3 position)
    {
        return Instance.gameCamera.WorldToViewportPoint(position);
    }

    /// <summary>
    /// Destroys and removes an environment object from the environment buffer
    /// </summary>
    /// <param name="obj">Environment or consumable object to destroy</param>
    public static void RemoveEnvironmentObj(GameObject obj)
    {
        Destroy(obj);
        Instance.environmentObjs.Remove(obj);
    }

    /// <summary>
    /// Moves the camera forward a certain amount in case the Player gets ahead of the camera
    /// </summary>
    /// <param name="magnitude">Amount to move camera forward</param>
    public static void Skip(float magnitude)
    {
        Instance.transform.position += new Vector3(Time.deltaTime * magnitude * skipMultiplier, 0, 0);
    }

    ////////////////////////////////////////////////////////////////
    // Unity Methods
    ////////////////////////////////////////////////////////////////

    private void Start ()
    {
        // Find self and camera
        instance = GameObject.FindGameObjectsWithTag("GameController")[0].GetComponent<God>();
        this.gameCamera = this.GetComponentInChildren<Camera>();

        // Spawn first two pairs of boundaries
        this.leftBoundaries = SpawnBoundary(0);
        this.rightBoundaryX = boundaryStats.x;
        this.rightBoundaries = SpawnBoundary(rightBoundaryX);

        // Generate spawn probabilities for environment and consumable objects based on their stats
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
        // Scroll camera
        this.transform.Translate(new Vector3(ScrollSpeed * Time.deltaTime, 0, 0));

        // Spawn the next boundary when we are past the middle of the rightmost boundary
        if (this.transform.position.x > this.rightBoundaryX)
        {
            Destroy(this.leftBoundaries[0]);
            Destroy(this.leftBoundaries[1]);
            this.rightBoundaryX += boundaryStats.x;
            this.leftBoundaries = this.rightBoundaries;
            this.rightBoundaries = this.SpawnBoundary(this.rightBoundaryX);
        }

        // Spawn the next environment or consumable object
        if (this.transform.position.x > this.nextEnv.x - envXLead)
        {
            this.SpawnNextObstacle();
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        // Destroy environment and consumable objects on collision (since they are out of player view)
        if (collision.gameObject.GetComponent<Obstacle>() != null && 
            collision.gameObject.GetComponent<Obstacle>().ObstacleType != ObstacleType.Boundary)
        {
            RemoveEnvironmentObj(collision.gameObject);
        }
    }

    ////////////////////////////////////////////////////////////////
    // Private Methods
    ////////////////////////////////////////////////////////////////

    /// <summary>
    /// Spawns a pair of boundaries (sky and ground) at a specified X positon
    /// </summary>
    /// <param name="x">The X position at which to spawn the boundaries</param>
    /// <returns>The pair of spawned boundaries</returns>
    private GameObject[] SpawnBoundary(float x)
    {
        GameObject[] boundaries = new GameObject[2];
        boundaries[0] = Instantiate(ground, new Vector3(x, -boundaryStats.y, boundaryStats.z), Quaternion.identity);
        boundaries[1] = Instantiate(sky, new Vector3(x, boundaryStats.y, boundaryStats.z), Quaternion.identity);
        return boundaries;
    }
    
    /// <summary>
    /// Spawns the next environment or consumable object at nextEnv
    /// </summary>
    private void SpawnNextObstacle()
    {
        // Randomly choose if we should spawn a consumable object
        if (Random.value < consumableProb)
        {
            // Randomly choose a consumable to spawn from consumableSpawnProbs
            int index = this.consumableSpawnProbs[(int)(Random.value * this.consumableSpawnProbs.Count)];
            this.SpawnConsumable(index);
            this.nextEnv.x += ConsumableStats.XDelay;
        }
        else
        {
            // Randomly choose an environment to spawn from envSpawnProbs
            int index = this.envSpawnProbs[(int)(Random.value * this.envSpawnProbs.Count)];
            EnvironmentStats stats = envStats[index];

            // Use a Markov model to choose the next Y position based on the current Y position
            this.nextEnv.y += Utilities.NormalDist(0, ySdev.GetValue(this.DifficultyMultiplier));

            // Adjust the Y position to make sure that it is within the YMax for this environment and does
            // not fall on a blocked Y position in curYBlocks
            do
            {
                this.nextEnv.y = this.nextEnv.y > stats.YMax ? 2 * stats.YMax - this.nextEnv.y : this.nextEnv.y;
                this.nextEnv.y = this.nextEnv.y < -stats.YMax ? 2 * -stats.YMax - this.nextEnv.y : this.nextEnv.y;
            }
            while (!VerifyY(this.nextEnv.y));

            // For pipes, we always spawn two (top and bottom) and a pipe score in between
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

    /// <summary>
    /// Spawn a particular environment object with the specified parameters
    /// </summary>
    /// <param name="index">The Environments index of the object to be spawned</param>
    /// <param name="yOffset">The Y offset of the object from nextEnv.y</param>
    /// <param name="rotation">The amount to rotate the object</param>
    private void SpawnEnv(int index, float yOffset = 0, Quaternion rotation = new Quaternion())
    { 
        // Spawn the specified environment object at the specified position/rotation
        GameObject newEnv = Instantiate(this.environmentPrefabs[index], this.nextEnv + new Vector3(0, yOffset, 0), rotation);

        // Set the environment object's public properties based on its stats in envStats
        EnvironmentStats stats = envStats[index];
        Obstacle newObst = newEnv.GetComponent<Obstacle>();
        newObst.ObstacleType = ObstacleType.Environment;
        if (Random.value < stats.MovementProb.GetValue(this.DifficultyMultiplier))
        {
            newObst.Movement = stats.Movement;
            newObst.Speed = stats.Speed.GetValue(this.DifficultyMultiplier);
        }

        // If the environment object blocks its Y position, add this to curYBlocks
        if (stats.YBlock > 0)
        {
            this.curYBlocks.Add(new Vector2(stats.YBlock + this.nextEnv.x, this.nextEnv.y));
        }
    }

    /// <summary>
    /// Spawn a particular consumable object
    /// </summary>
    /// <param name="index">The Consumables index of the object to be spawned</param>
    private void SpawnConsumable(int index)
    {
        // Spawn the specified consumable object at a random Y position at nextEnv.x
        ConsumableStats stats = consumableStats[index];
        GameObject newConsumable = Instantiate(
            this.consumablePrefabs[index], 
            new Vector3(this.nextEnv.x, Random.Range(-ConsumableStats.YMax, ConsumableStats.YMax), this.nextEnv.z), 
            Quaternion.identity);

        // Set the consumable object's public properties based on its stats in consumableStats
        Obstacle newObst = newConsumable.GetComponent<Obstacle>();
        newObst.ObstacleType = ObstacleType.Consumable;
        newObst.Score = stats.Score;
        newObst.Force = stats.Force;
        newObst.SpeedMultiplier = stats.SpeedMultiplier;
    }

    /// <summary>
    /// Check if a Y position conflicts with a blocked Y in curYBlocks
    /// </summary>
    /// <param name="y">The Y position to check</param>
    /// <returns>true if the Y position does not conflict with any Y blocks</returns>
    private bool VerifyY(float y)
    {
        for (int i = 0; i < this.curYBlocks.Count; ++i)
        {
            // Remove a Y block if the ending x position has been passed
            if (this.transform.position.x > this.curYBlocks[i].x)
            {
                this.curYBlocks.RemoveAt(i);
                i--;
            }
            else
            {
                // Return false if we are within 1.0 of any Y block
                if (Mathf.Abs(y - this.curYBlocks[i].y) < 1.0f)
                {
                    return false;
                }
            }
        }

        return true;
    }
}
