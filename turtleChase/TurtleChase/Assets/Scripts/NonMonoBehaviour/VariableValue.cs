using UnityEngine;

public class VariableValue
{
    float worst;
    float best;
    float difficultyRate;

    public VariableValue(float worst, float best, float difficultyRate)
    {
        this.worst = worst;
        this.best = best;
        this.difficultyRate = difficultyRate;
    }
    public VariableValue(float singleValue)
    {
        this.worst = singleValue;
        this.best = singleValue;
        this.difficultyRate = 0;
    }
    
    public float GetValue(float difficultyMultiplier)
    {
        return (worst < best) ? 
            Mathf.Max(this.worst, this.best - this.difficultyRate * difficultyMultiplier) :
            Mathf.Min(this.worst, this.best + this.difficultyRate * difficultyMultiplier);
    }
}
