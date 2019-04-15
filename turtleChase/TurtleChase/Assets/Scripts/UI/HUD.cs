using UnityEngine;

/// <summary>
/// Controller for the heads up display
/// </summary>
public class HUD : UIElement
{
    /// <summary>
    /// The text objects contained in the HUD
    /// </summary>
    private enum Texts
    {
        Score,
        Distance,
        Tutorial,
        TutorialTaskCount,
        TutorialHelpText
    }

    /// <summary>
    /// Static reference to the one HUD object in the scene to enable static methods
    /// </summary>
    public static HUD instance;
    


    ////////////////////////////////////////////////////////////////
    // Methods
    ////////////////////////////////////////////////////////////////

    /// <summary>
    /// Updates the score shown on the HUD
    /// </summary>
    /// <param name="score">New score to show</param>
    public static void UpdateScore(int score)
    {
        instance.texts[Texts.Score.GetHashCode()].text = "Score: " + score;
    }

    /// <summary>
    /// Updates the distance shown on the HUD
    /// </summary>
    /// <param name="distance">New distance to show</param>
    public static void UpdateDistance(int distance)
    {
        instance.texts[Texts.Distance.GetHashCode()].text = "Distance: " + distance + "m";
    }

    /// <summary>
    /// Updates the tutorial text
    /// </summary>
    /// <param name="text">New tutorial text</param>
    public static void UpdateTutorialText(string text)
    {
        instance.texts[Texts.Tutorial.GetHashCode()].text = text;
    }

    /// <summary>
    /// Updates the tutorial task text of the form: "text": "remainingCount"
    /// </summary>
    /// <param name="text">Text describing the task</param>
    /// <param name="remainingCount">Remaining number to complete</param>
    public static void UpdateTutorialTaskCountText(string text, int remainingCount)
    {
        instance.texts[Texts.TutorialTaskCount.GetHashCode()].text = $"{text}: {remainingCount}";
    }

    /// <summary>
    /// Updates the tutorial bonus help text
    /// </summary>
    /// <param name="text">New tutorial help text</param>
    public static void UpdateTutorialHelpText(string text)
    {
        instance.texts[Texts.TutorialHelpText.GetHashCode()].text = text;
    }

    /// <summary>
    /// Removes the tutorial task count text
    /// </summary>
    public static void ClearTutorialTaskCountText()
    {
        instance.texts[Texts.TutorialTaskCount.GetHashCode()].text = string.Empty;
    }

    protected override void Awake()
    {
        base.Awake();
        this.defaultActive = true;
        instance = GameObject.FindGameObjectsWithTag("HUD")[0].GetComponent<HUD>();
    }
}
