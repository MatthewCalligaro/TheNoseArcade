using UnityEngine;

public class Player : MonoBehaviour
{
    private const float jumpVelocity = 6;
    private const float jumpForce = 500;
    private const float jumpJetpackForce = 1500;

    private int score = 0;
    private bool isJetpacking = false;
	
    public void JumpDiscrete()
    {
        if (Settings.JumpStyle == JumpStyle.Velocity)
        {
            this.GetComponent<Rigidbody2D>().velocity = new Vector2(0, jumpVelocity);
        }
        else
        {
            this.GetComponent<Rigidbody2D>().AddForce(new Vector2(0, jumpForce));
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
            this.GetComponent<Rigidbody2D>().velocity = new Vector2(0, jumpVelocity * magnitude);
        }
        else
        {
            this.GetComponent<Rigidbody2D>().AddForce(new Vector2(0, jumpForce * magnitude));
        }
    }

	private void Update ()
    {
        this.transform.Translate(new Vector3(God.ScrollSpeed * Time.deltaTime, 0, 0));

        if (isJetpacking)
        {
            this.GetComponent<Rigidbody2D>().AddForce(new Vector2(0, jumpJetpackForce * Time.deltaTime));
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

        if (Input.GetKeyDown(KeyCode.Escape))
        {
            PauseMenu.Pause();
        }

        if (Settings.PlayStyle == PlayStyle.Chase && !God.InCamera(this.transform.position))
        {
            this.HandleLoss();
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

    private void HandleCollision(GameObject other)
    {
        if (other.gameObject.GetComponent<Obstacle>())
        {
            switch (other.gameObject.GetComponent<Obstacle>().ObstacleType)
            {
                case ObstacleType.Environment:
                    if (Settings.PlayStyle == PlayStyle.Classic) this.HandleLoss();
                    break;

                case ObstacleType.Score:
                    this.score += other.gameObject.GetComponent<Obstacle>().Score;
                    HUD.UpdateScore(this.score);
                    God.RemoveEnvironmentObj(other.gameObject);
                    break;

                case ObstacleType.Death:
                    this.HandleLoss();
                    break;
            }
        }
    }

    private void HandleLoss()
    {
        LossMenu.HandleLoss(this.score);
    }
}
