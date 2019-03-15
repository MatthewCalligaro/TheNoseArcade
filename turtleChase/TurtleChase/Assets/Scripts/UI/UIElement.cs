using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

/// <summary>
/// Defines generalized behavior for all UI elements
/// </summary>
public abstract class UIElement : MonoBehaviour
{
    /// <summary>
    /// True if Settings have been inititialized this play session (only happens once)
    /// </summary>
    protected static bool initializedSettings = false;

    /// <summary>
    /// True if the UI element should be active when a scene loads
    /// </summary>
    protected bool defaultActive;

    /// <summary>
    /// The Button objects that are children of the UIElement
    /// </summary>
    protected Button[] buttons;

    /// <summary>
    /// The Text objects that are children of the UIElement
    /// </summary>
    protected Text[] texts;

    /// <summary>
    /// The Dropdown objects that are children of the UIElement
    /// </summary>
    protected Dropdown[] dropdowns;

    /// <summary>
    /// The Slider objects that are children of the UIElement
    /// </summary>
    protected Slider[] sliders;



    ////////////////////////////////////////////////////////////////
    // Public Methods
    ////////////////////////////////////////////////////////////////

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
    /// Handles when the Options button is pressed by opening the Options UI element
    /// </summary>
    public void HandleOptions()
    {
        OptionMenu.HandleOpen();
    }



    ////////////////////////////////////////////////////////////////
    // Unity Methods
    ////////////////////////////////////////////////////////////////

    protected virtual void Awake()
    {
        this.buttons = this.GetComponentsInChildren<Button>();
        this.texts = this.GetComponentsInChildren<Text>();
        this.dropdowns = this.GetComponentsInChildren<Dropdown>();
        this.sliders = this.GetComponentsInChildren<Slider>();

        if (!initializedSettings)
        {
            Settings.RestoreDefaults();
            initializedSettings = true;
        }
    }

    protected virtual void Start()
    {
        this.gameObject.SetActive(defaultActive);
    }
}
