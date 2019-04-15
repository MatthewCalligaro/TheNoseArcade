using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// A menu button which can be selected by the face controller
/// </summary>
public class MenuButton : MonoBehaviour, IMenuItem
{
    /// <summary>
    /// Color that the button takes on to show that it is selected
    /// </summary>
    private static readonly Color selectedColor = new Color(0.75f, 1, 1);

    /// <summary>
    /// Unselected buttor color
    /// </summary>
    private static readonly Color defaultColor = Color.white;

    /// <summary>
    /// Handles when the button recieves focus by changing its color
    /// </summary>
    public void HandleEnter()
    {
        this.GetComponent<Image>().color = selectedColor;
    }

    /// <summary>
    /// Handles when the button loses focus by resetting its color
    /// </summary>
    public void HandleExit()
    {
        this.GetComponent<Image>().color = defaultColor;
    }

    /// <summary>
    /// Handles when the button is left swiped by doing nothing
    /// </summary>
    public void HandleLeft()
    {
        // Intentionally left blank
    }

    /// <summary>
    /// Handles when the button is right swiped by pressing it
    /// </summary>
    public void HandleRight()
    {
        this.GetComponent<Button>().onClick.Invoke();
    }
}
