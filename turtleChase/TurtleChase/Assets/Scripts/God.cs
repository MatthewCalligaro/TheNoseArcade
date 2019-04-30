using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// The obstacles which can be added to the level
/// </summary>
public enum Obstacles
{
    Pipe,
    Rock,
    LedgeS,
    LedgeM,
    LedgeL,
    Plane
}



/// <summary>
/// The consumable objects which can be added to the level
/// </summary>
public enum Consumables
{
    Pipe,
    Silver,
    Gold,
    Speed,
    Knockback
}



/// <summary>
/// Controls the camera and procedurally generates the level
/// </summary>
public class God : MonoBehaviour
{
    ////////////////////////////////////////////////////////////////
    // Unity-Assigned Fields
    ////////////////////////////////////////////////////////////////

    public GameObject[] ObstaclePrefabs;
    public GameObject[] ConsumablePrefabs;
    public GameObject Ground;
    public GameObject Sky;



    ////////////////////////////////////////////////////////////////
    // Properties
    ////////////////////////////////////////////////////////////////

    /// <summary>
    /// Prevents the camera from scrolling forward when true
    /// </summary>
    public static bool Stopped { get; set; }

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

    /// <summary>
    /// Current relative difficulty based on Difficulty setting and X position
    /// </summary>
    protected virtual float DifficultyMultiplier
    {
        get
        {
            return this.transform.position.x * Settings.DifficultyMultipliers[Settings.Difficulty.GetHashCode()];
        }
    }


    ////////////////////////////////////////////////////////////////
    // Fields
    ////////////////////////////////////////////////////////////////

    /// <summary>
    /// Z position of environment objects
    /// </summary>
    protected const float envZ = 2;

    /// <summary>
    /// Amount that environment objects spawn ahead of the player in the X direction
    /// </summary>
    protected const float envXLead = 15;

    /// <summary>
    /// Maximum Y position of environment objects
    /// </summary>
    protected const float envMaxY = 4.25f;

    /// <summary>
    /// Stats defining obstacles
    /// </summary>
    protected static readonly ObstacleStats[] obstacleStats =
    {
        // Pipe
        new ObstacleStats
        {
            XDelay = new VariableValue(3, 6, 0.03f),
            YGap = new VariableValue(0.75f, 2, 0.01f),
            YOffset = 0,
            YBlock = 0,
            MovementProb = new VariableValue(0.5f, 0, 0.01f),
            Movement = new Vector2(0, 0.5f),
            Speed = new VariableValue(2, 0.5f, 0.05f),
            SpawnProb = 10
        },

        // Rock
        new ObstacleStats
        {
            XDelay = new VariableValue(3, 4, 0.02f),
            YGap =  new VariableValue(0.5f, 3, 0.01f),
            YOffset = 1f,
            YBlock = 0,
            MovementProb = new VariableValue(1f, 0.25f, 0.015f),
            Movement = new Vector2(0, 3),
            Speed = new VariableValue(3, 0.5f, 0.02f),
            SpawnProb = 8
        },

        // LedgeS
        new ObstacleStats
        {
            XDelay = new VariableValue(2, 5, 0.1f),
            YGap = new VariableValue(3, 5, 0.1f),
            YOffset = 0.5f,
            YBlock = 2,
            MovementProb = new VariableValue(0.5f, 0, 0.01f),
            Movement = new Vector2(2, 0),
            Speed = new VariableValue(2, 0.5f, 0.05f),
            SpawnProb = 5
        },

        // LedgeM
        new ObstacleStats
        {
            XDelay = new VariableValue(3, 8, 0.1f),
            YGap = new VariableValue(3, 5, 0.1f),
            YOffset = 0.5f,
            YBlock = 5,
            MovementProb = new VariableValue(0.25f, 0, 0.01f),
            Movement = new Vector2(2, 0),
            Speed = new VariableValue(1, 0.5f, 0.05f),
            SpawnProb = 2
        },

        // LedgeL
        new ObstacleStats
        {
            XDelay = new VariableValue(3, 8, 0.1f),
            YGap = new VariableValue(3, 5, 0.1f),
            YOffset = 0.5f,
            YBlock = 10,
            MovementProb = new VariableValue(0.25f, 0, 0.01f),
            Movement = new Vector2(2, 0),
            Speed = new VariableValue(1, 0.5f, 0.05f),
            SpawnProb = 1
        },

        // Plane
        new ObstacleStats
        {
            XDelay = new VariableValue(1),
            YGap = new VariableValue(0),
            YOffset = 0.75f,
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
    protected static readonly ConsumableStats[] consumableStats =
    {
        // Pipe Score
        new ConsumableStats
        {
            SpawnProb = 0,
            Score = 1,
            SpeedMultiplier = 1,
            Force = Vector2.zero
        },
        
        // Silver
        new ConsumableStats
        {
            SpawnProb = 25,
            Score = 2,
            SpeedMultiplier = 1,
            Force = new Vector2(125, 0)
        },

        // Gold
        new ConsumableStats
        {
            SpawnProb = 10,
            Score = 5,
            SpeedMultiplier = 1,
            Force = new Vector2(200, 0)
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

    /// <summary>
    /// Whether the God should precedurally generate obstacles and consumables
    /// </summary>
    protected bool generateLevel = true;

    /// <summary>
    /// X position at which the first environment object is spawned
    /// </summary>
    private const float firstEnvX = 20;

    /// <summary>
    /// Multiplier to the speed at which the camera skips forward to catch up with the player
    /// </summary>
    private const float skipMultiplier = 0.5f;

    /// <summary>
    /// Distance to leave for player to pass through on ideal path
    /// </summary>
    private const float playerSpace = 0.5f;

    /// <summary>
    /// X length, +/- Y position, and Z position of boundary objects
    /// </summary>
    private static readonly Vector3 boundaryStats = new Vector3(100, 4.5f, 1);

    /// <summary>
    /// Standard deviation for the Markov chain used to determine the Y position of environment objects
    /// </summary>
    private static readonly VariableValue ySdev = new VariableValue(1.5f, 4, 0.02f);

    /// <summary>
    /// Static reference to the one God object in the scene to enable static methods
    /// </summary>
    private static God instance;

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
    /// List of all obstacles and consumable objects currently in play
    /// </summary>
    private List<GameObject> environmentObjs = new List<GameObject>();

    /// <summary>
    /// Position of the ideal path at the X position where the next environment object will be spawned
    /// </summary>
    private Vector3 nextEnv = new Vector3(firstEnvX / Settings.DifficultyMultipliers[Settings.Difficulty.GetHashCode()], 0, envZ);

    /// <summary>
    /// List of Y positions which are blocked by a horizontal obstacle as (rightmost X position, blocked Y position)
    /// </summary>
    private List<Vector2> curYBlocks = new List<Vector2>();

    /// <summary>
    /// List of Obstacles indicies (with repetitions) from which new obstacles are drawn
    /// </summary>
    private List<int> obstacleSpawnProbs = new List<int>();

    /// <summary>
    /// List of Consumables indices (with repetitions) from which new consumable objects are drawn
    /// </summary>
    private List<int> consumableSpawnProbs = new List<int>();

    /// <summary>
    /// Probability of spawning a consumable
    /// </summary>
    private float consumableProb = 0.25f / Settings.DifficultyMultipliers[Settings.Difficulty.GetHashCode()];

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
        Vector3 viewPos = instance.gameCamera.WorldToViewportPoint(position);
        return 0 <= viewPos.x && viewPos.x <= 1 && 0 <= viewPos.y && viewPos.y <= 1;
    }

    /// <summary>
    /// Converts a point in world coordinates to camera coordinates
    /// </summary>
    /// <param name="position">A point in world coordinates</param>
    /// <returns>The corresponding point in camera coordinates</returns>
    public static Vector3 CameraCoords(Vector3 position)
    {
        return instance.gameCamera.WorldToViewportPoint(position);
    }

    /// <summary>
    /// Destroys and removes an environment object from the environment buffer
    /// </summary>
    /// <param name="obj">Environment object to destroy</param>
    public static void RemoveEnvironmentObj(GameObject obj)
    {
        Destroy(obj);
        instance.environmentObjs.Remove(obj);
    }

    /// <summary>
    /// Moves the camera forward a certain amount in case the Player gets ahead of the camera
    /// </summary>
    /// <param name="magnitude">Amount to move camera forward</param>
    public static void Skip(float magnitude)
    {
        instance.transform.position += new Vector3(Time.deltaTime * magnitude * skipMultiplier, 0, 0);
    }



    ////////////////////////////////////////////////////////////////
    // Unity Methods
    ////////////////////////////////////////////////////////////////

    protected virtual void Start ()
    {
        // Find self and camera
        instance = GameObject.FindGameObjectsWithTag("God")[0].GetComponent<God>();
        this.gameCamera = this.GetComponentInChildren<Camera>();

        // Spawn first two pairs of boundaries
        this.leftBoundaries = SpawnBoundary(0);
        this.rightBoundaryX = boundaryStats.x;
        this.rightBoundaries = SpawnBoundary(rightBoundaryX);

        // Generate spawn probabilities for obstacles and consumable objects based on their stats
        for (int i = 0; i < obstacleStats.Length; i++)
        {
            for (int j = 0; j < obstacleStats[i].SpawnProb; j++)
            {
                obstacleSpawnProbs.Add(i);
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

	protected virtual void Update()
    {
        if (!Stopped && Menu.InPlay)
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

            // Spawn the next environment object
            if (this.generateLevel && this.transform.position.x > this.nextEnv.x - envXLead)
            {
                this.SpawnNextEnv();
            }
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        // Destroy environment objects on collision (since they are out of player view)
        if (collision.gameObject.GetComponent<Environment>() != null && 
            collision.gameObject.GetComponent<Environment>().EnvironmentType != EnvironmentType.Boundary)
        {
            RemoveEnvironmentObj(collision.gameObject);
        }
    }



    ////////////////////////////////////////////////////////////////
    // Protected and Private Methods
    ////////////////////////////////////////////////////////////////

    /// <summary>
    /// Spawns a pair of boundaries (sky and ground) at a specified X positon
    /// </summary>
    /// <param name="x">The X position at which to spawn the boundaries</param>
    /// <returns>The pair of spawned boundaries</returns>
    private GameObject[] SpawnBoundary(float x)
    {
        GameObject[] boundaries = new GameObject[2];
        boundaries[0] = Instantiate(Ground, new Vector3(x, -boundaryStats.y, boundaryStats.z), Quaternion.identity);
        boundaries[1] = Instantiate(Sky, new Vector3(x, boundaryStats.y, boundaryStats.z), Quaternion.identity);
        return boundaries;
    }
    
    /// <summary>
    /// Spawns the next environment object at nextEnv
    /// </summary>
    private void SpawnNextEnv()
    {
        // Randomly choose if we should spawn a consumable object
        if (Random.value < consumableProb)
        {
            // Randomly choose a consumable to spawn from consumableSpawnProbs
            int index = this.consumableSpawnProbs[(int)(Random.value * this.consumableSpawnProbs.Count)];
            this.SpawnConsumable(index, new Vector3(this.nextEnv.x, Random.Range(-envMaxY, envMaxY), this.nextEnv.z));
            this.nextEnv.x += ConsumableStats.XDelay;
        }
        else
        {
            // Randomly choose an obstacle to spawn from obstacleSpawnProbs
            int index = this.obstacleSpawnProbs[(int)(Random.value * this.obstacleSpawnProbs.Count)];
            ObstacleStats stats = obstacleStats[index];

            // Calculate the minimum and maximum Y of the ideal path based on other obstacles blocking at this X position
            float minY = -envMaxY;
            float maxY = envMaxY;
            for (int i = 0; i < this.curYBlocks.Count; ++i)
            {
                // Remove a Y block if the ending X position has been passed
                if (this.nextEnv.x > this.curYBlocks[i].x)
                {
                    this.curYBlocks.RemoveAt(i);
                    i--;
                }
                else if (this.curYBlocks[i].y < this.nextEnv.y)
                {
                    minY = Mathf.Max(minY, this.curYBlocks[i].y);
                }
                else
                {
                    maxY = Mathf.Min(maxY, this.curYBlocks[i].y);
                }
            }
            minY += playerSpace;
            maxY -= playerSpace;

            // Use a Markov model to choose the next Y position of the ideal path
            this.nextEnv.y = Utilities.Markov(this.nextEnv.y, ySdev.GetValue(this.DifficultyMultiplier), minY, maxY);

            // Spawn the obstacle only if there is enough space to do so
            if (maxY - minY > stats.YOffset * 2)
            {
                // Calculate the offset of the current obstacle to leave room on the ideal path
                float yOffset = (Random.value > 0.5f) ? stats.YOffset : -stats.YOffset;
                if (yOffset < minY)
                {
                    yOffset += 2 * stats.YOffset;
                }
                else if (yOffset > maxY)
                {
                    yOffset -= 2 * stats.YOffset;
                }

                this.SpawnObstaclePre(index, this.nextEnv + Vector3.up * yOffset);
            }

            this.nextEnv.x += stats.XDelay.GetValue(this.DifficultyMultiplier);
        }
    }

    /// <summary>
    /// Spawns an obstacle type at a given position including handling obstacles consisting of multiple objects
    /// </summary>
    /// <param name="index">The Obstacle index of the obstacle type to be spawned</param>
    /// <param name="position">The position at which to center the obstacle type</param>
    /// <param name="forceStatic">Forces obstacle to either move (true) or not move (false)</param>
    protected void SpawnObstaclePre(int index, Vector3 position, bool? forceMove = null)
    {
        ObstacleStats stats = obstacleStats[index];
        bool moving = ((forceMove ?? true) && Random.value < stats.MovementProb.GetValue(this.DifficultyMultiplier)) || (forceMove ?? false);

        // For pipes, spawn two obstacles (top and bottom) and a pipe score in between
        if (index == Obstacles.Pipe.GetHashCode())
        {
            float spacing = stats.YGap.GetValue(DifficultyMultiplier);
            this.SpawnObstacle(index, position + Vector3.down * spacing, moving);
            this.SpawnObstacle(index, position + Vector3.up * spacing, moving, Quaternion.Euler(0, 0, 180));
            this.SpawnConsumable(Consumables.Pipe.GetHashCode(), position);
        }
        else
        {
            this.SpawnObstacle(index, position, moving);
        }
    }

    /// <summary>
    /// Spawns a particular obstacle with the specified parameters
    /// </summary>
    /// <param name="index">The Obstacles index of the object to be spawned</param>
    /// <param name="position">The position at which to spawn the obstacle</param>
    /// <param name="moving">Whether the object should move</param>
    /// <param name="rotation">The amount to rotate the object</param>
    protected void SpawnObstacle(int index, Vector3 position, bool moving, Quaternion rotation = new Quaternion())
    { 
        // Spawn the specified obstacle at the specified position and rotation
        GameObject newObstacle = Instantiate(this.ObstaclePrefabs[index], position, rotation);

        // Set the obstacle's public properties based on its stats in obstacleStats
        ObstacleStats stats = obstacleStats[index];
        Environment newObst = newObstacle.GetComponent<Environment>();
        newObst.EnvironmentType = EnvironmentType.Obstacle; 
        
        // Make the object move first based on forceMove (if not null) and otherwise based on a random probability
        if (moving)
        {
            newObst.Movement = stats.Movement;
            newObst.Speed = stats.Speed.GetValue(this.DifficultyMultiplier);
        }

        // If the obstacle blocks its Y position, add its top and bottom to curYBlocks
        if (stats.YBlock > 0)
        {
            this.curYBlocks.Add(new Vector2(position.x + stats.YBlock + playerSpace * 2, position.y + stats.YOffset));
            this.curYBlocks.Add(new Vector2(position.x + stats.YBlock + playerSpace * 2, position.y - stats.YOffset));
        }
    }

    /// <summary>
    /// Spawns a particular consumable object with the specified parameters
    /// </summary>
    /// <param name="index">The Consumables index of the object to be spawned</param>
    /// <param name="position">The position at which to spawn the obstacle</param>
    /// <param name="rotation">The amount to rotate the object</param>
    protected void SpawnConsumable(int index, Vector3 position, Quaternion rotation = new Quaternion())
    {
        // Spawn the specified consumable at the specified position and rotation
        GameObject newConsumable = Instantiate(this.ConsumablePrefabs[index], position, rotation);

        // Set the consumable object's public properties based on its stats in consumableStats
        ConsumableStats stats = consumableStats[index];
        Environment newObst = newConsumable.GetComponent<Environment>();
        newObst.EnvironmentType = EnvironmentType.Consumable;
        newObst.Score = stats.Score;
        newObst.Force = stats.Force;
        newObst.SpeedMultiplier = stats.SpeedMultiplier;
        newObst.Consumable = (Consumables)index;
    }
}
