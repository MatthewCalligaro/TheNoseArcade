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
    // Public Properties
    ////////////////////////////////////////////////////////////////

    /// <summary>
    /// Current player jump style
    /// </summary>
    public static JumpStyle JumpStyle { get; set; }

    /// <summary>
    /// Current game difficulty
    /// </summary>
    public static Difficulty Difficulty { get; set; }

    /// <summary>
    /// Current basline magnitude of player jump
    /// </summary>
    public static float JumpPower { get; set; }



    ////////////////////////////////////////////////////////////////
    // Private Fields
    ////////////////////////////////////////////////////////////////

    /// <summary>
    /// Multiplier corresponding to each difficulty in Difficulty
    /// </summary>
    public static readonly float[] difficultyMultipliers = { 1.0f, 1.5f, 2.0f };

    /// <summary>
    /// Maxmium value for JumpPower
    /// </summary>
    public static float maxJumpPower = 1.5f;

    /// <summary>
    /// Minimum value for JumpPower
    /// </summary>
    public static float minJumpPower = 0.5f;

    /// <summary>
    /// Default value for JumpStyle
    /// </summary>
    private static JumpStyle defaultJumpStyle = JumpStyle.Velocity;

    /// <summary>
    /// Default value for Difficulty
    /// </summary>
    private static Difficulty defaultDifficulty = Difficulty.Medium;

    /// <summary>
    /// Default value for JumpPower
    /// </summary>
    private static float defaultJumpPower = 1.0f;



    ////////////////////////////////////////////////////////////////
    // Public Methods
    ////////////////////////////////////////////////////////////////

    /// <summary>
    /// Restores all settings to default values
    /// </summary>
    public static void RestoreDefaults()
    {
        JumpStyle = defaultJumpStyle;
        Difficulty = defaultDifficulty;
        JumpPower = defaultJumpPower;
    }
}
