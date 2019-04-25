using System;
using System.Collections.Generic;
using UnityEngine;

public class Controller : MonoBehaviour, INoseController
{
    public static Vector2 Cursor
    {
        get
        {
            return instance.cursor;
        }
    }

    /// <summary>
    /// Time before the controller begins responding to input when first instantiated
    /// </summary>
    private const float initalWaitTime = 1.5f;

    /// <summary>
    /// Processing delay after a menu is opened or closed
    /// </summary>
    private const float menuDelay = 1.0f;

    /// <summary>
    /// Processing delay after a jump
    /// </summary>
    public const float jumpReloadTime = 0.5f;

    /// <summary>
    /// Processing delay after a menu is opened
    /// </summary>
    public const float menuMoveReloadTime = 1.0f;

    /// <summary>
    /// Static reference to the one Controller object in the scene to enable static methods
    /// </summary>
    private static Controller instance;

    /// <summary>
    /// The player object currently controlled by the Controller
    /// </summary>
    private Player player;

    /// <summary>
    /// The X pixel position of the last recieved face position
    /// </summary>
    private int lastX;

    /// <summary>
    /// The Y pixel position of the last recieved face position
    /// </summary>
    private int lastY;

    /// <summary>
    /// Time until the next action can occur
    /// </summary>
    private float counter = initalWaitTime;


    private Vector2 cursor = Settings.CursorStart;
    private Vector3 lastMousePosition;




    ////////////////////////////////////////////////////////////////
    // Public Methods
    ////////////////////////////////////////////////////////////////

    /// <summary>
    /// Sets the player controlled by the current Controller
    /// </summary>
    /// <param name="player">Player object for the controller to control</param>
    public static void SetPlayer(Player player)
    {
        instance.player = player;
    }

    /// <summary>
    /// Updates the controller with the current user's face position
    /// </summary>
    /// <param name="packed">32 bit signed int with bits 9-18 representing the x pixel and bits 19-28 representing the y pixel</param>
    public void UpdateFacePosition(int packed)
    {
        int x = (packed >> 9) & 0x3FF;
        int y = (packed >> 19) & 0x3FF;

        float dx = (x - lastX) / Settings.Sensitivity.x;
        float dy = (y - lastY) / Settings.Sensitivity.y;

        this.UpdateCursor(dx, dy);

        this.lastX = x;
        this.lastY = y;
    }



    ////////////////////////////////////////////////////////////////
    // Unity Methods
    ////////////////////////////////////////////////////////////////

    private void Awake()
    {
        instance = GameObject.FindGameObjectsWithTag("GameController")[0].GetComponent<Controller>();
    }

    private void Start()
    {
        HUD.UpdateCursor(this.cursor);
    }

    private void Update()
    {
        counter = Mathf.Max(counter - Time.deltaTime, 0);

        if (Input.GetKey(KeyCode.LeftShift) || Input.GetKey(KeyCode.RightShift))
        {
            UpdateFacePosition(
                ((int)(Input.mousePosition.x / Settings.MouseSensitivityMultiplier.x) & 0xFFFF) << 9 
                | (((int)(Input.mousePosition.y / Settings.MouseSensitivityMultiplier.y) & 0xFFFF) << 19));
        }

        if (Input.GetKeyDown(KeyCode.Space))
        {
            this.ResetCursor();
        }
    }



    ////////////////////////////////////////////////////////////////
    // Private Methods
    ////////////////////////////////////////////////////////////////

    private void UpdateCursor(float dx, float dy)
    {
        if (counter == 0 && dx < Settings.MaxMovement.x && dy < Settings.MaxMovement.y)
        {
            this.cursor.x = Mathf.Min(1, Mathf.Max(0, this.cursor.x + dx));
            this.cursor.y = Mathf.Min(1, Mathf.Max(0, this.cursor.y + dy));
            HUD.UpdateCursor(this.cursor);
        }
    }

    public void ResetCursor()
    {
        this.cursor = Settings.CursorStart;
        HUD.UpdateCursor(this.cursor);
    }
}