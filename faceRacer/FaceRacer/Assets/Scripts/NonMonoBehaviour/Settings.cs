using UnityEngine;

public static class Settings
{
    public static Vector2 MaxMovement = new Vector2(0.25f, 0.25f);
    public static Vector2 Sensitivity = new Vector2(400, 100);
    public static Vector2 MouseSensitivityMultiplier = new Vector2(2, 4);
    public static Threshold Accelerate = new Threshold(0.4f, 0.8f);
    public static Threshold Brake = new Threshold(0.2f, 0);
    public static Threshold Left = new Threshold(0.45f, 0.2f);
    public static Threshold Right = new Threshold(0.55f, 0.8f);

    public static Vector2 CursorStart = new Vector2(0.5f, 0.35f);
}
