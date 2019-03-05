using UnityEngine;

public enum ObstacleType
{
    Boundary,
    Environment,
    Consumable,
}

public class Obstacle : MonoBehaviour
{
    public ObstacleType ObstacleType { get; set; }
    public int Score { get; set; }
    public float SpeedMultiplier { get; set; }
    public Vector2 Force { get; set; }
    public float Speed { get; set; }

    private Vector3 movement;
    public Vector2 Movement
    {
        get
        {
            return this.movement;
        }
        set
        {
            this.movement = new Vector3(Mathf.Abs(value.x), Mathf.Abs(value.y), 0);
            direction = (value.x < 0 || value.y < 0) ? -1 : 1;
        }
    }

    private Vector3 startPosition;
    private int direction = 1;

    private void Start()
    {
        startPosition = this.transform.position;
    }

    private void Update()
    {
        if (this.Speed > 0 && movement != Vector3.zero)
        {
            this.transform.position += this.movement.normalized * direction * this.Speed * Time.deltaTime;

            Vector3 difference = direction * ((startPosition + direction * movement) - this.transform.position);
            if (difference.x < 0 || difference.y < 0)
            {
                direction *= -1;
            }
        }
    }
}
