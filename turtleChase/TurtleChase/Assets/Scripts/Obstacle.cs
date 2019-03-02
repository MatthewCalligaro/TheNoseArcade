using UnityEngine;

public enum ObstacleType
{
    Boundary,
    Environment,
    Score,
    Death
}

public class Obstacle : MonoBehaviour
{
    public ObstacleType ObstacleType;
    public int Score;
}
