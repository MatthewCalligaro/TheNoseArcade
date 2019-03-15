using UnityEngine;

/// <summary>
/// Controls the player and player score
/// </summary>
public class Player : MonoBehaviour
{
    /// <summary>
    /// Number of UI elements currently preventing pause
    /// </summary>
    public static int BlockingPause = 0;

    /// <summary>
    /// Property to set a new multiplier to the player's speed
    /// </summary>
    public float SpeedMultiplier
    {
        get
        {
            return this.speedMultiplier;
        }
        set
        {
            this.speedMultiplier = value;
            this.speedCounter = speedMultiplierTime;
        }
    }



    ////////////////////////////////////////////////////////////////
    // Private Constants
    ////////////////////////////////////////////////////////////////

    /// <summary>
    /// Upward velocity after jumping for JumpStyle.Velocity
    /// </summary>
    private const float jumpVelocity = 8;

    /// <summary>
    /// Upward force applied from a jump for JumpStyle.Force
    /// </summary>
    private const float jumpForce = 500;

    /// <summary>
    /// Upward force applied per second for JumpStyle.Jetpack
    /// </summary>
    private const float jumpJetpackForce = 1500;

    /// <summary>
    /// Time for which a speed multiplier lasts
    /// </summary>
    private const float speedMultiplierTime = 1.5f;

    /// <summary>
    /// The maximum percentage of the screen width that the player can get ahead
    /// </summary>
    private const float maxScreenX = 0.75f;



    ////////////////////////////////////////////////////////////////
    // Private Fields
    ////////////////////////////////////////////////////////////////

    /// <summary>
    /// Current player score
    /// </summary>
    private int score = 0;

    /// <summary>
    /// True if the player is receiving an upward force from jumping with JumpStyle.Jetpack
    /// </summary>
    private bool isJetpacking = false;

    /// <summary>
    /// True if the current game has been lost, preventing any player action
    /// </summary>
    private bool loss = false;

    /// <summary>
    /// Current multiplier applied to the player's speed
    /// </summary>
    private float speedMultiplier = 1.0f;

    /// <summary>
    /// Time remaining for the current speed multiplier
    /// </summary>
    private float speedCounter = 0;



    ////////////////////////////////////////////////////////////////
    // Public Methods
    ////////////////////////////////////////////////////////////////

    /// <summary>
    /// Handles when the player recieves the command to jump
    /// </summary>
    /// <param name="magnitude"></param>
    public void JumpEnter(float magnitude = 1.0f)
    {
        switch (Settings.JumpStyle)
        {
            case JumpStyle.Velocity:
                this.GetComponent<Rigidbody2D>().velocity = new Vector2(0, jumpVelocity * Settings.JumpPower * magnitude);
                break;

            case JumpStyle.Force:
                this.GetComponent<Rigidbody2D>().AddForce(new Vector2(0, jumpForce * Settings.JumpPower * magnitude));
                break;

            // Jetpack does not use magnitude
            case JumpStyle.Jetpack:
                this.isJetpacking = true;
                break;
        }
        else
        {
            this.isJetpacking = true;
        }
    }

    /// <summary>
    /// Handles when the player stops recieving the command to jump
    /// </summary>
    public void JumpExit()
    {
        this.isJetpacking = false;
    }

    /// <summary>
    /// Applies a force vector to the player
    /// </summary>
    /// <param name="force">The force vector ot apply to the player</param>
    public void ApplyForce(Vector2 force)
    {
        this.GetComponent<Rigidbody2D>().AddForce(force);
    }



    ////////////////////////////////////////////////////////////////
    // Unity Methods
    ////////////////////////////////////////////////////////////////

    private void Start()
    {
        BlockingPause = 0;
        HUD.UpdateScore(this.score);
    }

    private void Update()
    {
        if (!this.loss)
        {
            // Remove the speedMultiplier when the timer expires
            if (this.speedCounter > 0)
            {
                this.speedCounter -= Time.deltaTime;
                if (this.speedCounter <= 0)
                {
                    this.speedMultiplier = 1.0f;
                    this.speedCounter = 0;
                }
            }

            // Use space bar to jump
            if (Input.GetKeyDown(KeyCode.Space))
            {
                this.JumpEnter();
            }
            if (Input.GetKeyUp(KeyCode.Space))
            {
                this.JumpExit();
            }

            // Use escape to pause
            if (Input.GetKeyDown(KeyCode.Escape) && BlockingPause <= 0)
            {
                PauseMenu.Pause();
            }

            // The player loses if they fall outside of the camera
            if (!God.InCamera(this.transform.position))
            {
                this.HandleLoss();
            }

            // Tell the camera to skip ahead if the player surpasses maxScreenX
            if (God.CameraCoords(this.transform.position).x > maxScreenX)
            {
                God.Skip(1 / (1 - God.CameraCoords(this.transform.position).x));
            }

            // Apply upward jetpack force if the player is currently jetpacking
            if (this.isJetpacking)
            {
                this.GetComponent<Rigidbody2D>().AddForce(new Vector2(0, jumpJetpackForce * Time.deltaTime * Settings.JumpPower));
            }

            // Move forward with the current speed multiplier
            this.transform.Translate(new Vector3(God.ScrollSpeed * this.SpeedMultiplier * Time.deltaTime, 0, 0));
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        this.HandleCollision(collision.gameObject);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        this.HandleCollision(collision.gameObject);
    }



    ////////////////////////////////////////////////////////////////
    // Private Methods
    ////////////////////////////////////////////////////////////////

    /// <summary>
    /// Handles collisions with any type of game object
    /// </summary>
    /// <param name="other">The game object with which the player collided</param>
    private void HandleCollision(GameObject other)
    {
        // If other is a consumable object, apply its relevant stats and remove it
        if (other.gameObject.GetComponent<Environment>() && other.gameObject.GetComponent<Environment>().EnvironmentType == EnvironmentType.Consumable)
        {
            Environment otherObs = other.gameObject.GetComponent<Environment>();
            this.score += otherObs.Score;
            HUD.UpdateScore(this.score);
            this.GetComponent<Rigidbody2D>().AddForce(otherObs.Force);
            this.SpeedMultiplier *= otherObs.SpeedMultiplier;
            God.RemoveEnvironmentObj(other.gameObject);
        }
    }

    /// <summary>
    /// Handles when the player loses
    /// </summary>
    private void HandleLoss()
    {
        this.loss = true;
        LossMenu.HandleLoss(this.score);
    }
}
