using UnityEngine;
using UnityEngine.UI;

public class HUD : MonoBehaviour
{
    public Text Score;

    public static HUD instance;
    public static HUD Instance
    {
        get
        {
            return instance;
        }
    }

    private void Start()
    {
        instance = GameObject.FindGameObjectsWithTag("HUD")[0].GetComponent<HUD>();
    }

    public void UpdateScore(int score)
    {
        this.Score.text = "Score: " + score;
    }
}
