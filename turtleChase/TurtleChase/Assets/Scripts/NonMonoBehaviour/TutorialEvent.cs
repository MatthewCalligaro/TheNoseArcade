using UnityEngine;

/// <summary>
/// Encapsulates the information defining a task which the player must complete in the tutorial
/// </summary>
public class TutorialEvent
{
    /// <summary>
    /// Text displayed at the top of the screen explaining the task
    /// </summary>
    public string Text { get; set; }

    /// <summary>
    /// Text to label the remaining number of times the task must be performed
    /// </summary>
    public string TaskCountText { get; set; }

    /// <summary>
    /// Represents the type of task to be completed
    /// </summary>
    public TutorialTask Task { get; set; }

    /// <summary>
    /// The number of times the task must be completed
    /// </summary>
    public int TaskNum { get; set; }

    /// <summary>
    /// For the Distance task, the distance which the player must travel
    /// </summary>
    public float? RequiredDistance { get; set; }

    /// <summary>
    /// For the Consumable task, the type of consumable which the player must collect
    /// </summary>
    public Consumables? RequiredConsumable { get; set; }

    /// <summary>
    /// The indices of obstacles to generate (if any)
    /// </summary>
    public int[] ObstacleIndices { get; set; }

    /// <summary>
    /// The position of obstacles to generate (if any)
    /// </summary>
    public Vector3[] ObstaclePositions { get; set; }

    /// <summary>
    /// Whether the obstacles should move
    /// </summary>
    public bool ObstacleMovement { get; set; }

    /// <summary>
    /// The index of the consumable to spawn
    /// </summary>
    public int? ConsumableIndex { get; set; }
}
