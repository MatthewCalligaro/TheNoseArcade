using UnityEngine.SceneManagement;

/// <summary>
/// Defines generalized behavior for all menus
/// </summary>
public abstract class Menu : UIElement
{
    public static bool InPlay
    {
        get
        {
            return !PauseMenu.Paused && !WinMenu.GameOver;
        }
    }

    /// <summary>
    /// The current item selected by face controls
    /// </summary>
    protected int curItem = 0;



    ////////////////////////////////////////////////////////////////
    // Public Methods
    ////////////////////////////////////////////////////////////////

    /// <summary>
    /// Handles when the Restart button is pressed by reloading the current scene
    /// </summary>
    public void HandleRestart()
    {
        this.MenuClose();
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    /// <summary>
    /// Handles when the MainMenu button is pressed by loading the main menu scene
    /// </summary>
    public virtual void HandleMainMenu()
    {
        this.MenuClose();
        SceneManager.LoadScene("MainMenu");
    }

    public void HandleNewGame(string track)
    {
        this.MenuClose();
        SceneManager.LoadScene(track);
    }

    /// <summary>
    /// Handles when the Options button is pressed by opening the Options UI element
    /// </summary>
    public void HandleOptions()
    {
        OptionMenu.HandleOpen();
    }



    ////////////////////////////////////////////////////////////////
    // Protected Methods
    ////////////////////////////////////////////////////////////////

    /// <summary>
    /// Called when the Menu is opened to register itself with the Controller
    /// </summary>
    protected virtual void MenuOpen()
    {
        // Controller.AddMenu(this);
    }

    /// <summary>
    /// Called when the Menu is closed to remove itself from the Controller
    /// </summary>
    protected virtual void MenuClose()
    {
        // Controller.RemoveMenu();
    }
}