using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Scoreboard : Menu
{
    private enum Texts
    {
        Title,
        ScoreTitle,
        Scores,
        DistanceTitle,
        Distances,
        DateTitle,
        Dates
    }

    public int HighScore
    {
        get
        {
            return this.scores[0].Score;
        }
    }

    private const int numScores = 10;

    /// <summary>
    /// Static reference to the one Scoreboard object in the scene to enable static methods
    /// </summary>
    private static Scoreboard instance;

    private List<HighScore> scores;



    ////////////////////////////////////////////////////////////////
    // Public Methods
    ////////////////////////////////////////////////////////////////

    /// <summary>
    /// Method called when the Scoreboard is opened
    /// </summary>
    public static void HandleOpen()
    {
        instance.PopulateScoreboard();
        instance.gameObject.SetActive(true);
        instance.MenuOpen();
    }

    public static void AddScore(HighScore score)
    {
        instance.scores.Add(score);
        instance.scores.Sort();
    }

    /// <summary>
    /// Handles when the Close button is pressed by closing the Scoreboard
    /// </summary>
    public void HandleClose()
    {
        this.MenuClose();
        this.gameObject.SetActive(false);
    }



    ////////////////////////////////////////////////////////////////
    // Unity Methods
    ////////////////////////////////////////////////////////////////

    protected override void Awake()
    {
        base.Awake();
        this.defaultActive = false;

        // Find the single Scoreboard object by tag
        instance = GameObject.FindGameObjectsWithTag("Scoreboard")[0].GetComponent<Scoreboard>();
    }



    ////////////////////////////////////////////////////////////////
    // Private Methods
    ////////////////////////////////////////////////////////////////

    private void PopulateScoreboard()
    {
        for (int i = 0; i < Mathf.Max(numScores, this.scores.Count); i++)
        {
            this.texts[Texts.Scores.GetHashCode()].text = $"{i + 1}: {this.scores[i].Score}";
            this.texts[Texts.Distances.GetHashCode()].text = $"{this.scores[i].Distance} m";
            this.texts[Texts.Dates.GetHashCode()].text = this.scores[i].Date.ToString("dd/mm hh:mm");
        }
    }

}
