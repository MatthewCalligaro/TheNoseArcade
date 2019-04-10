using System;
using UnityEngine;
using UnityEngine.SceneManagement;

/// <summary>
/// A nose controller used to calibrate settings at the begining of the game
/// </summary>
public class Calibration : MonoBehaviour, INoseController
{
    /// <summary>
    /// Calibrated sensitivity is multiplied by this factor to produce final sensitivity
    /// </summary>
    private const float calibrationFactor = 0.75f;

    /// <summary>
    /// Messages shown to walk the user through calibrations
    /// </summary>
    private static readonly string[] messages = 
    {
        "Please adjust your position so that your nose is centered and within 18 inches of your webcam.  When you are ready to begin calibration, press the space bar.",
        "In Turtle Chase, you jump by raising your nose.  It is easiest for us to detect large nose movements, so we recommend performing each jump by pointing your nose downward and then upward, as though you went from looking at the ground to looking at the ceiling.\n\n Please perform one jump now and press the space bar.",
        "Please perform another jump and press the space bar (1/3 complete).",
        "Please perform another jump and press the space bar (2/3 complete).",
    };

    /// <summary>
    /// The current stage of calibration
    /// </summary>
    private int curStage = 0;

    /// <summary>
    /// The Y pixel position of the last recieved face position
    /// </summary>
    private int lastY;
    
    /// <summary>
    /// The time at which the last face position was recieved
    /// </summary>
    private DateTime lastTime;

    /// <summary>
    /// The maximum upward vertical change in face position in pixels per millisecond for this round of calibration
    /// </summary>
    private float maxDy = 0;

    /// <summary>
    /// Stores the running total of the maxDy calculated each round of calibration
    /// </summary>
    private float maxDyTotal;



    ////////////////////////////////////////////////////////////////
    // Methods
    ////////////////////////////////////////////////////////////////

    /// <summary>
    /// Updates the controller with the current user's face position
    /// </summary>
    /// <param name="packed">32 bit signed int with the 16 least significant bits representing the x pixel and the 16 most significant bits representing the y pixel</param>
    public void UpdateFacePosition(int packed)
    {
        int y = (packed >> 16) & 0xFFFF;
        DateTime now = DateTime.Now;
        this.maxDy = Mathf.Max((y - lastY) / ((float)(now - lastTime).TotalMilliseconds), this.maxDy);

        this.lastY = y;
        this.lastTime = now;
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            ProgressCalibration();
        }

        // FOR DEBUGGING - the mouse position is treated as the face position when shift is held down
        if (Input.GetKey(KeyCode.LeftShift) || Input.GetKey(KeyCode.RightShift))
        {
            UpdateFacePosition((((int)Input.mousePosition.x) & 0xFFFF) | ((((int)Input.mousePosition.y) & 0xFFFF) << 16));
        }
    }

    private void Start()
    {
        CalibrationUI.UpdateMessage(messages[this.curStage]);
    }

    /// <summary>
    /// Progresses to the next stage of calibration
    /// </summary>
    private void ProgressCalibration()
    {
        if (this.curStage > 0)
        {
            this.maxDyTotal += this.maxDy;
            this.maxDy = 0;
        }

        this.curStage++;
        if (this.curStage >= messages.Length)
        {
            Settings.Sensitivity = (this.maxDyTotal / 3) * calibrationFactor;
            Debug.Log(Settings.Sensitivity);
            SceneManager.LoadScene("MainMenu");
        }
        else
        {
            CalibrationUI.UpdateMessage(messages[this.curStage]);
        }
    }
}
