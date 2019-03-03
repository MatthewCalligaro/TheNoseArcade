using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnvironmentStats
{
    /// <summary>
    /// X distance before the next environment object is spawned
    /// </summary>
    public VariableDistance XDelay { get; set; }

    /// <summary>
    /// Y distance between multiple of these environment objects at the same x position
    /// </summary>
    public VariableDistance YGap { get; set; }

    /// <summary>
    /// Maximum y position
    /// </summary>
    public float YMax { get; set; }

    /// <summary>
    /// Distance for which future environment objects cannot be spawned at this y position
    /// </summary>
    public float YBlock { get; set; }
}
