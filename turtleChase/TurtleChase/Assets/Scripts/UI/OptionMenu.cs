﻿using UnityEngine;

/// <summary>
/// Controller for the option menu
/// </summary>
public class OptionMenu : Menu
{
    /// <summary>
    /// The dropdowns contained in the option menu
    /// </summary>
    private enum Dropdowns
    {
        JumpStyle
    }

    /// <summary>
    /// The sliders contained in the option menu
    /// </summary>
    private enum Sliders
    {
        Sensitivity,
        JumpPower
    }

    /// <summary>
    /// Static reference to the one OptionMenu object in the scene to enable static methods
    /// </summary>
    private static OptionMenu instance;

    /// <summary>
    /// Settings which have been updated since the OptionMenu was opened
    /// </summary>
    private SettableSettings curSettings;



    ////////////////////////////////////////////////////////////////
    // Public Methods
    ////////////////////////////////////////////////////////////////

    /// <summary>
    /// Method called when the option menu is opened
    /// </summary>
    public static void HandleOpen()
    {
        Player.BlockingPause++;
        Controller.AddMenu(instance);
        instance.gameObject.SetActive(true);
        instance.curSettings = new SettableSettings();
        instance.ResetUIValues();
        instance.items[instance.curItem].HandleEnter();
    }

    /// <summary>
    /// Handles when the Sensitivity slider is changed by updating the relevant setting
    /// </summary>
    public void ChangeSensitivity()
    {
        if (this.sliders.Length > 0)
        {
            // The highest sensitivity slider value produces the minimum settingns sensitivity value and vice versa, 
            // since a "higher sensitivity" from the user's perspective means a smaller minimum pixel change
            curSettings.Sensitivity = Settings.maxSensitivity - this.sliders[Sliders.Sensitivity.GetHashCode()].value * (Settings.maxSensitivity - Settings.minSensitivity);
        }
    }

    /// <summary>
    /// Handles when the JumpPower slider is changed by updating the relevant setting
    /// </summary>
    public void ChangeJumpPower()
    {
        if (this.sliders.Length > 0)
        {
            curSettings.JumpPower = Settings.minJumpPower + this.sliders[Sliders.JumpPower.GetHashCode()].value * (Settings.maxJumpPower - Settings.minJumpPower);
        }
    }

    /// <summary>
    /// Handles when the JumpStyle dropdown is changed by updating the relevant setting
    /// </summary>
    public void ChangeJumpStyle()
    {
        if (curSettings != null)
        {
            curSettings.JumpStyle = (JumpStyle)this.dropdowns[Dropdowns.JumpStyle.GetHashCode()].value;
        }
    }

    /// <summary>
    /// Handles when the RestoreDefaults button is pressed by restoring default settings
    /// </summary>
    public void HandleRestoreDefaults()
    {
        Settings.RestoreDefaults();
        ResetUIValues();
    }

    /// <summary>
    /// Handles when the Quit button is pressed by closing the options menu without saving changes
    /// </summary>
    public void HandleQuit()
    {
        Player.BlockingPause--;
        Controller.RemoveMenu();
        this.gameObject.SetActive(false);
    }

    /// <summary>
    /// Handles when the Save button is pressed by saving the new settings and closing the options menu
    /// </summary>
    public void HandleSave()
    {
        Settings.UpdateSettings(this.curSettings);
        this.HandleQuit();
    }



    ////////////////////////////////////////////////////////////////
    // Unity Methods
    ////////////////////////////////////////////////////////////////

    protected override void Awake()
    {
        base.Awake();
        this.defaultActive = false;

        // Find the single OptionMenu object by tag
        instance = GameObject.FindGameObjectsWithTag("OptionMenu")[0].GetComponent<OptionMenu>();
    }



    ////////////////////////////////////////////////////////////////
    // Private Methods
    ////////////////////////////////////////////////////////////////

    /// <summary>
    /// Reset all UI dropdowns and sliders to reflect current Settings
    /// </summary>
    private void ResetUIValues()
    {
        this.sliders[Sliders.Sensitivity.GetHashCode()].value = ((float)(Settings.maxSensitivity - Settings.Sensitivity)) / (Settings.maxSensitivity - Settings.minSensitivity);
        this.sliders[Sliders.JumpPower.GetHashCode()].value = (Settings.JumpPower - Settings.minJumpPower) / (Settings.maxJumpPower - Settings.minJumpPower);
        this.dropdowns[Dropdowns.JumpStyle.GetHashCode()].value = Settings.JumpStyle.GetHashCode();
    }
}
