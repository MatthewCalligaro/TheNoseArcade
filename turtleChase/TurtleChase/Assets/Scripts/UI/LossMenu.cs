using UnityEngine;
using UnityEngine.UI;

public class LossMenu : UIElement
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

    public static LossMenu instance;
    public static LossMenu Instance
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
        Instance.texts[Texts.Score.GetHashCode()].text = "Score: " + score;
    }

    protected void Awake()
    {
        this.defaultActive = false;
        instance = GameObject.FindGameObjectsWithTag("LossMenu")[0].GetComponent<LossMenu>();
    }
}
