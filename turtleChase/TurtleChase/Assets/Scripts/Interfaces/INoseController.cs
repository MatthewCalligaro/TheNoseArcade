/// <summary>
/// Defines a controller which takes the user's face position
/// </summary>
public interface INoseController
{
    /// <summary>
    /// Updates the controller with the current user's face position
    /// </summary>
    /// <param name="packed">32 bit signed int with the 16 least significant bits representing the x pixel and the 16 most significant bits representing the y pixel</param>
    void UpdateFacePosition(int packed);
}
