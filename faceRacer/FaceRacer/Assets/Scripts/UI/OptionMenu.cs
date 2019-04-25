using UnityEngine;

/// <summary>
/// Controller for the option menu
/// </summary>
public class OptionMenu : Menu
{
    /// <summary>
    /// The sliders contained in the option menu
    /// </summary>
    private enum Sliders
    {
        HorizontalSensitivity,
        VerticalSensitivity
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
        // Player.BlockingPause++;
        instance.gameObject.SetActive(true);
        instance.curSettings = new SettableSettings();
        instance.ResetUIValues();
        instance.MenuOpen();
    }

    public void ChangeHorizontalSensitivity()
    {
        if (this.sliders.Length > 0)
        {
            // The highest sensitivity slider value produces the minimum settings sensitivity value and vice versa, 
            // since a "higher sensitivity" from the user's perspective means a smaller minimum pixel change
            // curSettings.Sensitivity = Settings.MaxSensitivity - this.sliders[Sliders.Sensitivity.GetHashCode()].value * (Settings.MaxSensitivity - Settings.MinSensitivity);
        }
    }

    /// <summary>
    /// Handles when the JumpPower slider is changed by updating the relevant setting
    /// </summary>
    public void ChangeVerticalSensitivity()
    {
        if (this.sliders.Length > 0)
        {
            // curSettings.JumpPower = Settings.MinJumpPower + this.sliders[Sliders.JumpPower.GetHashCode()].value * (Settings.MaxJumpPower - Settings.MinJumpPower);
        }
    }

    /// <summary>
    /// Handles when the RestoreDefaults button is pressed by restoring default settings
    /// </summary>
    public void HandleRestoreDefaults()
    {
        // Settings.RestoreDefaults();
        ResetUIValues();
    }

    /// <summary>
    /// Handles when the Quit button is pressed by closing the options menu without saving changes
    /// </summary>
    public void HandleQuit()
    {
        // Player.BlockingPause--;
        this.MenuClose();
        this.gameObject.SetActive(false);
    }

    /// <summary>
    /// Handles when the Save button is pressed by saving the new settings and closing the options menu
    /// </summary>
    public void HandleSave()
    {
        // Settings.UpdateSettings(this.curSettings);
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
        //this.sliders[Sliders.Sensitivity.GetHashCode()].value = ((float)(Settings.MaxSensitivity - Settings.Sensitivity)) / (Settings.MaxSensitivity - Settings.MinSensitivity);
        //this.sliders[Sliders.JumpPower.GetHashCode()].value = (Settings.JumpPower - Settings.MinJumpPower) / (Settings.MaxJumpPower - Settings.MinJumpPower);
        //this.dropdowns[Dropdowns.JumpStyle.GetHashCode()].value = Settings.JumpStyle.GetHashCode();
    }
}