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
        Dates,
        HelpText
    }

    public int HighScore
    {
        get
        {
            return this.scores[0].Score;
        }
    }

    private const int numScores = 5;

    /// <summary>
    /// Static reference to the one Scoreboard object in the scene to enable static methods
    /// </summary>
    private static Scoreboard instance;

    private List<HighScore> scores = new List<global::HighScore>();



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
        if (this.scores.Count > 0)
        {
            this.texts[Texts.HelpText.GetHashCode()].gameObject.SetActive(false);

            string scores = string.Empty;
            string distances = string.Empty;
            string dates = string.Empty;

            for (int i = 0; i < Mathf.Min(numScores, this.scores.Count); i++)
            {
                scores += $"{i + 1}: {this.scores[i].Score}\n";
                distances += $"{this.scores[i].Distance} m\n";
                dates += $"{this.scores[i].Date.ToString("dd/mm hh:mm")}\n";
            }

            // Load strings after removing trailing newlines
            this.texts[Texts.Scores.GetHashCode()].text = scores.Substring(0, scores.Length - 2);
            this.texts[Texts.Distances.GetHashCode()].text = distances.Substring(0, distances.Length - 2);
            this.texts[Texts.Dates.GetHashCode()].text = dates.Substring(0, dates.Length - 2);
        }
        else
        {
            this.texts[Texts.HelpText.GetHashCode()].gameObject.SetActive(true);

            this.texts[Texts.Scores.GetHashCode()].text = string.Empty;
            this.texts[Texts.Distances.GetHashCode()].text = string.Empty;
            this.texts[Texts.Dates.GetHashCode()].text = string.Empty;
        }
    }

}
