/// <summary>
/// Defines a menu item which the user can interact with via face controls
/// </summary>
public interface IMenuItem
{
    /// <summary>
    /// Method called when the user swipes their nose to the left on this item
    /// </summary>
    void HandleLeft();

    /// <summary>
    /// Method called when the user swipes their nose to the left on this item
    /// </summary>
    void HandleRight();

    /// <summary>
    /// Method called when the user toggles to this item
    /// </summary>
    void HandleEnter();

    /// <summary>
    /// Method called when the user toggles off of this item
    /// </summary>
    void HandleExit();
}
