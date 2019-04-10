/// <summary>
/// Encapsulates the settings which the player can update
/// </summary>
public class SettableSettings
{
    /// <summary>
    /// Player jump style
    /// </summary>
    public JumpStyle? JumpStyle { get; set; }

    /// <summary>
    /// Game difficulty
    /// </summary>
    public Difficulty? Difficulty { get; set; }

    /// <summary>
    /// Baseline magnitude of player jump
    /// </summary>
    public float? JumpPower { get; set; }

    /// <summary>
    /// Minimum number of pixels/millisecond for a nose movement to be interpreted as a gesture
    /// </summary>
    public float? Sensitivity { get; set; }
}
