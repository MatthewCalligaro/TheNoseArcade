using UnityEngine;
using UnityEngine.UI;

public class LossMenu : Menu
{
    private enum Buttons
    {
        Restart,
        MainMenu,
        Exit
    }

    private enum Texts
    {
        Title,
        Score
    }

    public static Menu instance;
    public static Menu Instance
    {
        get
        {
            return instance;
        }
    }

    public static void HandleLoss(int score)
    {
        Time.timeScale = 0;
        Instance.gameObject.SetActive(true);
        ((LossMenu)Instance).texts[Texts.Score.GetHashCode()].text = "Score: " + score;
    }

    protected void Awake()
    {
        this.defaultActive = false;
        instance = GameObject.FindGameObjectsWithTag("LossMenu")[0].GetComponent<Menu>();
    }
}
