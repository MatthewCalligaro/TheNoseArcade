using UnityEngine;

/// <summary>
/// Version of player used during tutorial which registers tasks with GodTutorial
/// </summary>
public class PlayerTutorial : Player
{
    /// <summary>
    /// Farthest left that the player can fall before the camera lets the player catch up
    /// </summary>
    private const float minScreenX = 0.2f;

    /// <summary>
    /// Handles the jump command and registers a jump task with GodTutorial
    /// </summary>
    /// <param name="magnitude">Additional multiplier to apply to the jump's force</param>
    public override void JumpEnter(float magnitude = 1)
    {
        GodTutorial.RegisterTask(TutorialTask.Jump);
        base.JumpEnter(magnitude);
    }

    /// <summary>
    /// Handles the pause command and registers a pause task with GodTutorial
    /// </summary>
    public override void Pause()
    {
        GodTutorial.RegisterTask(TutorialTask.Pause);
        base.Pause();
    }

    protected override void Update()
    {
        base.Update();

        // Prevent the camera from moving forward if the player is behind minScreenX to prevent loss
        God.Stopped = God.CameraCoords(this.transform.position).x < minScreenX;
    }

    protected override void HandleCollision(GameObject other)
    {
        // Register a consumable task if we collided with a consumable
        if (other.gameObject.GetComponent<Environment>() && other.gameObject.GetComponent<Environment>().EnvironmentType == EnvironmentType.Consumable)
        {
            GodTutorial.RegisterTask(TutorialTask.Consumable, other.gameObject.GetComponent<Environment>().Consumable.Value);
        }
        base.HandleCollision(other);
    }
}
