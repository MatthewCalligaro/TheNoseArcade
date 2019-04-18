using System;

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
        // Compare by score first and use other data members to break ties
        if (Score.CompareTo(other.Score) != 0)
        {
            return Score.CompareTo(other.Score);
        }

        if (Distance.CompareTo(other.Distance) != 0)
        {
            return Distance.CompareTo(other.Distance);
        }

        if (Difficulty.CompareTo(other.Difficulty) != 0)
        {
            return Difficulty.CompareTo(other.Difficulty);
        }

        return Date.CompareTo(other.Date);
    }

    public bool Equals(HighScore other)
    {
        if (ReferenceEquals(null, other))
        {
            return false;
        }

        if (ReferenceEquals(this, other))
        {
            return true;
        }

        return Equals(other.Score, Score) && Equals(other.Distance, Distance) && Equals(other.Date, Date) && Equals(other.Difficulty, Difficulty);
    }

    public override bool Equals(object obj)
    {
        if (ReferenceEquals(null, obj))
        {
            return false;
        }

        if (ReferenceEquals(this, obj))
        {
            return true;
        }

        if (obj.GetType() != typeof(HighScore))
        {
            return false;
        }

        return Equals((HighScore)obj);
    }

    public override int GetHashCode()
    {
        unchecked
        {
            return (Score.GetHashCode() * 397) ^ (Distance.GetHashCode() * 73) ^ (Difficulty.GetHashCode() * 23) ^ Date.GetHashCode();
        }
    }

    public static bool operator==(HighScore left, HighScore right)
    {
        return Equals(left, right);
    }

    public static bool operator!=(HighScore left, HighScore right)
    {
        return !Equals(left, right);
    }
}
