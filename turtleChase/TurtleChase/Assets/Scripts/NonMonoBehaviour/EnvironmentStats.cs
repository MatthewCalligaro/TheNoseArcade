using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnvironmentStats
{
    /// <summary>
    /// X distance before the next obstacle is spawned
    /// </summary>
    public VariableValue XDelay { get; set; }

    /// <summary>
    /// Y distance between multiple of these environment objects at the same x position
    /// </summary>
    public VariableValue YGap { get; set; }

    /// <summary>
    /// Maximum y position
    /// </summary>
    public float YMax { get; set; }

    /// <summary>
    /// Distance for which future environment objects cannot be spawned at this y position
    /// </summary>
    public float YBlock { get; set; }

    public int SpawnProb { get; set; }

    public VariableValue MovementProb { get; set; }

    public Vector2 Movement { get; set; }

    public VariableValue Speed { get; set; }
}
