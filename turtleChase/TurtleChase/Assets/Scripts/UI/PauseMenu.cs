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
            Controller.AddMenu(instance);
            Time.timeScale = 0;
            instance.items[instance.curItem].HandleEnter();
        }
        else
        {
            Controller.RemoveMenu();
            Time.timeScale = 1;
        }
    }
    
    /// <summary>
    /// Handles when the Resume button is pressed by unpausing
    /// </summary>
    public void HandleResume()
    {
        GodTutorial.RegisterTask(TutorialTask.MenuSelect);
        Pause();
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

        // MenuButtons are the only type of interactable MenuItem in PauseMenu
        this.items = this.GetComponentsInChildren<MenuButton>();
    }
}
