using UnityEngine;
using UnityEngine.SceneManagement;

/// <summary>
/// Controller for the main menu
/// </summary>
public class MainMenu : UIElement
{
    /// <summary>
    /// The dropdowns contained in the main menu
    /// </summary>
    private enum Dropdowns
    {
        Difficulty
    }

    /// <summary>
    /// Static reference to the one MainMenu object in the scene to enable static methods
    /// </summary>
    private static MainMenu instance;



    ////////////////////////////////////////////////////////////////
    // Methods
    ////////////////////////////////////////////////////////////////

    /// <summary>
    /// Handles when the NewGame button is pressed by starting a game with the selected difficulty
    /// </summary>
    public void HandleNewGame()
    {
        Settings.Difficulty = (Difficulty)this.dropdowns[Dropdowns.Difficulty.GetHashCode()].value;
        SceneManager.LoadScene(1);
    }

    protected override void Awake()
    {
        base.Awake();
        this.defaultActive = true;

        // Find the single MainMenu object by tag
        instance = GameObject.FindGameObjectsWithTag("MainMenu")[0].GetComponent<MainMenu>();
    }

    protected override void Start()
    {
        #if !UNITY_EDITOR && UNITY_WEBGL
            WebGLInput.captureAllKeyboardInput = false;
        #endif
        base.Start();
        this.dropdowns[Dropdowns.Difficulty.GetHashCode()].value = Settings.Difficulty.GetHashCode();
    }
}
