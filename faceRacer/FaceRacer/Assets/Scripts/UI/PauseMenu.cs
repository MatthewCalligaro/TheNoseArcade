using System;
using UnityEngine;

/// <summary>
/// Controller for the pause menu
/// </summary>
public class PauseMenu : Menu
{
    /// <summary>
    /// True if the game is currently paused
    /// </summary>
    public static bool Paused
    {
        get
        {
            return instance.paused;
        }
    }

    public static TimeSpan TimePaused
    {
        get
        {
            return instance.timePaused;
        }
    }

    /// <summary>
    /// Static reference to the one PauseMenu object in the scene to enable static methods
    /// </summary>
    private static PauseMenu instance;

    /// <summary>
    /// True if the game is currently paused
    /// </summary>
    private bool paused = false;

    private TimeSpan timePaused = TimeSpan.Zero;

    private DateTime enterPauseTime;



    ////////////////////////////////////////////////////////////////
    // Public Methods
    ////////////////////////////////////////////////////////////////

    /// <summary>
    /// Toggles whether the pause menu is opened or closed
    /// </summary>
    public static void TogglePauseMenu()
    {
        instance.paused = !instance.paused;
        instance.gameObject.SetActive(instance.paused);

        if (instance.paused)
        {
            instance.MenuOpen();
            instance.enterPauseTime = DateTime.Now;
            Time.timeScale = 0;
        }
        else
        {
            instance.MenuClose();
            instance.timePaused += DateTime.Now - instance.enterPauseTime;
            Time.timeScale = 1;
        }
    }

    /// <summary>
    /// Handles when the Resume button is pressed by unpausing
    /// </summary>
    public void HandleResume()
    {
        TogglePauseMenu();
    }



    ////////////////////////////////////////////////////////////////
    // Unity Methods
    ////////////////////////////////////////////////////////////////

    protected override void Awake()
    {
        base.Awake();
        this.defaultActive = false;
        Time.timeScale = 1;

        // Find the single PauseMenu object by tag
        instance = GameObject.FindGameObjectsWithTag("PauseMenu")[0].GetComponent<PauseMenu>();
    }
}