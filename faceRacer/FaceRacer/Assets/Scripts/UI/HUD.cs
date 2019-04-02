using UnityEngine;

/// <summary>
/// Controller for the heads up display
/// </summary>
public class HUD : UIElement
{
    /// <summary>
    /// The RawImage objects contained in the HUD
    /// </summary>
    private enum Raws
    {
        Cursor
    }

    /// <summary>
    /// Static reference to the one HUD object in the scene to enable static methods
    /// </summary>
    public static HUD instance;

    private static readonly Vector2 cursorRadius = new Vector2(0.02f, 0.03f);


    ////////////////////////////////////////////////////////////////
    // Methods
    ////////////////////////////////////////////////////////////////

    public static void UpdateCursor(Vector2 position)
    {
        Debug.Log(position);
        instance.raws[Raws.Cursor.GetHashCode()].rectTransform.anchorMax = position + cursorRadius;
        instance.raws[Raws.Cursor.GetHashCode()].rectTransform.anchorMin = position - cursorRadius;
        instance.raws[Raws.Cursor.GetHashCode()].rectTransform.offsetMin = new Vector2();
        instance.raws[Raws.Cursor.GetHashCode()].rectTransform.offsetMax = new Vector2();
    }

    protected override void Awake()
    {
        base.Awake();
        this.defaultActive = true;
        instance = GameObject.FindGameObjectsWithTag("HUD")[0].GetComponent<HUD>();
    }

    protected override void Start()
    {
        base.Start();
    }
}