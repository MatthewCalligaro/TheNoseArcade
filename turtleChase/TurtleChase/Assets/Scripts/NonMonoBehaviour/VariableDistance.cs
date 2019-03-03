using UnityEngine;

public class VariableDistance
{
    float min;
    float max;
    float decayRate;

    public VariableDistance(float min, float max, float decayRate)
    {
        this.min = min;
        this.max = max;
        this.decayRate = decayRate;
    }
    
    public float GetDistance(float decay)
    {
        return Mathf.Max(this.min, this.max - this.decayRate * decay);
    }
}
