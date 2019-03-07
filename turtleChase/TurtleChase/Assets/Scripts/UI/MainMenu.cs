using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenu : UIElement
{
    private enum Dropdowns
    {
        Difficulty
    }

    private static MainMenu instance;
    public static MainMenu Instance
    {
        get
        {
            return instance;
        }
    }

    public void NewGame()
    {
        Settings.Difficulty = (Difficulty)this.dropdowns[Dropdowns.Difficulty.GetHashCode()].value;
        SceneManager.LoadScene(1);
    }

    public void CloseOptions()
    {
        Player.BlockingPause--;
        this.gameObject.SetActive(false);
    }

    protected override void Awake()
    {
        base.Awake();
        this.defaultActive = true;
        instance = GameObject.FindGameObjectsWithTag("MainMenu")[0].GetComponent<MainMenu>();
    }

    protected override void Start()
    {
        base.Start();
        this.dropdowns[Dropdowns.Difficulty.GetHashCode()].value = Settings.Difficulty.GetHashCode();
    }
}
