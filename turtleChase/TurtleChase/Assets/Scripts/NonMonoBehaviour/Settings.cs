using UnityEngine;

/// <summary>
/// The different styles in which the player can jump
/// </summary>
public enum JumpStyle
{
    Velocity,
    Force,
    Jetpack
}

/// <summary>
/// The different game difficulties
/// </summary>
public enum Difficulty
{
    Easy,
    Medium,
    Hard,
}

/// <summary>
/// Stores the current game settings
/// </summary>
public static class Settings
{
    ////////////////////////////////////////////////////////////////
    // Properties
    ////////////////////////////////////////////////////////////////

    /// <summary>
    /// Current game difficulty
    /// </summary>
    public static Difficulty Difficulty { get; set; }

    /// <summary>
    /// Current player jump style
    /// </summary>
    public static JumpStyle JumpStyle
    {
        get
        {
            return settableSettings.JumpStyle;
        }
    }

    /// <summary>
    /// Current baseline magnitude of player jump
    /// </summary>
    public static float JumpPower
    {
        get
        {
            return settableSettings.JumpPower;
        }
    }

    /// <summary>
    /// Minimum number of pixels/millisecond for a nose movement to be interpreted as a gesture
    /// </summary>
    public static float Sensitivity
    {
        get
        {
            return settableSettings.Sensitivity;
        }
        set
        {
            settableSettings.Sensitivity = Mathf.Max(Mathf.Min(value, MaxSensitivity), MinSensitivity);
        }
    }

    /// <summary>
    /// Settings which can be set by the user
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



    ////////////////////////////////////////////////////////////////
    // Fields
    ////////////////////////////////////////////////////////////////

    /// <summary>
    /// Minimum value for JumpPower
    /// </summary>
    public const float MinJumpPower = 0.025f;

    /// <summary>
    /// Maximum value for JumpPower
    /// </summary>
    public const float MaxJumpPower = 1.5f;

    /// <summary>
    /// Minimum value for Sensitivity (pixels per millisecond)
    /// </summary>
    public const float MinSensitivity = 0.01f;

    /// <summary>
    /// Maximum value for Sensitivity (pixels per millisecond)
    /// </summary>
    public const float MaxSensitivity = 0.25f;

    /// <summary>
    /// Sensitivity at which a change is assumed to be a sampling error (pixels per millisecond)
    /// </summary>
    public const float IgnoreSensitivity = 0.5f;

    /// <summary>
    /// Difference in sensitivity for horizontal vs. vertical movements
    /// </summary>
    public const float HorizontalSensitivityFactor = 1.75f;

    /// <summary>
    /// Multiplier corresponding to each difficulty in Difficulty
    /// </summary>
    public static readonly float[] DifficultyMultipliers = { 1.0f, 1.5f, 2.0f };

    /// <summary>
    /// Default values for the settable settings
    /// </summary>
    public static readonly SettableSettings DefaultSettings = new SettableSettings
    {
        JumpStyle = JumpStyle.Velocity,
        JumpPower = 1.0f,
        Sensitivity = 0.1f,
    };

    /// <summary>
    /// Current values of settable settings
    /// </summary>
    private static SettableSettings settableSettings = DefaultSettings;
}
