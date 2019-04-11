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
public class Settings
{
    ////////////////////////////////////////////////////////////////
    // Properties
    ////////////////////////////////////////////////////////////////

    /// <summary>
    /// Current player jump style
    /// </summary>
    public static JumpStyle JumpStyle
    {
        get
        {
            return settableSettings.JumpStyle.Value;
        }
    }

    /// <summary>
    /// Current game difficulty
    /// </summary>
    public static Difficulty Difficulty
    {
        get
        {
            return settableSettings.Difficulty.Value;
        }
        set
        {
            settableSettings.Difficulty = value;
        }
    }

    /// <summary>
    /// Current baseline magnitude of player jump
    /// </summary>
    public static float JumpPower
    {
        get
        {
            return settableSettings.JumpPower.Value;
        }
    }

    /// <summary>
    /// Minimum number of pixels/millisecond for a nose movement to be interpreted as a gesture
    /// </summary>
    public static float Sensitivity
    {
        get
        {
            return settableSettings.Sensitivity.Value;
        }
        set
        {
            settableSettings.Sensitivity = Mathf.Max(Mathf.Min(value, MaxSensitivity), MinSensitivity);
        }
    }



    ////////////////////////////////////////////////////////////////
    // Fields
    ////////////////////////////////////////////////////////////////

    /// <summary>
    /// Minimum value for JumpPower
    /// </summary>
    public const float MinJumpPower = 0.5f;

    /// <summary>
    /// Maximum value for JumpPower
    /// </summary>
    public const float MaxJumpPower = 1.5f;

    /// <summary>
    /// Minimum value for Sensitivity
    /// </summary>
    public const float MinSensitivity = 0.01f;

    /// <summary>
    /// Maximum value for Sensitivity
    /// </summary>
    public const float MaxSensitivity = 0.5f;

    /// <summary>
    /// Difference in sensitivity for horizontal vs. vertical movements
    /// </summary>
    public const float HorizontalSensitivityFactor = 1.5f;

    /// <summary>
    /// Minimum delay between jumps
    /// </summary>
    public const float JumpReloadTime = 0.5f;

    /// <summary>
    /// Minimum time between Menu actions
    /// </summary>
    public const float MenuMoveReloadTime = 1f;

    /// <summary>
    /// Multiplier corresponding to each difficulty in Difficulty
    /// </summary>
    public static readonly float[] difficultyMultipliers = { 1.0f, 1.5f, 2.0f };

    /// <summary>
    /// Default values for the settable settings
    /// </summary>
    private static readonly SettableSettings defaultSettings = new SettableSettings
    {
        JumpStyle = JumpStyle.Velocity,
        Difficulty = Difficulty.Medium,
        JumpPower = 1.0f,
        Sensitivity = 2.0f,
    };

    /// <summary>
    /// Current values of settable settings
    /// </summary>
    private static SettableSettings settableSettings;



    ////////////////////////////////////////////////////////////////
    // Methods
    ////////////////////////////////////////////////////////////////

    /// <summary>
    /// Restores all settings to default values
    /// </summary>
    public static void RestoreDefaults()
    {
        settableSettings = defaultSettings;
    }

    /// <summary>
    /// Updates the current settable settings with new values
    /// </summary>
    /// <param name="newSettings">Encapsulates new settings values (null values will be ignored)</param>
    public static void UpdateSettings(SettableSettings newSettings)
    {
        // Only take the non-null settings in newSettings
        if (newSettings.JumpStyle.HasValue)
        {
            settableSettings.JumpStyle = newSettings.JumpStyle;
        }
        if (newSettings.Difficulty.HasValue)
        {
            settableSettings.Difficulty = newSettings.Difficulty;
        }
        if (newSettings.JumpPower.HasValue)
        {
            settableSettings.JumpPower = newSettings.JumpPower;
        }
        if (newSettings.Sensitivity.HasValue)
        {
            Debug.Log(newSettings.Sensitivity.Value);
            settableSettings.Sensitivity = newSettings.Sensitivity;
        }
    }
}
