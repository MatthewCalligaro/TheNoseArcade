using UnityEngine;
using UnityEngine.UI;

public class LossMenu : UIElement
{
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
        Player.BlockingPause++;
        Time.timeScale = 0;
        Instance.gameObject.SetActive(true);
        Instance.texts[Texts.Score.GetHashCode()].text = "Score: " + score;
    }

    public override void HandleRestart()
    {
        base.HandleRestart();
    }

    protected override void Awake()
    {
        base.Awake();
        this.defaultActive = false;
        instance = GameObject.FindGameObjectsWithTag("LossMenu")[0].GetComponent<LossMenu>();
    }
}
