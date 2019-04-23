using System;
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

    private DateTime startTime;

    int laps = 0;



	private void Start ()
    {
        this.startTime = DateTime.Now;
	}

    private void Update()
    {
        if (Settings.Accelerate.Interpolate(Controller.Cursor.y) > 0)
        {
            this.Accelerate(Settings.Accelerate.Interpolate(Controller.Cursor.y));
        }
        else if (Settings.Brake.Interpolate(Controller.Cursor.y) > 0)
        {
            this.Brake(Settings.Brake.Interpolate(Controller.Cursor.y));
        }
        else
        {
            this.velocity = Mathf.Max(0, this.velocity - decayAcceleration * Time.deltaTime);
        }

        this.Turn(-Settings.Left.Interpolate(Controller.Cursor.x));
        this.Turn(Settings.Right.Interpolate(Controller.Cursor.x));

        this.transform.position += this.transform.forward * this.velocity * Time.deltaTime;

        HUD.UpdateSpeed(this.velocity);
        HUD.UpdateTime(DateTime.Now - this.startTime);
        HUD.UpdateLaps(this.laps + 1, 3);
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
