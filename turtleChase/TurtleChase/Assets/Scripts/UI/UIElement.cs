using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public abstract class UIElement : MonoBehaviour
{
    protected static bool initializedSettings = false;
    protected bool defaultActive;
    protected Button[] buttons;
    protected Text[] texts;
    protected Dropdown[] dropdowns;
    protected Slider[] sliders;

    protected virtual void Awake()
    {
        this.buttons = this.GetComponentsInChildren<Button>();
        this.texts = this.GetComponentsInChildren<Text>();
        this.dropdowns = this.GetComponentsInChildren<Dropdown>();
        this.sliders = this.GetComponentsInChildren<Slider>();

        if (!initializedSettings)
        {
            Settings.RestoreDefaults();
        }
    }

    protected virtual void Start()
    {
        this.gameObject.SetActive(defaultActive);
    }

    public void HandleRestart()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    public void LaunchMainMenu()
    {
        Debug.Log("TODO");
    }

    public void LaunchOptions()
    {
        OptionMenu.HandleLaunch();
    }
}
