using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    private const float acceleration = 10;
    private const float decayAcceleration = 2;
    private const float brakeAcceleration = 30;
    private const float turnSpeed = 90;

    private float velocity = 0;

    private bool drifting = false;

    private Vector2 cursor = new Vector2(0.5f, 0.5f);
    private Vector3 lastMousePosition;

    public void UpdateCursor(float dx, float dy)
    {
        this.cursor.x = Mathf.Min(1, Mathf.Max(0, this.cursor.x + dx));
        this.cursor.y = Mathf.Min(1, Mathf.Max(0, this.cursor.y + dy));

        if (Settings.Accelerate.Interpolate(cursor.y) > 0)
        {
            this.Accelerate(Settings.Accelerate.Interpolate(cursor.y));
        }
        else if (Settings.Brake.Interpolate(cursor.y) > 0)
        {
            this.Brake(Settings.Brake.Interpolate(cursor.y));
        }
        else
        {
            this.velocity = Mathf.Max(0, this.velocity - decayAcceleration * Time.deltaTime);
        }

        this.Turn(-Settings.Left.Interpolate(cursor.x));
        this.Turn(Settings.Right.Interpolate(cursor.x));
    }

	private void Start ()
    {
        lastMousePosition = Input.mousePosition;
        HUD.UpdateCursor(this.cursor);
	}

    private void Update()
    {
        if (!Input.GetMouseButton(1))
        {
            UpdateCursor(
                (Input.mousePosition - this.lastMousePosition).x / Screen.width * Settings.MouseSensitivity.x,
                (Input.mousePosition - this.lastMousePosition).y / Screen.height * Settings.MouseSensitivity.y);
              HUD.UpdateCursor(this.cursor);
        }
        this.lastMousePosition = Input.mousePosition;

        this.transform.position += this.transform.forward * this.velocity * Time.deltaTime;
    }

    private void Accelerate(float magnitude)
    {
        this.velocity += magnitude * acceleration * Time.deltaTime;
    }

    private void Brake(float magnitude)
    {
        this.velocity = Mathf.Max(0, this.velocity - magnitude * brakeAcceleration * Time.deltaTime);
    }

    private void Turn(float magnitude)
    {
        this.transform.Rotate(0, magnitude * turnSpeed * Time.deltaTime, 0);
    }
}
