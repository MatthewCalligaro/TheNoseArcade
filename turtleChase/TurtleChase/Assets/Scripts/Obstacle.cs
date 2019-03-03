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
    public Vector3 Movement;

    public float Speed { get; set; }

    private Vector3 startPosition;
    private int direction = 1;

    private void Start()
    {
        startPosition = this.transform.position;
    }

    private void Update()
    {
        if (this.Speed > 0 && Movement != Vector3.zero)
        {
            this.transform.Translate(Movement.normalized * direction * this.Speed * Time.deltaTime);

            Vector3 difference = direction * ((startPosition + direction * Movement) - this.transform.position);
            Debug.Log(difference);
            if (difference.x < 0 || difference.y < 0)
            {
                direction *= -1;
            }
        }
    }
}
