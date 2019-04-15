using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CalibrationUI : UIElement
{
    /// <summary>
    /// The text objects contained in the CalibrationUI
    /// </summary>
    private enum Texts
    {
        Title,
        Message
    }

    /// <summary>
    /// Static reference to the one CalibrationUI object in the scene to enable static methods
    /// </summary>
    public static CalibrationUI instance;



    ////////////////////////////////////////////////////////////////
    // Methods
    ////////////////////////////////////////////////////////////////

    /// <summary>
    /// Updates the message shown to the user
    /// </summary>
    /// <param name="text">New message show</param>
    public static void UpdateMessage(string text)
    {
        instance.texts[Texts.Message.GetHashCode()].text = text;
    }

    protected override void Awake()
    {
        base.Awake();
        this.defaultActive = true;
        instance = GameObject.FindGameObjectsWithTag("HUD")[0].GetComponent<CalibrationUI>();
    }
}
