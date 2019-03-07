using UnityEngine;

public class PauseMenu : UIElement
{
    private enum Buttons
    {
        Resume,
        Restart,
        MainMenu,
        Exit
    }

    private static PauseMenu instance;
    public static PauseMenu Instance
    {
        get
        {
            return instance;
        }
    }

    private static bool paused;
    public static void Pause()
    {
        paused = !paused;
        Time.timeScale = paused ? 0 : 1;
        Instance.gameObject.SetActive(paused);
    }
    public static void Pause(bool pause)
    {
        paused = !pause;
        Pause();
    }

    protected override void Awake()
    {
        base.Awake();
        this.defaultActive = false;
        instance = GameObject.FindGameObjectsWithTag("PauseMenu")[0].GetComponent<PauseMenu>();
    }

    protected override void Start()
    {
        base.Start();
        Pause(false);
    }

    public void HandleResume()
    {
        Pause();
    }
}
