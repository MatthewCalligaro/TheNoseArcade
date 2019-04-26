using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class Utilities
{
    public static float InterpolateReverse(float min, float max, float scale)
    {
        return min + (max - min) * (1 - scale);
    }
}
