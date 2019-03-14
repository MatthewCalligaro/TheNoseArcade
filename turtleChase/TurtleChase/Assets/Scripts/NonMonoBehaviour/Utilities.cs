using UnityEngine;

/// <summary>
/// Implements math functions not in Mathf
/// </summary>
public class Utilities
{
    /// <summary>
    /// Generates a Gaussian variable with a given mean and standard deviation
    /// </summary>
    /// <param name="mean">The mean of the normal distribution from which to draw</param>
    /// <param name="sdev">The standard deviation of the normal distribution from which to draw</param>
    /// <returns>A Gaussian variable randomly chosen from the specified normal distribution</returns>
    public static float NormalDist(float mean = 0, float sdev = 1)
    {
        float r1 = Random.value + float.Epsilon;
        float r2 = Random.value + float.Epsilon;
        return mean + sdev * Mathf.Sqrt(-2.0f * Mathf.Log(r1)) * Mathf.Sin(2.0f * Mathf.PI * r2);
    }
}
