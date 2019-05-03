using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Scoreboard : Menu
{
    /// <summary>
    /// The text objects contained in the scoreboard
    /// </summary>
    private enum Texts
    {
        Title,
        NumberTitle,
        Numbers,
        ScoreTitle,
        Scores,
        DistanceTitle,
        Distances,
        DateTitle,
        Dates,
        DifficultyTitle,
        Difficulties,
        HelpText
    }
    
    /// <summary>
    /// Highest score on the scoreboard
    /// </summary>
    public static int HighScore
    {
        get
        {
            return scores.Count > 0 ? scores[0].Score : 0;
        }
    }

    /// <summary>
    /// Number of scores shown on the scoreboard
    /// </summary>
    private const int numScores = 5;

    /// <summary>
    /// Static reference to the one Scoreboard object in the scene to enable static methods
    /// </summary>
    private static Scoreboard instance;

    /// <summary>
    /// Results of every completed game
    /// </summary>
    private static List<HighScore> scores = new List<HighScore>();



    ////////////////////////////////////////////////////////////////
    // Public Methods
    ////////////////////////////////////////////////////////////////

    /// <summary>
    /// Method called when the scoreboard is opened
    /// </summary>
    public static void HandleOpen()
    {
        instance.PopulateScoreboard();
        instance.gameObject.SetActive(true);
        instance.MenuOpen();
    }

    /// <summary>
    /// Adds the information associated with a finished game to the scoreboard
    /// </summary>
    /// <param name="score">Information associated with a finished game</param>
    public static void AddScore(HighScore score)
    {
        scores.Add(score);

        // Sort scores in descending order so highest score is on top
        scores.Sort((a, b) => b.CompareTo(a));
    }

    /// <summary>
    /// Handles when the Close button is pressed by closing the sScoreboard
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

    /// <summary>
    /// Updates the text of the scoreboard based on the information in scores
    /// </summary>
    private void PopulateScoreboard()
    {
        if (scores.Count > 0)
        {
            this.texts[Texts.HelpText.GetHashCode()].gameObject.SetActive(false);

            string numberText = string.Empty;
            string scoreText = string.Empty;
            string distanceText = string.Empty;
            string dateText = string.Empty;
            string difficultyText = string.Empty;

            for (int i = 0; i < Mathf.Min(numScores, scores.Count); i++)
            {
                numberText += $"{i + 1}.\n";
                scoreText += $"{scores[i].Score}\n";
                distanceText += $"{scores[i].Distance}m\n";
                dateText += $"{scores[i].Date.ToString("MM/dd hh:mm")}\n";
                difficultyText += $"{scores[i].Difficulty.ToString()}\n";
            }

            string emptyLines = string.Empty;
            for (int i = scores.Count; i < numScores; i++)
            {
                emptyLines += "\n";
            }

            // Ensure every text field is exactly numScores lines long
            numberText += emptyLines;
            numberText = numberText.Substring(0, numberText.Length - 1);
            scoreText += emptyLines;
            scoreText = scoreText.Substring(0, scoreText.Length - 1);
            distanceText += emptyLines;
            distanceText = distanceText.Substring(0, distanceText.Length - 1);
            dateText += emptyLines;
            dateText = dateText.Substring(0, dateText.Length - 1);
            difficultyText += emptyLines;
            difficultyText = difficultyText.Substring(0, difficultyText.Length - 1);

            this.texts[Texts.Numbers.GetHashCode()].text = numberText;
            this.texts[Texts.Scores.GetHashCode()].text = scoreText;
            this.texts[Texts.Distances.GetHashCode()].text = distanceText;
            this.texts[Texts.Dates.GetHashCode()].text = dateText;
            this.texts[Texts.Difficulties.GetHashCode()].text = difficultyText;
        }
        else
        {
            this.texts[Texts.HelpText.GetHashCode()].gameObject.SetActive(true);

            this.texts[Texts.Numbers.GetHashCode()].text = string.Empty;
            this.texts[Texts.Scores.GetHashCode()].text = string.Empty;
            this.texts[Texts.Distances.GetHashCode()].text = string.Empty;
            this.texts[Texts.Dates.GetHashCode()].text = string.Empty;
            this.texts[Texts.Difficulties.GetHashCode()].text = string.Empty;
        }
    }

}
