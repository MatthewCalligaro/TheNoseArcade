using UnityEngine;

public static class Settings
{
    public static readonly Vector2 MaxMovement = new Vector2(0.25f, 0.25f);
    public static Vector2 MouseSensitivityMultiplier = new Vector2(2, 4);

    public static readonly Threshold Accelerate = new Threshold(0.4f, 0.8f);
    public static readonly Threshold Brake = new Threshold(0.2f, 0);
    public static readonly Threshold Left = new Threshold(0.45f, 0.2f);
    public static readonly Threshold Right = new Threshold(0.55f, 0.8f);

    public static readonly Vector2 MinSensitivity = new Vector2(200, 50);
    public static readonly Vector2 MaxSensitivity = new Vector2(800, 200);

    public static readonly Vector2 CursorStart = new Vector2(0.5f, 0.35f);

    public static Vector2 Sensitivity
    {
        get
        {
            return settableSettings.Sensitivity;
        }
    }

    public static SettableSettings Settable
    {
        get
        {
            return settableSettings;
        }
        set
        {
            settableSettings = value;
        }
    }

    public static readonly SettableSettings DefaultSettings = new SettableSettings
    {
        Sensitivity = new Vector2(400, 100)
    };


    private static SettableSettings settableSettings = DefaultSettings;
}
