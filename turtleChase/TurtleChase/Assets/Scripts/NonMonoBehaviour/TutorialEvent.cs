using UnityEngine;

public class TutorialEvent
{
    public string Text { get; set; }

    public TutorialTask Task { get; set; }

    public int TaskNum { get; set; }

    public float Distance { get; set; }

    public int[] ObstacleIndices { get; set; }

    public Vector3[] ObstaclePositions { get; set; }

    public int[] ConsumableIndices { get; set; }

    public Vector3[] ConsumablePositions { get; set; }
}
