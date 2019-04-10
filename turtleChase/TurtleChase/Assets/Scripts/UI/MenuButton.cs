using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MenuButton : MonoBehaviour, IMenuItem
{
    private static readonly Color pressedColor = new Color(0.75f, 1, 1);
    private static readonly Color releasedColor = Color.white;

    public void HandleEnter()
    {
        this.GetComponent<Image>().color = pressedColor;
    }

    public void HandleExit()
    {
        this.GetComponent<Image>().color = releasedColor;
    }

    public void HandleLeft()
    {
        // Intentionally left blank
    }

    public void HandleRight()
    {
        this.GetComponent<Button>().onClick.Invoke();
    }
}
