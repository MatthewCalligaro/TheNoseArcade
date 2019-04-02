using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Settings
{
    public static Vector2 MouseSensitivity = new Vector2(1f, 1f);
    public static Threshold Accelerate = new Threshold(0.4f, 0.8f);
    public static Threshold Brake = new Threshold(0.2f, 0);
    public static Threshold Left = new Threshold(0.45f, 0.2f);
    public static Threshold Right = new Threshold(0.55f, 0.8f);
}
