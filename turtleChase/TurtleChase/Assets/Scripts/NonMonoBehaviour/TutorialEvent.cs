using UnityEngine;

public class TutorialEvent
{
    public string Text { get; set; }

    public TutorialTask Task { get; set; }

    public int TaskNum { get; set; }

    public float? RequiredDistance { get; set; }

    public Consumables? RequiredConsumable { get; set; }

    public int[] ObstacleIndices { get; set; }

    public Vector3[] ObstaclePositions { get; set; }

    public bool ObstacleMovement { get; set; }

    public int? ConsumableIndex { get; set; }

    public bool ContinuousSpawn { get; set; }
}
