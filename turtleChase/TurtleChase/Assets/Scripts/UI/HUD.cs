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

    public static void UpdateDistance(int distance)
    {
        instance.texts[Texts.Distance.GetHashCode()].text = "Distance: " + distance + "m";
    }

    protected override void Awake()
    {
        base.Awake();
        this.defaultActive = true;
        instance = GameObject.FindGameObjectsWithTag("HUD")[0].GetComponent<HUD>();
    }
}
