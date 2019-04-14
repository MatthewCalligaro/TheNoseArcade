/// <summary>
/// Defines a controller which takes the user's face position
/// </summary>
public interface INoseController
{
    /// <summary>
    /// Updates the controller with the current user's face position
    /// </summary>
    /// <param name="packed">32 bit signed int with bits 9-18 representing the x pixel and bits 19-28 representing the y pixel</param>
    void UpdateFacePosition(int packed);
}
