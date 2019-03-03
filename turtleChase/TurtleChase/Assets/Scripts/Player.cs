using UnityEngine;

public class Player : MonoBehaviour
{
    public const float JumpVelocity = 6;
    public const float JumpForce = 500;
    public const float JumpJetpack = 1500;

    private int score = 0;
	
	private void Update ()
    {
        this.transform.Translate(new Vector3(God.ScrollSpeed * Time.deltaTime, 0, 0));
        if (Input.GetKeyDown(KeyCode.Space))
        {
            switch(Settings.JumpStyle)
            {
                case JumpStyle.Velocity:
                    this.GetComponent<Rigidbody2D>().velocity = new Vector2(0, JumpVelocity);
                    break;

                case JumpStyle.Force:
                    this.GetComponent<Rigidbody2D>().AddForce(new Vector2(0, JumpForce));
                    break;
            }
        }

        if (Settings.JumpStyle == JumpStyle.Jetpack && Input.GetKey(KeyCode.Space))
        {
            this.GetComponent<Rigidbody2D>().AddForce(new Vector2(0, JumpJetpack * Time.deltaTime));
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
