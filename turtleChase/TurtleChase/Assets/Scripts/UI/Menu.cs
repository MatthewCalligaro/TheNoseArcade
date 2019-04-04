using UnityEngine.SceneManagement;

/// <summary>
/// Defines generalized behavior for all menus
/// </summary>
public abstract class Menu : UIElement
{
    /// <summary>
    /// Handles when the Restart button is pressed by reloading the current scene
    /// </summary>
    public void HandleRestart()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    /// <summary>
    /// Handles when the MainMenu button is pressed by loading the main menu scene
    /// </summary>
    public void HandleMainMenu()
    {
        SceneManager.LoadScene(0);
    }

    /// <summary>
    /// Handles when the NewGame button is pressed by loading the main scene with a specified difficulty
    /// </summary>
    /// <param name="difficulty">The difficulty to apply to Settings</param>
    public void HandleNewGame(int difficulty)
    {
        Settings.Difficulty = (Difficulty)difficulty;
        SceneManager.LoadScene(1);
    }

    /// <summary>
    /// Handles when the Tutorial button is pressed by loading the tutorial scene
    /// </summary>
    public void HandleTutorial()
    {
        SceneManager.LoadScene(2);
    }

    /// <summary>
    /// Handles when the Options button is pressed by opening the Options UI element
    /// </summary>
    public void HandleOptions()
    {
        OptionMenu.HandleOpen();
    }
}
