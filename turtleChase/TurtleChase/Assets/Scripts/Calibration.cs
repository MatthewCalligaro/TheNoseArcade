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
    private const float calibrationFactor = 0.6f;

    /// <summary>
    /// First stage in which user movement is used for calibration
    /// </summary>
    private const int firstCalibrationStage = 2;

    /// <summary>
    /// Messages shown to walk the user through calibration
    /// </summary>
    private static readonly string[] messages = 
    {
        "Please move your face within 12 inches of your webcam and center your nose in the webcam feed to the right.  A green dot should appear over your nose.\nPress space to continue.",
        "In Turtle Chase, you jump by raising your nose.  This works best if you face directly towards the webcam at all times and change the position of your face rather than rotating it.\nPress space when you are ready to begin calibration.",
        "Please perform one jump by raising your nose, then press the space bar (0/3 complete).",
        "Please perform another jump, then press the space bar (1/3 complete).",
        "Please perform another jump, then press the space bar (2/3 complete)."
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
    /// <param name="packed">32 bit signed int with bits 9-18 representing the x pixel and bits 19-28 representing the y pixel</param>
    public void UpdateFacePosition(int packed)
    {
        // Update maxDy with the most recent change in y if it was bigger
        int y = (packed >> 19) & 0x3FF;
        DateTime now = DateTime.Now;
        float dy = (y - lastY) / ((float)(now - lastTime).TotalMilliseconds);
        this.maxDy = Mathf.Max(dy, this.maxDy);

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
        if (this.curStage >= firstCalibrationStage)
        {
            this.maxDyTotal += this.maxDy;
            this.maxDy = 0;
        }

        this.curStage++;
        if (this.curStage >= messages.Length)
        {
            // Update the Sensitivity with the average of the three calibration trials (scaled by calibrationFactor)
            Settings.Sensitivity = (this.maxDyTotal / 3) * calibrationFactor;
            SceneManager.LoadScene("MainMenu");
        }
        else
        {
            CalibrationUI.UpdateMessage(messages[this.curStage]);
        }
    }
}
