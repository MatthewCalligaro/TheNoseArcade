using UnityEngine;

/// <summary>
/// Controller for the pause menu
/// </summary>
public class PauseMenu : Menu
{
    /// <summary>
    /// Static reference to the one PauseMenu object in the scene to enable static methods
    /// </summary>
    private static PauseMenu instance;

    /// <summary>
    /// True if the game is currently paused
    /// </summary>
    private bool paused = false;

    private GameObject tutorialHelper;



    ////////////////////////////////////////////////////////////////
    // Public Methods
    ////////////////////////////////////////////////////////////////

    /// <summary>
    /// Toggles whether the game is paused or unpaused
    /// </summary>
    public static void Pause()
    {
        instance.paused = !instance.paused;
        instance.gameObject.SetActive(instance.paused);

        if (instance.paused)
        {
            instance.MenuOpen();
            Time.timeScale = 0;
        }
        else
        {
            instance.MenuClose();
            Time.timeScale = 1;
        }
    }
    
    /// <summary>
    /// Handles when the Resume button is pressed by unpausing
    /// </summary>
    public void HandleResume()
    {
        GodTutorial.RegisterTask(TutorialTask.PressPause);
        this.tutorialHelper.SetActive(false);
        Pause();
    }

    public override void HandleMainMenu()
    {
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

        this.tutorialHelper = this.transform.Find("TutorialHelper").gameObject;
        this.tutorialHelper.SetActive(false);
    }
}
