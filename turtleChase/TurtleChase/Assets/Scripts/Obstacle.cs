using UnityEngine;

/// <summary>
/// The different types of objects with which the player can interact
/// </summary>
public enum ObstacleType
{
    Boundary,
    Environment,
    Consumable,
}

/// <summary>
/// An object with which the player can interact
/// </summary>
public class Obstacle : MonoBehaviour
{
    ////////////////////////////////////////////////////////////////
    // Public Properties
    ////////////////////////////////////////////////////////////////

    /// <summary>
    /// The type of the object
    /// </summary>
    public ObstacleType ObstacleType { get; set; }

    /// <summary>
    ///  The score that the player recieves from colliding with the object
    /// </summary>
    public int Score { get; set; }

    /// <summary>
    /// The speed multiplier that the player recieves from colliding with the object
    /// </summary>
    public float SpeedMultiplier { get; set; }

    /// <summary>
    /// The force vector which the object exerts on the player when they collide
    /// </summary>
    public Vector2 Force { get; set; }

    /// <summary>
    /// The speed at which the object moves
    /// </summary>
    public float Speed { get; set; }

    /// <summary>
    /// A vector specifying the range of motion for the object
    /// </summary>
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



    ////////////////////////////////////////////////////////////////
    // Private Fields
    ////////////////////////////////////////////////////////////////

    private Vector3 movement;

    /// <summary>
    /// The position at which the object was spawned
    /// </summary>
    private Vector3 startPosition;

    /// <summary>
    /// The direction along Movement which the object is currently moving (either 1 or -1)
    /// </summary>
    private int direction = 1;



    ////////////////////////////////////////////////////////////////
    // Unity Methods
    ////////////////////////////////////////////////////////////////

    private void Start()
    {
        startPosition = this.transform.position;
    }

    private void Update()
    {
        // If given a speed and movement, move along movement at the given speed and direction 
        if (this.Speed > 0 && this.movement != Vector3.zero)
        {
            this.transform.position += this.movement.normalized * this.direction * this.Speed * Time.deltaTime;

            // Take the difference between the current position and the boundary position to determine if we need to change direction
            Vector3 difference = this.direction * ((this.startPosition + this.direction * this.movement) - this.transform.position);
            if (difference.x < 0 || difference.y < 0)
            {
                this.direction *= -1;
            }
        }
    }
}
