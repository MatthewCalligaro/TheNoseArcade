public enum JumpStyle
{
    Velocity,
    Force,
    Jetpack
}

public enum Difficulty
{
    Easy,
    Medium,
    Hard,
}

public class Settings
{
    public static readonly float[] difficultyMultiers = { 1.0f, 1.5f, 2.0f };
    public static float maxJumpPower = 2.0f;
    public static JumpStyle JumpStyle { get; set; }
    public static Difficulty Difficulty { get; set; }
    public static float JumpPower { get; set; }

    private static JumpStyle defaultJumpStyle = JumpStyle.Velocity;
    private static Difficulty defaultDifficulty = Difficulty.Medium;
    private static float defaultJumpPower = 1.0f;

    public static void RestoreDefaults()
    {
        JumpStyle = defaultJumpStyle;
        Difficulty = defaultDifficulty;
        JumpPower = defaultJumpPower;
    }
}
