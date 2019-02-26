﻿using UnityEngine;

public class Utilities
{
    public static float NormalDist(float mean = 0, float sdev = 1)
    {
        float r1 = Random.value + float.Epsilon;
        float r2 = Random.value + float.Epsilon;
        return mean + sdev * Mathf.Sqrt(-2.0f * Mathf.Log(r1)) * Mathf.Sin(2.0f * Mathf.PI * r2);
    }
}
