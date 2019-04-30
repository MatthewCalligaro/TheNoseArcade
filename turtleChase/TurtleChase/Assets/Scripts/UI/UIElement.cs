using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Defines generalized behavior for all UI elements
/// </summary>
public abstract class UIElement : MonoBehaviour
{
    /// <summary>
    /// True if the UI element should be active when a scene loads
    /// </summary>
    protected bool defaultActive;

    /// <summary>
    /// The Button objects that are children of the UIElement
    /// </summary>
    protected Button[] buttons;

    /// <summary>
    /// The Text objects that are children of the UIElement
    /// </summary>
    protected Text[] texts;

    /// <summary>
    /// The Dropdown objects that are children of the UIElement
    /// </summary>
    protected Dropdown[] dropdowns;

    /// <summary>
    /// The Slider objects that are children of the UIElement
    /// </summary>
    protected Slider[] sliders;



    ////////////////////////////////////////////////////////////////
    // Unity Methods
    ////////////////////////////////////////////////////////////////

    protected virtual void Awake()
    {
        this.buttons = this.GetComponentsInChildren<Button>();
        this.texts = this.GetComponentsInChildren<Text>();
        this.dropdowns = this.GetComponentsInChildren<Dropdown>();
        this.sliders = this.GetComponentsInChildren<Slider>();
    }

    protected virtual void Start()
    {
        this.gameObject.SetActive(defaultActive);
    }
}
