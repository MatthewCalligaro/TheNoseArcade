using System;
using UnityEngine;

public class WinMenu : Menu
{
    /// <summary>
    /// The text objects contained in the loss menu
    /// </summary>
    private enum Texts
    {
        Title,
        Time
    }

    /// <summary>
    /// True if the current game has ended
    /// </summary>
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
    public static WinMenu instance;

    /// <summary>
    /// True if the current game has ended
    /// </summary>
    private bool gameOver = false;



    ////////////////////////////////////////////////////////////////
    // Methods
    ////////////////////////////////////////////////////////////////

    public static void HandleWin(TimeSpan time)
    {
        // Player.BlockingPause++;
        instance.gameOver = true;
        instance.gameObject.SetActive(true);
        instance.texts[Texts.Time.GetHashCode()].text = $"Time: {time.ToString(@"mm\:ss\.fff")}";
        instance.MenuOpen();
    }

    protected override void Awake()
    {
        base.Awake();
        this.defaultActive = false;

        // Find the single LossMenu object by tag
        instance = GameObject.FindGameObjectsWithTag("WinMenu")[0].GetComponent<WinMenu>();
    }
}