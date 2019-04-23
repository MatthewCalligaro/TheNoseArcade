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

    /// <summary>
    /// Static reference to the one PauseMenu object in the scene to enable static methods
    /// </summary>
    private static PauseMenu instance;

    /// <summary>
    /// True if the game is currently paused
    /// </summary>
    private bool paused = false;



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
        }
        else
        {
            instance.MenuClose();
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

        // Find the single PauseMenu object by tag
        instance = GameObject.FindGameObjectsWithTag("PauseMenu")[0].GetComponent<PauseMenu>();
    }
}