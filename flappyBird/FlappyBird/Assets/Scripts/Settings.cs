public enum JumpStyle
{
    Velocity,
    Force,
    Jetpack
}

public enum PlayStyle
{
    Classic,
    Runner,
}

public class Settings
{
    public static JumpStyle JumpStyle = JumpStyle.Velocity;
    public static PlayStyle PlayStyle = PlayStyle.Classic;
}
