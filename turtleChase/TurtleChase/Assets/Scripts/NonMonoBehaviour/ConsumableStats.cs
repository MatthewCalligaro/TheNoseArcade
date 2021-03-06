﻿using UnityEngine;

/// <summary>
/// Organizes the statistics associated with a consumable object
/// </summary>
public class ConsumableStats
{
    /// <summary>
    /// X distance before the next obstacle is spawned
    /// </summary>
    public const float XDelay = 1.0f;

    /// <summary>
    /// Relative frequency with which this consumable object is spawned
    /// </summary>
    public int SpawnProb { get; set; }

    /// <summary>
    /// Score which the player recieves by consuming this object
    /// </summary>
    public int Score { get; set; }

    /// <summary>
    /// Speed multiplier which the player recieves by consuming this object
    /// </summary>
    public float SpeedMultiplier { get; set; }

    /// <summary>
    /// Force vector which is exerted on the player when they consume this object
    /// </summary>
    public Vector2 Force { get; set; }
}
