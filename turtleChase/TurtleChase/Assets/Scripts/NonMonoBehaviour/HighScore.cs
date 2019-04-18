﻿using System;

/// <summary>
/// Encapsulates the information from a finished game
/// </summary>
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

    /// <summary>
    /// Difficulty at which the round was played
    /// </summary>
    public Difficulty Difficulty { get; set; }

    public int CompareTo(HighScore other)
    {
        // Compare by score and use distance to break ties
        if (Score.CompareTo(other.Score) != 0)
        {
            return Score.CompareTo(other.Score);
        }

        return Distance.CompareTo(other.Distance);
    }
}
