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
    private static bool paused;



    ////////////////////////////////////////////////////////////////
    // Public Methods
    ////////////////////////////////////////////////////////////////

    /// <summary>
    /// Toggles whether the game is paused or unpaused
    /// </summary>
    public static void Pause()
    {
        paused = !paused;
        Time.timeScale = paused ? 0 : 1;
        instance.gameObject.SetActive(paused);
    }

    /// <summary>
    /// Pauses or unpauses the game
    /// </summary>
    /// <param name="pause">True to pause the game, false to unpause</param>
    public static void Pause(bool pause)
    {
        paused = !pause;
        Pause();
    }
    
    /// <summary>
    /// Handles when the Resume button is pressed by unpausing
    /// </summary>
    public void HandleResume()
    {
        GodTutorial.RegisterTask(TutorialTask.MenuSelect);
        Pause(false);
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
    }

    protected override void Start()
    {
        base.Start();
        Pause(false);
    }
}
