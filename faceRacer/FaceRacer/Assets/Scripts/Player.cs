using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    private TimeSpan elapsedTime
    {
        get
        {
            return DateTime.Now - startTime - PauseMenu.TimePaused;
        }
    }

    private const float acceleration = 10;
    private const float decayAcceleration = 2;
    private const float brakeAcceleration = 30;
    private const float turnSpeed = 90;

    private const int maxLaps = 2;

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
        if (Menu.InPlay)
        {
            this.UpdateKinetics();
            HUD.UpdateSpeed(this.velocity);
            HUD.UpdateTime(this.elapsedTime);
            HUD.UpdateLaps(this.laps, maxLaps);
        }
        
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            PauseMenu.TogglePauseMenu();
        }
    }

    private void OnTriggerEnter(Collider collider)
    {
        if (collider.GetComponent<FinishLine>() != null)
        {
            this.laps++;
            if (this.laps > maxLaps)
            {
                WinMenu.HandleWin(this.elapsedTime);
            }
            else
            {
                HUD.UpdateLaps(this.laps, maxLaps);
            }
        }
    }

    private void UpdateKinetics()
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
