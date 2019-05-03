using UnityEngine;

/// <summary>
/// Encapsulates a threshold between a minimum and maximum value
/// </summary>
public class Threshold
{
    /// <summary>
    /// The minimum value of the threshold
    /// </summary>
    private float min;

    /// <summary>
    /// The maximum value of the threshold
    /// </summary>
    private float max;

    public Threshold(float min, float max)
    {
        this.min = min;
        this.max = max;
    }

    /// <summary>
    /// Interpolates between the minimum and maximum value of the threshold
    /// </summary>
    /// <param name="value">Amount between minimum (0) and maximum (1)</param>
    /// <returns>A value in the range [min, max] based on value</returns>
    public float Interpolate(float value)
    {
        return Mathf.Min(1, Mathf.Max(0, (value - min) / (max - min)));
    }
}
