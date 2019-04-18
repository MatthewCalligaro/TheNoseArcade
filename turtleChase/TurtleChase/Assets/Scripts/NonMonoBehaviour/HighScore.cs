using System;

public class HighScore : IComparable<HighScore>
{
    /// <summary>
    /// Score achieved during the round
    /// </summary>
    public int Score { get; set; }

    /// <summary>
    /// X distance traveled during the round
    /// </summary>
    public int Distance { get; set; }

    /// <summary>
    /// Date and time at which the round was completed 
    /// </summary>
    public DateTime Date { get; set; }

    public int CompareTo(HighScore other)
    {
        return Score.CompareTo(other.Score);
    }
}
