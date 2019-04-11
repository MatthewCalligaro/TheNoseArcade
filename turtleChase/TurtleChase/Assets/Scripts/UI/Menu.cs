using UnityEngine;
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
            return !PauseMenu.Paused && !LossMenu.GameOver;
        }
    }

    /// <summary>
    /// Menu items which can be interacted with through face controls
    /// </summary>
    protected IMenuItem[] items;

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

    /// <summary>
    /// Handles when the NewGame button is pressed by loading the main scene with a specified difficulty
    /// </summary>
    /// <param name="difficulty">The difficulty to apply to Settings</param>
    public void HandleNewGame(int difficulty)
    {
        Settings.Difficulty = (Difficulty)difficulty;
        this.MenuClose();
        SceneManager.LoadScene("Main");
    }

    /// <summary>
    /// Handles when the Tutorial button is pressed by loading the tutorial scene
    /// </summary>
    public void HandleTutorial()
    {
        this.MenuClose();
        SceneManager.LoadScene("Tutorial");
    }

    /// <summary>
    /// Handles when the Options button is pressed by opening the Options UI element
    /// </summary>
    public void HandleOptions()
    {
        OptionMenu.HandleOpen();
    }

    /// <summary>
    /// Handles when the user swipes their nose up by selecting the above item
    /// </summary>
    public void HandleUp()
    {
        this.items[this.curItem].HandleExit();
        this.curItem = (this.curItem - 1 + this.items.Length) % this.items.Length;
        this.items[this.curItem].HandleEnter();
    }

    /// <summary>
    /// Handles when the user swipes their nose down by selecting the below item
    /// </summary>
    public void HandleDown()
    {
        this.items[this.curItem].HandleExit();
        this.curItem = (this.curItem + 1 + this.items.Length) % this.items.Length;
        this.items[this.curItem].HandleEnter();
    }

    /// <summary>
    /// Handles when the user swipes their nose right by passing this on to the selected item
    /// </summary>
    public void HandleRight()
    {
        this.items[this.curItem].HandleRight();
    }

    /// <summary>
    /// Handles when the user swipes their nose left by passing this on to the selected item
    /// </summary>
    public void HandleLeft()
    {
        this.items[this.curItem].HandleLeft();
    }
    


    ////////////////////////////////////////////////////////////////
    // Unity Methods
    ////////////////////////////////////////////////////////////////

    protected override void Start()
    {
        base.Start();
        this.items = this.GetComponentsInChildren<IMenuItem>();
    }



    ////////////////////////////////////////////////////////////////
    // Protected Methods
    ////////////////////////////////////////////////////////////////

    /// <summary>
    /// Called when the Menu is opened to register itself with the Controller
    /// </summary>
    protected virtual void MenuOpen()
    {
        Controller.AddMenu(this);
        this.items[this.curItem].HandleEnter();
    }

    /// <summary>
    /// Called when the Menu is closed to remove itself from the Controller
    /// </summary>
    protected virtual void MenuClose()
    {
        this.items[this.curItem].HandleExit();
        Controller.RemoveMenu();
    }
}
