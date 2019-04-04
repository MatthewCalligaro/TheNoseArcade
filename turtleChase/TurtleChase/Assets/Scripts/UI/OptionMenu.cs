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
        instance.ResetUIValues();
    }

    /// <summary>
    /// Handles when the Difficulty dropdown is changed by updating the relevant setting
    /// </summary>
    public void ChangeDifficulty()
    {
        Settings.Difficulty = (Difficulty)this.dropdowns[Dropdowns.Difficulty.GetHashCode()].value;
    }

    /// <summary>
    /// Handles when the JumpStyle dropdown is changed by updating the relevant setting
    /// </summary>
    public void ChangeJumpStyle()
    {
        Settings.JumpStyle = (JumpStyle)this.dropdowns[Dropdowns.JumpStyle.GetHashCode()].value;
    }

    /// <summary>
    /// Handles when the JumpPower slider is changed by updating the relevant setting
    /// </summary>
    public void ChangeJumpPower()
    {
        Settings.JumpPower = Settings.minJumpPower + this.sliders[Sliders.JumpPower.GetHashCode()].value * (Settings.maxJumpPower - Settings.minJumpPower);
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
    /// Handles when the Close button is pressed by closing the option's menu
    /// </summary>
    public void HandleClose()
    {
        Player.BlockingPause--;
        this.gameObject.SetActive(false);
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
    /// Reset all UI dropdown
    /// </summary>
    private void ResetUIValues()
    {
        this.dropdowns[Dropdowns.Difficulty.GetHashCode()].value = Settings.Difficulty.GetHashCode();
        this.dropdowns[Dropdowns.JumpStyle.GetHashCode()].value = Settings.JumpStyle.GetHashCode();
        this.sliders[Sliders.JumpPower.GetHashCode()].value = (Settings.JumpPower - Settings.minJumpPower) / (Settings.maxJumpPower - Settings.minJumpPower);
    }
}
