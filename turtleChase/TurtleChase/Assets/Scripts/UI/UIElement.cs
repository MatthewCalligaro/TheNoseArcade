using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public abstract class UIElement : MonoBehaviour
{
    protected bool defaultActive;
    protected Button[] buttons;
    protected Text[] texts;

    protected virtual void Awake()
    {
        this.buttons = this.GetComponentsInChildren<Button>();
        this.texts = this.GetComponentsInChildren<Text>();
    }

    protected virtual void Start()
    {
        this.gameObject.SetActive(defaultActive);
    }

    public void HandleRestart()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    public void HandleMainMenu()
    {
        Debug.Log("TODO");
    }

    public void HandleExit()
    {
        Application.Quit();
    }
}
