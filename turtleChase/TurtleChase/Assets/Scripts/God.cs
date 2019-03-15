using System.Collections.Generic;
using UnityEngine;

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
    // Enums
    ////////////////////////////////////////////////////////////////

    /// <summary>
    /// The obstacles which can be added to the level
    /// </summary>
    private enum Obstacles
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
    // Private Properties
    ////////////////////////////////////////////////////////////////

    private float DifficultyMultiplier
    {
        get
        {
            return this.transform.position.x * Settings.difficultyMultiers[Settings.Difficulty.GetHashCode()];
        }
    }


    ////////////////////////////////////////////////////////////////
    // Private Fields
    ////////////////////////////////////////////////////////////////

    /// <summary>
    /// X position at which the first environment object is spawned
    /// </summary>
    private const float firstEnvX = 20;

    /// <summary>
    /// Amount that environment objects spawn ahead of the player in the X direction
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
    /// X length, +/- Y position, and Z position of boundary objects
    /// </summary>
    private static readonly Vector3 boundaryStats = new Vector3(100, 4.5f, 1);

    /// <summary>
    /// Standard deviation for the Markov chain used to determine the Y position of environment objects
    /// </summary>
    private static readonly VariableValue ySdev = new VariableValue(1.5f, 4, 0.02f);

    /// <summary>
    /// Stats defining obstacles
    /// </summary>
    private static readonly ObstacleStats[] obstacleStats =
    {
        // Pipe
        new ObstacleStats
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
        new ObstacleStats
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
        new ObstacleStats
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
        new ObstacleStats
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
        new ObstacleStats
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
        new ObstacleStats
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
    /// Position of the next environment object to be spawned
    /// </summary>
    private Vector3 nextEnv = new Vector3(firstEnvX / Settings.difficultyMultiers[Settings.Difficulty.GetHashCode()], 0, envZ);

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
    private float consumableProb = 0.25f / Settings.difficultyMultiers[Settings.Difficulty.GetHashCode()];

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

    private void Start ()
    {
        // Find self and camera
        instance = GameObject.FindGameObjectsWithTag("GameController")[0].GetComponent<God>();
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

        // Spawn the next environment object
        if (this.transform.position.x > this.nextEnv.x - envXLead)
        {
            this.SpawnNextEnv();
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
            this.SpawnConsumable(index);
            this.nextEnv.x += ConsumableStats.XDelay;
        }
        else
        {
            // Randomly choose an obstacle to spawn from obstacleSpawnProbs
            int index = this.obstacleSpawnProbs[(int)(Random.value * this.obstacleSpawnProbs.Count)];
            ObstacleStats stats = obstacleStats[index];

            // Use a Markov model to choose the next Y position based on the current Y position
            this.nextEnv.y += Utilities.NormalDist(0, ySdev.GetValue(this.DifficultyMultiplier));

            // Adjust the Y position to make sure that it is within the YMax for this obstacle and does
            // not fall on a blocked Y position in curYBlocks
            int attempts = 0;
            do
            {
                this.nextEnv.y = this.nextEnv.y > stats.YMax ? 2 * stats.YMax - this.nextEnv.y : this.nextEnv.y;
                this.nextEnv.y = this.nextEnv.y < -stats.YMax ? 2 * -stats.YMax - this.nextEnv.y : this.nextEnv.y;
                attempts++;
            }
            while (!VerifyY(this.nextEnv.y) && attempts < 100);
            // TODO: prevent infinite looping with a more robust solution

            // For pipes, we always spawn two (top and bottom) and a pipe score in between
            if (index == Obstacles.Pipe.GetHashCode())
            {
                float spacing = stats.YGap.GetValue(DifficultyMultiplier);
                this.SpawnObstacle(index, -spacing);
                this.SpawnObstacle(index, spacing, Quaternion.Euler(0, 0, 180));
                this.SpawnConsumable(Consumables.Pipe.GetHashCode());
            }
            else
            {
                this.SpawnObstacle(index);
            }

            this.nextEnv.x += stats.XDelay.GetValue(this.DifficultyMultiplier);
        }
    }

    /// <summary>
    /// Spawn a particular obstacle with the specified parameters
    /// </summary>
    /// <param name="index">The Obstacles index of the object to be spawned</param>
    /// <param name="yOffset">The Y offset of the object from nextEnv.y</param>
    /// <param name="rotation">The amount to rotate the object</param>
    private void SpawnObstacle(int index, float yOffset = 0, Quaternion rotation = new Quaternion())
    { 
        // Spawn the specified obstacle at the specified position/rotation
        GameObject newObstacle = Instantiate(this.ObstaclePrefabs[index], this.nextEnv + new Vector3(0, yOffset, 0), rotation);

        // Set the obstacle's public properties based on its stats in obstacleStats
        ObstacleStats stats = obstacleStats[index];
        Environment newObst = newObstacle.GetComponent<Environment>();
        newObst.EnvironmentType = EnvironmentType.Obstacle;
        if (Random.value < stats.MovementProb.GetValue(this.DifficultyMultiplier))
        {
            newObst.Movement = stats.Movement;
            newObst.Speed = stats.Speed.GetValue(this.DifficultyMultiplier);
        }

        // If the obstacle blocks its Y position, add this to curYBlocks
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
            this.ConsumablePrefabs[index], 
            new Vector3(this.nextEnv.x, Random.Range(-ConsumableStats.YMax, ConsumableStats.YMax), this.nextEnv.z), 
            Quaternion.identity);

        // Set the consumable object's public properties based on its stats in consumableStats
        Environment newObst = newConsumable.GetComponent<Environment>();
        newObst.EnvironmentType = EnvironmentType.Consumable;
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
