using UnityEngine;

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
        instance.gameObject.SetActive(true);
        instance.curSettings = new SettableSettings();
        instance.ResetUIValues(Settings.Settable);
        instance.MenuOpen();
    }

    /// <summary>
    /// Handles when the RestoreDefaults button is pressed by restoring default settings
    /// </summary>
    public void HandleRestoreDefaults()
    {
        ResetUIValues(Settings.DefaultSettings);
    }

    /// <summary>
    /// Handles when the Quit button is pressed by closing the options menu without saving changes
    /// </summary>
    public void HandleQuit()
    {
        Player.BlockingPause--;
        this.MenuClose();
        this.gameObject.SetActive(false);
    }

    /// <summary>
    /// Handles when the Save button is pressed by saving the new settings and closing the options menu
    /// </summary>
    public void HandleSave()
    {
        Settings.Settable = new SettableSettings()
        {
            JumpStyle = (JumpStyle)this.dropdowns[Dropdowns.JumpStyle.GetHashCode()].value,
            JumpPower = Settings.MinJumpPower + this.sliders[Sliders.JumpPower.GetHashCode()].value * (Settings.MaxJumpPower - Settings.MinJumpPower),
            Sensitivity = Settings.MaxSensitivity - this.sliders[Sliders.Sensitivity.GetHashCode()].value * (Settings.MaxSensitivity - Settings.MinSensitivity),
        };
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
    private void ResetUIValues(SettableSettings settings)
    {
        this.sliders[Sliders.Sensitivity.GetHashCode()].value = ((float)(Settings.MaxSensitivity - settings.Sensitivity)) / (Settings.MaxSensitivity - Settings.MinSensitivity);
        this.sliders[Sliders.JumpPower.GetHashCode()].value = (settings.JumpPower - Settings.MinJumpPower) / (Settings.MaxJumpPower - Settings.MinJumpPower);
        this.dropdowns[Dropdowns.JumpStyle.GetHashCode()].value = settings.JumpStyle.GetHashCode();
    }
}
