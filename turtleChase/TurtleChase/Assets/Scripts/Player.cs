using UnityEngine;

public class Player : MonoBehaviour
{
    public static int BlockingPause = 0;

    private const float jumpVelocity = 8;
    private const float jumpForce = 500;
    private const float jumpJetpackForce = 1500;
    private const float speedMultiplierTime = 1.5f;

    private int score = 0;
    private bool isJetpacking = false;

    private float speedMultiplier = 1.0f;
    private float speedCounter = 0;
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

    public void JumpDiscrete()
    {
        if (Settings.JumpStyle == JumpStyle.Velocity)
        {
            this.GetComponent<Rigidbody2D>().velocity = new Vector2(0, jumpVelocity * Settings.JumpPower);
        }
        else
        {
            this.GetComponent<Rigidbody2D>().AddForce(new Vector2(0, jumpForce * Settings.JumpPower));
        }
    }

    public void JumpContinuousEnter()
    {
        this.isJetpacking = true;
    }

    public void JumpContinuousExit()
    {
        this.isJetpacking = false;
    }

    public void JumpWithMagnitude(float magnitude)
    {
        if (Settings.JumpStyle == JumpStyle.Velocity)
        {
            this.GetComponent<Rigidbody2D>().velocity = new Vector2(0, jumpVelocity * magnitude * Settings.JumpPower);
        }
        else
        {
            this.GetComponent<Rigidbody2D>().AddForce(new Vector2(0, jumpForce * magnitude * Settings.JumpPower));
        }
    }

    public void ApplyForce(Vector2 force)
    {
        this.GetComponent<Rigidbody2D>().AddForce(force);
    }

    private void Start()
    {
        HUD.UpdateScore(this.score);
    }

    private void Update()
    {
        if (isJetpacking)
        {
            this.GetComponent<Rigidbody2D>().AddForce(new Vector2(0, jumpJetpackForce * Time.deltaTime * Settings.JumpPower));
        }

        if (speedCounter > 0)
        {
            speedCounter -= Time.deltaTime;
            if (speedCounter <= 0)
            {
                this.speedMultiplier = 1.0f;
                this.speedCounter = 0;
            }
        }

        if (Input.GetKeyDown(KeyCode.Space))
        {
            if (Settings.JumpStyle == JumpStyle.Jetpack)
            {
                this.JumpContinuousEnter();
            }
            else
            {
                this.JumpDiscrete();
            }
        }

        if (Input.GetKeyUp(KeyCode.Space) && Settings.JumpStyle == JumpStyle.Jetpack)
        {
            this.JumpContinuousExit();
        }

        if (Input.GetKeyDown(KeyCode.Escape) && BlockingPause <= 0)
        {
            PauseMenu.Pause();
        }

        if (!God.InCamera(this.transform.position))
        {
            this.HandleLoss();
        }

        if (God.CameraCoords(this.transform.position).x > 0.75f)
        {
            God.Skip(1 / (1 - God.CameraCoords(this.transform.position).x));
        }

        this.transform.Translate(new Vector3(God.ScrollSpeed * this.SpeedMultiplier * Time.deltaTime, 0, 0));
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        this.HandleCollision(collision.gameObject);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        this.HandleCollision(collision.gameObject);
    }

    private void HandleCollision(GameObject other)
    {
        if (other.gameObject.GetComponent<Obstacle>() && other.gameObject.GetComponent<Obstacle>().ObstacleType == ObstacleType.Consumable)
        {
            Obstacle otherObs = other.gameObject.GetComponent<Obstacle>();
            this.score += otherObs.Score;
            HUD.UpdateScore(this.score);
            this.GetComponent<Rigidbody2D>().AddForce(otherObs.Force);
            this.SpeedMultiplier *= otherObs.SpeedMultiplier;
            God.RemoveEnvironmentObj(other.gameObject);
        }
    }

    private void HandleLoss()
    {
        LossMenu.HandleLoss(this.score);
    }
}
