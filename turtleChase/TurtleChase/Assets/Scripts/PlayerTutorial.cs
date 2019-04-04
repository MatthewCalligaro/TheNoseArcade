using UnityEngine;

public class PlayerTutorial : Player
{
    private const float minScreenX = 0.2f;

    public override void JumpEnter(float magnitude = 1)
    {
        GodTutorial.RegisterTask(TutorialTask.Jump);
        base.JumpEnter(magnitude);
    }

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
        if (other.gameObject.GetComponent<Environment>() && other.gameObject.GetComponent<Environment>().EnvironmentType == EnvironmentType.Consumable)
        {
            GodTutorial.RegisterTask(TutorialTask.Consumable, other.gameObject.GetComponent<Environment>().Consumable.Value);
        }
        base.HandleCollision(other);
    }
}
