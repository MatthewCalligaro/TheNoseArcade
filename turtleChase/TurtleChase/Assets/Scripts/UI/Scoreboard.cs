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
            return scores[0].Score;
        }
    }

    private const int numScores = 5;

    /// <summary>
    /// Static reference to the one Scoreboard object in the scene to enable static methods
    /// </summary>
    private static Scoreboard instance;

    private static List<HighScore> scores = new List<global::HighScore>();



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
        scores.Add(score);
        scores.Sort();
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
        if (scores.Count > 0)
        {
            this.texts[Texts.HelpText.GetHashCode()].gameObject.SetActive(false);

            string scoreText = string.Empty;
            string distanceText = string.Empty;
            string dateText = string.Empty;

            for (int i = 0; i < Mathf.Min(numScores, scores.Count); i++)
            {
                scoreText += $"{i + 1}: {scores[i].Score}\n";
                distanceText += $"{scores[i].Distance}m\n";
                dateText += $"{scores[i].Date.ToString("dd/mm hh:mm")}\n";
            }

            // Load strings after removing trailing newlines
            this.texts[Texts.Scores.GetHashCode()].text = scoreText.Substring(0, scoreText.Length - 1);
            this.texts[Texts.Distances.GetHashCode()].text = distanceText.Substring(0, distanceText.Length - 1);
            this.texts[Texts.Dates.GetHashCode()].text = dateText.Substring(0, dateText.Length - 1);
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
