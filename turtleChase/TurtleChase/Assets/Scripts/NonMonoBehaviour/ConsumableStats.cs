using UnityEngine;

public class ConsumableStats
{
    /// <summary>
    /// Maximum y position
    /// </summary>
    public const float YMax = 3.5f;

    /// <summary>
    /// X distance before the next obstacle is spawned
    /// </summary>
    public const float XDelay = 1.0f;

    public int SpawnProb { get; set; }
    public int Score { get; set; }
    public float SpeedMultiplier { get; set; }
    public Vector2 Force { get; set; }
}
