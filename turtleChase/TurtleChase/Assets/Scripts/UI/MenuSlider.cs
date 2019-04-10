using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// A menu button which can be selected by the face controller
/// </summary>
public class MenuSlider : MonoBehaviour, IMenuItem
{
    /// <summary>
    /// Color that the slider takes on to show that it is selected
    /// </summary>
    private static readonly Color selectedColor = new Color(0.75f, 1, 1);

    /// <summary>
    /// Unselected slider color
    /// </summary>
    private static readonly Color defaultColor = Color.white;

    /// <summary>
    /// Handles when the slider recieves focus by changing its color
    /// </summary>
    public void HandleEnter()
    {
        foreach (Image image in this.GetComponentsInChildren<Image>())
        {
            image.color = selectedColor;
        }
    }

    /// <summary>
    /// Handles when the slider loses focus by resetting its color
    /// </summary>
    public void HandleExit()
    {
        foreach (Image image in this.GetComponentsInChildren<Image>())
        {
            image.color = defaultColor;
        }
    }

    /// <summary>
    /// Handles when the slider is left swiped by decreasing the value
    /// </summary>
    public void HandleLeft()
    {
        this.GetComponent<Slider>().value = Mathf.Min(this.GetComponent<Slider>().value - 0.1f, 0);
    }

    /// <summary>
    /// Handles when the slider is right swiped by increasing the value
    /// </summary>
    public void HandleRight()
    {
        this.GetComponent<Slider>().value = Mathf.Min(this.GetComponent<Slider>().value + 0.1f, 1);
    }
}
