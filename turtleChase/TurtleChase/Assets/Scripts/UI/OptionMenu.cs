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
        Difficulty,
        JumpStyle
    }

    /// <summary>
    /// The sliders contained in the option menu
    /// </summary>
    private enum Sliders
    {
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
    /// Handles when the Difficulty dropdown is changed by updating the relevant setting
    /// </summary>
    public void ChangeDifficulty()
    {
        curSettings.Difficulty = (Difficulty)this.dropdowns[Dropdowns.Difficulty.GetHashCode()].value;
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
    /// Handles when the JumpPower slider is changed by updating the relevant setting
    /// </summary>
    public void ChangeJumpPower()
    {
        curSettings.JumpPower = Settings.minJumpPower + this.sliders[Sliders.JumpPower.GetHashCode()].value * (Settings.maxJumpPower - Settings.minJumpPower);
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
        this.dropdowns[Dropdowns.Difficulty.GetHashCode()].value = Settings.Difficulty.GetHashCode();
        this.dropdowns[Dropdowns.JumpStyle.GetHashCode()].value = Settings.JumpStyle.GetHashCode();
        this.sliders[Sliders.JumpPower.GetHashCode()].value = (Settings.JumpPower - Settings.minJumpPower) / (Settings.maxJumpPower - Settings.minJumpPower);
    }
}
