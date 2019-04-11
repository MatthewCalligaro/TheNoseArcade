using UnityEngine;

/// <summary>
/// Controller for the loss menu (shown when the game ends)
/// </summary>
public class LossMenu : Menu
{
    /// <summary>
    /// The text objects contained in the loss menu
    /// </summary>
    private enum Texts
    {
        Title,
        Score
    }

    public static bool GameOver
    {
        get
        {
            return instance.gameOver;
        }
    }

    /// <summary>
    /// Static reference to the one LossMenu object in the scene to enable static methods
    /// </summary>
    public static LossMenu instance;

    private bool gameOver = false;



    ////////////////////////////////////////////////////////////////
    // Methods
    ////////////////////////////////////////////////////////////////

    /// <summary>
    /// Method called when the game ends to stop gameplay and launch the loss menu
    /// </summary>
    /// <param name="score">Player score when the game ended</param>
    public static void HandleLoss(int score)
    {
        Player.BlockingPause++;
        instance.gameOver = true;
        instance.gameObject.SetActive(true);
        instance.texts[Texts.Score.GetHashCode()].text = "Score: " + score;
        instance.MenuOpen();
    }

    protected override void Awake()
    {
        base.Awake();
        this.defaultActive = false;

        // Find the single LossMenu object by tag
        instance = GameObject.FindGameObjectsWithTag("LossMenu")[0].GetComponent<LossMenu>();
    }
}
