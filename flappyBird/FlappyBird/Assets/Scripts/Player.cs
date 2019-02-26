using UnityEngine;

public enum JumpStyle
{
    Velocity,
    Force
}

public class Player : MonoBehaviour
{
    public const float JumpVelocity = 6;
    public const float JumpForce = 500;
    public const JumpStyle Style = JumpStyle.Velocity;
	
	private void Update ()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            if (Style == JumpStyle.Velocity)
            {
                this.GetComponent<Rigidbody2D>().velocity = new Vector2(0, JumpVelocity);
            }
            else
            {
                this.GetComponent<Rigidbody2D>().AddForce(new Vector2(0, JumpForce));
            }
        }

        transform.Translate(new Vector3(God.ScrollSpeed * Time.deltaTime, 0, 0));
	}

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.GetComponent<Obstacle>())
        {
            print("Handle Collision");
        }
    }
}
