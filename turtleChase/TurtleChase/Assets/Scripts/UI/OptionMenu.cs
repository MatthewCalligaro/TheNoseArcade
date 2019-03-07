using UnityEngine;
using UnityEngine.UI;

public class OptionMenu : UIElement
{
    private enum Dropdowns
    {
        Difficulty,
        JumpStyle
    }

    private enum Sliders
    {
        JumpPower
    }

    private static OptionMenu instance;
    public static OptionMenu Instance
    {
        get
        {
            return instance;
        }
    }

    public static void HandleLaunch()
    {
        Player.BlockingPause++;
        Instance.gameObject.SetActive(true);
        Instance.ResetUIValues();
    }

    public void ChangeDifficulty()
    {
        Settings.Difficulty = (Difficulty)this.dropdowns[Dropdowns.Difficulty.GetHashCode()].value;
    }

    public void ChangeJumpStyle()
    {
        Settings.JumpStyle = (JumpStyle)this.dropdowns[Dropdowns.JumpStyle.GetHashCode()].value;
    }

    public void ChangeJumpPower()
    {
        Settings.JumpPower = this.sliders[Sliders.JumpPower.GetHashCode()].value * Settings.maxJumpPower;
    }

    public void RestoreDefaults()
    {
        Settings.RestoreDefaults();
        ResetUIValues();
    }

    public void CloseOptions()
    {
        Player.BlockingPause--;
        this.gameObject.SetActive(false);
    }

    protected override void Awake()
    {
        base.Awake();
        this.defaultActive = false;
        instance = GameObject.FindGameObjectsWithTag("OptionMenu")[0].GetComponent<OptionMenu>();
    }

    private void ResetUIValues()
    {
        this.dropdowns[Dropdowns.Difficulty.GetHashCode()].value = Settings.Difficulty.GetHashCode();
        this.dropdowns[Dropdowns.JumpStyle.GetHashCode()].value = Settings.JumpStyle.GetHashCode();
        this.sliders[Sliders.JumpPower.GetHashCode()].value = Settings.JumpPower / Settings.maxJumpPower;
    }
}
