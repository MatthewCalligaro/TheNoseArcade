using UnityEngine;

public static class Settings
{
    /// <summary>
    /// Factor by which speed causes turn angle to decrease
    /// </summary>
    public const float SteerVelocityFactor = 0.1f;

    /// <summary>
    /// Maximum amount that the cursor can move in one frame
    /// </summary>
    public static readonly Vector2 MaxMovement = new Vector2(0.25f, 0.25f);

    /// <summary>
    /// Additional multiplier added to mouse sensitivity
    /// </summary>
    public static Vector2 MouseSensitivityMultiplier = new Vector2(2, 4);

    /// <summary>
    /// Screen threshold for acceleration zone
    /// </summary>
    public static readonly Threshold Accelerate = new Threshold(0.4f, 0.8f);

    /// <summary>
    /// Screen threshold for brake zone
    /// </summary>
    public static readonly Threshold Brake = new Threshold(0.2f, 0);

    /// <summary>
    /// Screen threshold for left turn zone
    /// </summary>
    public static readonly Threshold Left = new Threshold(0.45f, 0.2f);

    /// <summary>
    /// Screen threshold for right turn zone
    /// </summary>
    public static readonly Threshold Right = new Threshold(0.55f, 0.8f);

    /// <summary>
    /// Minimum sensitivity value in pixels to move the cursor across the entire screen
    /// </summary>
    public static readonly Vector2 MinSensitivity = new Vector2(200, 50);

    /// <summary>
    /// Maximum sensitivity value in pixels to move the cursor across the entire screen
    /// </summary>
    public static readonly Vector2 MaxSensitivity = new Vector2(800, 200);

    /// <summary>
    /// Starting position of the cursor on the screen in the [0, 1] range
    /// </summary>
    public static readonly Vector2 CursorStart = new Vector2(0.5f, 0.35f);

    /// <summary>
    /// Offset of the camera relative to the car
    /// </summary>
    public static readonly Vector3 CameraOffset = new Vector3(0, 2.5f, -3.5f);

    /// <summary>
    /// Downward rotation of the camera along the X axis
    /// </summary>
    public static readonly float CameraRotationX = 20;

    /// <summary>
    /// Number of pixels needed to move the cursor across the entire screen
    /// </summary>
    public static Vector2 Sensitivity
    {
        get
        {
            return settableSettings.Sensitivity;
        }
    }

    /// <summary>
    /// Settings which can be set by the player
    /// </summary>
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

    /// <summary>
    /// Default values for the settable settings
    /// </summary>
    public static readonly SettableSettings DefaultSettings = new SettableSettings
    {
        Sensitivity = new Vector2(400, 100)
    };

    /// <summary>
    /// Settings which can be set by the player
    /// </summary>
    private static SettableSettings settableSettings = DefaultSettings;
}
