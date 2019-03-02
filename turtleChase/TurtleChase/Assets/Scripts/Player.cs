using UnityEngine;

public class Player : MonoBehaviour
{
    public const float JumpVelocity = 6;
    public const float JumpForce = 500;
    public const float JumpJetpack = 500;

    private int score = 0;

    private bool jump = false;

    public void Jump ()
    {
        jump = true;
    }

	
	private void Update ()
    {
        this.transform.Translate(new Vector3(God.ScrollSpeed * Time.deltaTime, 0, 0));
        if (jump || Input.GetKeyDown(KeyCode.Space))
        {
            jump = false;
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
            this.GetComponent<Rigidbody2D>().AddForce(new Vector2(0, JumpForce * Time.deltaTime));
        }

        if (Input.GetKeyDown(KeyCode.Escape))
        {
            PauseMenu.Pause();
        }

        if (Settings.PlayStyle == PlayStyle.Runner && !God.InCamera(this.transform.position))
        {
            this.HandleLoss();
        }
	}

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.GetComponent<Obstacle>())
        {
            switch (collision.gameObject.GetComponent<Obstacle>().ObstacleType)
            {
                case ObstacleType.Environment:
                    if (Settings.PlayStyle == PlayStyle.Classic) this.HandleLoss();
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
