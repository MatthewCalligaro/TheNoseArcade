using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Calibration : MonoBehaviour, INoseController
{
    private const float calibrationFactor = 0.6f;

    private static readonly string[] messages = 
    {
        "Please adjust your position so that your nose is centered and within 18 inches of your webcam.  When you are ready to begin calibration, press the space bar.",
        "In Turtle Chase, you jump by raising your nose.  It is easiest for us to detect large nose movements, so we recommend performing each jump by pointing your nose downward and then upward, as though you went from looking at the ground to looking at the ceiling.\n\n Please perform one jump now and press the space bar.",
        "Please perform another jump and press the space bar (1/3 complete).",
        "Please perform another jump and press the space bar (2/3 complete).",
    };

    private int curStage = 0;

    private int lastY;

    private int maxDy = 0;

    private int sensitivity;

    public void UpdateFacePosition(int packed)
    {
        int y = (packed >> 16) & 0xFFFF;
        this.maxDy = Mathf.Max(y - this.lastY, this.maxDy);
        this.lastY = y;
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
                Settings.Sensitivity = (int)((this.sensitivity / 3) * calibrationFactor);
                Debug.Log(Settings.Sensitivity);
                SceneManager.LoadScene(0);
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
