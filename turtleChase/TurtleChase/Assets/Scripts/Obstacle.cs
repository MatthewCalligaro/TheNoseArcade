using UnityEngine;

public enum ObstacleType
{
    Environment,
    Death
}

public class Obstacle : MonoBehaviour
{
    public ObstacleType ObstacleType;
}
