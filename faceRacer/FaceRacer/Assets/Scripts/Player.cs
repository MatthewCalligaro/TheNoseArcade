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

    private const float maxTorque = 400;
    private const float maxBrakeTorque = 1000;
    private const float maxSteerAngle = 20;

    private const int maxLaps = 2;

    private bool drifting = false;

    private DateTime startTime;

    int laps = 0;

    float brakeTorque = 0;
    float motorTorque = 0;
    float steerAngle = 0;

    WheelCollider[] wheels;

    private Camera gameCamera;



	private void Start ()
    {
        this.gameCamera = GameObject.FindGameObjectsWithTag("MainCamera")[0].GetComponent<Camera>();

        this.startTime = DateTime.Now;
        this.wheels = this.GetComponentsInChildren<WheelCollider>();
        this.GetComponent<Rigidbody>().centerOfMass = new Vector3(0, -0.2f, -0.3f);
        foreach (WheelCollider wheel in wheels)
        {
            wheel.ConfigureVehicleSubsteps(1, 15, 20);
        }
	}

    private void FixedUpdate()
    {
        this.motorTorque = Mathf.Max(0, Settings.Accelerate.Interpolate(Controller.Cursor.y) * maxTorque);
        this.brakeTorque = Mathf.Max(0, Settings.Brake.Interpolate(Controller.Cursor.y) * maxBrakeTorque);
        this.steerAngle = (Settings.Right.Interpolate(Controller.Cursor.x) - Settings.Left.Interpolate(Controller.Cursor.x)) * maxSteerAngle
            / Math.Max(1, Settings.SteerVelocityFactor * this.GetComponent<Rigidbody>().velocity.magnitude);

        for (int i = 0; i < 2; i++)
        {
            wheels[i].motorTorque = this.motorTorque;
            wheels[i].brakeTorque = this.brakeTorque;
            wheels[i].steerAngle = this.steerAngle;
        }

        gameCamera.transform.position = this.transform.position + Settings.CameraOffset.z * this.transform.forward + Settings.CameraOffset.y * Vector3.up;
        gameCamera.transform.rotation = Quaternion.Euler(Settings.CameraRotationX, this.transform.rotation.eulerAngles.y, 0);
    }

    private void Update()
    {
        if (Menu.InPlay)
        {
            HUD.UpdateSpeed(this.GetComponent<Rigidbody>().velocity.magnitude);
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
}
