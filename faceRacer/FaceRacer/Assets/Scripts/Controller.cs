using UnityEngine;

/// <summary>
/// Controls the game by processing nose coordinates from the ML model
/// </summary>
public class Controller : MonoBehaviour, INoseController
{
    /// <summary>
    /// The current screen coordinates of the cursor in the [0, 1] range
    /// </summary>
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
    /// Player object currently controlled by the Controller
    /// </summary>
    private Player player;

    /// <summary>
    /// X pixel position of the last recieved face position
    /// </summary>
    private int lastX;

    /// <summary>
    /// Y pixel position of the last recieved face position
    /// </summary>
    private int lastY;

    /// <summary>
    /// Time until the next action can occur
    /// </summary>
    private float counter = initalWaitTime;

    /// <summary>
    /// Current screen coordinates of the cursor in the [0, 1] range
    /// </summary>
    private Vector2 cursor = Settings.CursorStart;



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

        this.UpdateCursor(new Vector2((x - lastX) / Settings.Sensitivity.x, (y - lastY) / Settings.Sensitivity.y));

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

        // Use the mouse as a proxy for face position for debugging
        if (Input.GetKey(KeyCode.LeftShift) || Input.GetKey(KeyCode.RightShift))
        {
            UpdateFacePosition(
                ((int)(Input.mousePosition.x / Settings.MouseSensitivityMultiplier.x) & 0xFFFF) << 9 
                | (((int)(Input.mousePosition.y / Settings.MouseSensitivityMultiplier.y) & 0xFFFF) << 19));
        }

        // Space resets the cursor
        if (Input.GetKeyDown(KeyCode.Space))
        {
            this.ResetCursor();
        }
    }



    ////////////////////////////////////////////////////////////////
    // Private Methods
    ////////////////////////////////////////////////////////////////

    /// <summary>
    /// Moves the cursor by a certain amount
    /// </summary>
    /// <param name="delta">Amount to move the cursor</param>
    private void UpdateCursor(Vector2 delta)
    {
        if (counter == 0 && delta.x < Settings.MaxMovement.x && delta.y < Settings.MaxMovement.y)
        {
            this.cursor.x = Mathf.Min(1, Mathf.Max(0, this.cursor.x + delta.x));
            this.cursor.y = Mathf.Min(1, Mathf.Max(0, this.cursor.y + delta.y));
            HUD.UpdateCursor(this.cursor);
        }
    }

    /// <summary>
    /// Resets the cursor to its starting position
    /// </summary>
    public void ResetCursor()
    {
        this.cursor = Settings.CursorStart;
        HUD.UpdateCursor(this.cursor);
    }
}