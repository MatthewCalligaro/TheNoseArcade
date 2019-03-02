using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public abstract class Menu : MonoBehaviour
{
    protected bool defaultActive;
    protected Button[] buttons;
    protected Text[] texts;

    protected virtual void Start()
    {
        this.buttons = this.GetComponentsInChildren<Button>();
        this.texts = this.GetComponentsInChildren<Text>();
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
