using System;
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

    private enum Texts
    {
        Speed,
        Time,
        Laps
    }

    /// <summary>
    /// Static reference to the one HUD object in the scene to enable static methods
    /// </summary>
    public static HUD instance;

    private static readonly Vector2 cursorRadius = new Vector2(0.02f, 0.03f);

    private const float velocityScale = 3.6f;

    private const float minSpeed = 0.5f;


    ////////////////////////////////////////////////////////////////
    // Methods
    ////////////////////////////////////////////////////////////////

    public static void UpdateCursor(Vector2 position)
    {
        instance.raws[Raws.Cursor.GetHashCode()].rectTransform.anchorMax = position + cursorRadius;
        instance.raws[Raws.Cursor.GetHashCode()].rectTransform.anchorMin = position - cursorRadius;
        instance.raws[Raws.Cursor.GetHashCode()].rectTransform.offsetMin = new Vector2();
        instance.raws[Raws.Cursor.GetHashCode()].rectTransform.offsetMax = new Vector2();
    }

    public static void UpdateSpeed(float speed)
    {
        if (speed < minSpeed)
        {
            speed = 0;
        }

        instance.texts[Texts.Speed.GetHashCode()].text = $"{(speed * velocityScale).ToString("0.0")} km";
    }

    public static void UpdateTime(TimeSpan time)
    {
        instance.texts[Texts.Time.GetHashCode()].text = time.ToString(@"mm\:ss\.fff");
    }

    public static void UpdateLaps(int numerator, int denominator)
    {
        instance.texts[Texts.Laps.GetHashCode()].text = numerator <= 0 ? string.Empty : $"{numerator}/{denominator}";
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