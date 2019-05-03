/// <summary>
/// Implements math functions not in Mathf
/// </summary>
public static class Utilities
{
    /// <summary>
    /// Interpolates from a maximum value to a minimum value
    /// </summary>
    /// <param name="min">Minimum value of the range</param>
    /// <param name="max">Maxmimu value of the range</param>
    /// <param name="scale">A value in the range [0, 1], where 0 gives max and 1 gives min</param>
    /// <returns>A value between min and max based on scale</returns>
    public static float InterpolateReverse(float min, float max, float scale)
    {
        return min + (max - min) * (1 - scale);
    }
}
