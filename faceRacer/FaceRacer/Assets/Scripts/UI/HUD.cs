using System;
using UnityEngine;

/// <summary>
/// Controller for the heads up display
/// </summary>
public class HUD : UIElement
{
    /// <summary>
    /// RawImage objects contained in the HUD
    /// </summary>
    private enum Raws
    {
        Cursor
    }

    /// <summary>
    /// Text objects contained in the HUD
    /// </summary>
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

    /// <summary>
    /// Length and width of the cursor
    /// </summary>
    private static readonly Vector2 cursorSize = new Vector2(0.02f, 0.03f);

    /// <summary>
    /// Amount to scale the velocity from game units to km/hour
    /// </summary>
    private const float velocityScale = 3.6f;

    /// <summary>
    /// Velocities below this value are shown as 0
    /// </summary>
    private const float minSpeed = 0.5f;


    ////////////////////////////////////////////////////////////////
    // Methods
    ////////////////////////////////////////////////////////////////

    /// <summary>
    /// Updates the position of the cursor
    /// </summary>
    /// <param name="position">New position of the cursor in the [0, 1] range</param>
    public static void UpdateCursor(Vector2 position)
    {
        instance.raws[Raws.Cursor.GetHashCode()].rectTransform.anchorMax = position + cursorSize;
        instance.raws[Raws.Cursor.GetHashCode()].rectTransform.anchorMin = position - cursorSize;
        instance.raws[Raws.Cursor.GetHashCode()].rectTransform.offsetMin = new Vector2();
        instance.raws[Raws.Cursor.GetHashCode()].rectTransform.offsetMax = new Vector2();
    }

    /// <summary>
    /// Updates the speed
    /// </summary>
    /// <param name="speed">Player's velocity in game units</param>
    public static void UpdateSpeed(float speed)
    {
        if (speed < minSpeed)
        {
            speed = 0;
        }

        instance.texts[Texts.Speed.GetHashCode()].text = $"{(speed * velocityScale).ToString("0.0")} km/h";
    }

    /// <summary>
    /// Updates the time spent in the current race
    /// </summary>
    /// <param name="time">Time spent in the current race</param>
    public static void UpdateTime(TimeSpan time)
    {
        instance.texts[Texts.Time.GetHashCode()].text = time.ToString(@"mm\:ss\.fff");
    }

    /// <summary>
    /// Updates the lap information
    /// </summary>
    /// <param name="numerator">Number of laps completed</param>
    /// <param name="denominator">Total laps of the race</param>
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