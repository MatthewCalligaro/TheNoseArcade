using UnityEngine;

/// <summary>
/// Organizes the statistics associated with an obstacle
/// </summary>
public class ObstacleStats
{
    /// <summary>
    /// X distance before the next obstacle is spawned
    /// </summary>
    public VariableValue XDelay { get; set; }

    /// <summary>
    /// Y distance between multiple of these obstacles at the same X position
    /// </summary>
    public VariableValue YGap { get; set; }

    /// <summary>
    /// Minimum Y distance which obstacle must be placed away from the intended path
    /// </summary>
    public float YOffset { get; set; }

    /// <summary>
    /// Distance for which future environment objects cannot be spawned at this Y position
    /// </summary>
    public float YBlock { get; set; }

    /// <summary>
    /// Relative frequency with which this obstacle is spawned
    /// </summary>
    public int SpawnProb { get; set; }

    /// <summary>
    /// Probability that the obstacle moves
    /// </summary>
    public VariableValue MovementProb { get; set; }

    /// <summary>
    /// Maximum distance that this obstacle can move from its spawn location
    /// </summary>
    public Vector2 Movement { get; set; }

    /// <summary>
    /// Speed at which the obstacle moves
    /// </summary>
    public VariableValue Speed { get; set; }
}
