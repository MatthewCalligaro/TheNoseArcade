using UnityEngine;

/// <summary>
/// Represents a value which will vary linearly based on difficulty
/// </summary>
public class VariableValue
{
    /// <summary>
    /// The hardest value
    /// </summary>
    private float worst;

    /// <summary>
    /// The easiest value
    /// </summary>
    private float best;

    /// <summary>
    /// The amount to change the value per unit of difficulty
    /// </summary>
    private float difficultyRate;



    ////////////////////////////////////////////////////////////////
    // Constructors
    ////////////////////////////////////////////////////////////////

    /// <summary>
    /// Creates a VariableValue by specifying all three private fields
    /// </summary>
    /// <param name="worst">The hardest value</param>
    /// <param name="best">The easiest value</param>
    /// <param name="difficultyRate">The amount to change the value per unit of difficulty</param>
    public VariableValue(float worst, float best, float difficultyRate)
    {
        this.worst = worst;
        this.best = best;
        this.difficultyRate = difficultyRate;
    }

    /// <summary>
    /// Creates a VariableValue that does not vary
    /// </summary>
    /// <param name="singleValue">The single value to always return</param>
    public VariableValue(float singleValue)
    {
        this.worst = singleValue;
        this.best = singleValue;
        this.difficultyRate = 0;
    }



    ////////////////////////////////////////////////////////////////
    // Public Methods
    ////////////////////////////////////////////////////////////////

    /// <summary>
    /// Calculates a VariableValue for a given difficulty
    /// </summary>
    /// <param name="difficultyMultiplier">Units of difficulty</param>
    /// <returns>A value between best and worst based on the difficulty</returns>
    public float GetValue(float difficultyMultiplier)
    {
        return (worst < best) ? 
            Mathf.Max(this.worst, this.best - this.difficultyRate * difficultyMultiplier) :
            Mathf.Min(this.worst, this.best + this.difficultyRate * difficultyMultiplier);
    }
}
