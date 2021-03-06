﻿using UnityEngine;

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
    
    /// <summary>
    /// Static reference to the one PauseMenu object in the scene to enable static methods
    /// </summary>
    private static PauseMenu instance;

    /// <summary>
    /// True if the game is currently paused
    /// </summary>
    private bool paused = false;

    /// <summary>
    /// UI elements which help direct the user to the resume button in the tutorial
    /// </summary>
    private GameObject tutorialHelper;



    ////////////////////////////////////////////////////////////////
    // Public Methods
    ////////////////////////////////////////////////////////////////

    /// <summary>
    /// Toggles whether the pause menu is opened or closed
    /// </summary>
    public static void TogglePauseMenu()
    {
        if (!LossMenu.GameOver)
        {
            instance.paused = !instance.paused;
            instance.gameObject.SetActive(instance.paused);

            if (instance.paused)
            {
                instance.MenuOpen();
            }
            else
            {
                instance.MenuClose();
            }
        }
    }
    
    /// <summary>
    /// Handles when the Resume button is pressed by unpausing
    /// </summary>
    public void HandleResume()
    {
        GodTutorial.RegisterTask(TutorialTask.PressPause);
        this.tutorialHelper.SetActive(false);
        TogglePauseMenu();
    }

    /// <summary>
    /// Handles when the MainMenu button is pressed by returning to the main menu
    /// </summary>
    public override void HandleMainMenu()
    {
        // First make sure that we are not being blocked by the tutorial
        if (GodTutorial.BlockingMainMenu)
        {
            this.tutorialHelper.SetActive(true);
        }
        else
        {
            base.HandleMainMenu();
        }
    }



    ////////////////////////////////////////////////////////////////
    // Unity Methods
    ////////////////////////////////////////////////////////////////

    protected override void Awake()
    {
        base.Awake();
        this.defaultActive = false;

        // Find the single PauseMenu object by tag
        instance = GameObject.FindGameObjectsWithTag("PauseMenu")[0].GetComponent<PauseMenu>();

        // Find the tutorialHelper gameobject (which is one of our children)
        this.tutorialHelper = this.transform.Find("TutorialHelper").gameObject;
        this.tutorialHelper.SetActive(false);
    }
}
