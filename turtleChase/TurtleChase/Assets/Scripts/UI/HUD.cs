using UnityEngine;
using UnityEngine.UI;

public class HUD : UIElement
{
    private enum Texts
    {
        Score
    }

    public static HUD instance;
    public static HUD Instance
    {
        get
        {
            return instance;
        }
    }

    protected void Awake()
    {
        this.defaultActive = true;
        instance = GameObject.FindGameObjectsWithTag("HUD")[0].GetComponent<HUD>();
    }

    public static void UpdateScore(int score)
    {
        Instance.texts[Texts.Score.GetHashCode()].text = "Score: " + score;
    }
}
