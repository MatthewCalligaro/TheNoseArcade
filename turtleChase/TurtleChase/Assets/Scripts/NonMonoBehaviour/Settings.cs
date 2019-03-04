public enum JumpStyle
{
    Velocity,
    Force,
    Jetpack
}

public enum PlayStyle
{
    Chase,
    Classic
}

public enum Difficulty
{
    Easy,
    Medium,
    Hard,
}

public class Settings
{
    public static JumpStyle JumpStyle = JumpStyle.Velocity;
    public static PlayStyle PlayStyle = PlayStyle.Chase;
    public static Difficulty Difficulty = Difficulty.Medium;
    public static float JumpMultiplier = 1.0f;
}
