using System;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Calibration : MonoBehaviour, INoseController
{
    private const float calibrationFactor = 0.75f;

    private static readonly string[] messages = 
    {
        "Please adjust your position so that your nose is centered and within 18 inches of your webcam.  When you are ready to begin calibration, press the space bar.",
        "In Turtle Chase, you jump by raising your nose.  It is easiest for us to detect large nose movements, so we recommend performing each jump by pointing your nose downward and then upward, as though you went from looking at the ground to looking at the ceiling.\n\n Please perform one jump now and press the space bar.",
        "Please perform another jump and press the space bar (1/3 complete).",
        "Please perform another jump and press the space bar (2/3 complete).",
    };

    private int curStage = 0;

    private int lastY;

    private DateTime lastTime;

    private float maxDy = 0;

    private float sensitivity;

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
            if (this.curStage > 0)
            {
                this.sensitivity += this.maxDy;
                this.maxDy = 0;
            }

            this.curStage++;
            if (this.curStage >= messages.Length)
            {
                Settings.Sensitivity = (this.sensitivity / 3) * calibrationFactor;
                Debug.Log(Settings.Sensitivity);
                SceneManager.LoadScene("MainMenu");
            }
            else
            {
                CalibrationUI.UpdateMessage(messages[this.curStage]);
            }
        }

        if (Input.GetKey(KeyCode.LeftShift) || Input.GetKey(KeyCode.RightShift))
        {
            UpdateFacePosition((((int)Input.mousePosition.x) & 0xFFFF) | ((((int)Input.mousePosition.y) & 0xFFFF) << 16));
        }
    }

    private void Start()
    {
        CalibrationUI.UpdateMessage(messages[this.curStage]);
    }
}
