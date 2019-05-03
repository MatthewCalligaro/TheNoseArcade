using System;
using UnityEngine;

public class Player : MonoBehaviour
{
    /// <summary>
    /// Total play time that has elapsed in the current race
    /// </summary>
    private TimeSpan ElapsedTime
    {
        get
        {
            return DateTime.Now - startTime - PauseMenu.TimePaused;
        }
    }

    /// <summary>
    /// Maximum acceleration torque the engine can apply to each wheel
    /// </summary>
    private const float maxTorque = 400;

    /// <summary>
    /// Maximum braking torque that the engine can apply to each wheel 
    /// </summary>
    private const float maxBrakeTorque = 1000;

    /// <summary>
    /// Maximum angle which the front wheels can turn from straight
    /// </summary>
    private const float maxSteerAngle = 20;

    /// <summary>
    /// The number of laps in the race
    /// </summary>
    private const int maxLaps = 2;

    /// <summary>
    /// The time at the first frame of the race
    /// </summary>
    private DateTime startTime;

    /// <summary>
    /// The lap which the player is currently on
    /// </summary>
    private int laps = 0;

    /// <summary>
    /// The WheelColliders defining the physics of the car's wheels
    /// </summary>
    WheelCollider[] wheels;

    /// <summary>
    /// The game camera which follows the car
    /// </summary>
    private Camera gameCamera;



    ////////////////////////////////////////////////////////////////
    // Unity Methods
    ////////////////////////////////////////////////////////////////

    private void Start ()
    {
        this.gameCamera = GameObject.FindGameObjectsWithTag("MainCamera")[0].GetComponent<Camera>();

        this.startTime = DateTime.Now;
        this.wheels = this.GetComponentsInChildren<WheelCollider>();
        this.GetComponent<Rigidbody>().centerOfMass = new Vector3(0, -0.2f, -0.3f);

        // Configure wheel settings to prevent car "jittering"
        foreach (WheelCollider wheel in wheels)
        {
            wheel.ConfigureVehicleSubsteps(1, 15, 20);
        }
	}

    private void FixedUpdate()
    { 
        // Apply acceleration, braking, and turning to wheels
        float motorTorque = Mathf.Max(0, Settings.Accelerate.Interpolate(Controller.Cursor.y) * maxTorque);
        float brakeTorque = Mathf.Max(0, Settings.Brake.Interpolate(Controller.Cursor.y) * maxBrakeTorque);
        float steerAngle = (Settings.Right.Interpolate(Controller.Cursor.x) - Settings.Left.Interpolate(Controller.Cursor.x)) * maxSteerAngle
            / Math.Max(1, Settings.SteerVelocityFactor * this.GetComponent<Rigidbody>().velocity.magnitude);

        for (int i = 0; i < 2; i++)
        {
            wheels[i].motorTorque = motorTorque;
            wheels[i].brakeTorque = brakeTorque;
            wheels[i].steerAngle = steerAngle;
        }

        // Have the game camera follow the car
        gameCamera.transform.position = this.transform.position + Settings.CameraOffset.z * this.transform.forward + Settings.CameraOffset.y * Vector3.up;
        gameCamera.transform.rotation = Quaternion.Euler(Settings.CameraRotationX, this.transform.rotation.eulerAngles.y, 0);
    }

    private void Update()
    {
        HUD.UpdateSpeed(this.GetComponent<Rigidbody>().velocity.magnitude);
        HUD.UpdateTime(this.ElapsedTime);
        HUD.UpdateLaps(this.laps, maxLaps);
        
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            PauseMenu.TogglePauseMenu();
        }
    }

    private void OnTriggerEnter(Collider collider)
    {
        // Handle when we complete a lap
        if (collider.GetComponent<FinishLine>() != null)
        {
            this.laps++;
            if (this.laps > maxLaps)
            {
                WinMenu.HandleWin(this.ElapsedTime);
            }
            else
            {
                HUD.UpdateLaps(this.laps, maxLaps);
            }
        }
    }
}
