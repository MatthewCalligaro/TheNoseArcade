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
        // Player.BlockingPause--;
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
            Sensitivity = new Vector2(
                Utilities.InterpolateReverse(Settings.MinSensitivity.x, Settings.MaxSensitivity.x, this.sliders[Sliders.HorizontalSensitivity.GetHashCode()].value),
                Utilities.InterpolateReverse(Settings.MinSensitivity.y, Settings.MaxSensitivity.y, this.sliders[Sliders.VerticalSensitivity.GetHashCode()].value))
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
        this.sliders[Sliders.HorizontalSensitivity.GetHashCode()].value = (Settings.MaxSensitivity.x - settings.Sensitivity.x) / (Settings.MaxSensitivity.x - Settings.MinSensitivity.x);
        this.sliders[Sliders.VerticalSensitivity.GetHashCode()].value = (Settings.MaxSensitivity.y - settings.Sensitivity.y) / (Settings.MaxSensitivity.y - Settings.MinSensitivity.y);
    }
}