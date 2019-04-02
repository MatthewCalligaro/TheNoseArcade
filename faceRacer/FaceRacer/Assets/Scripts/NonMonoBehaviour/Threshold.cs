using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Threshold
{
    float min;
    float max;

    public Threshold(float min, float max)
    {
        this.min = min;
        this.max = max;
    }

    public float Interpolate(float value)
    {
        return Mathf.Min(1, Mathf.Max(0, (value - min) / (max - min)));
    }
}
