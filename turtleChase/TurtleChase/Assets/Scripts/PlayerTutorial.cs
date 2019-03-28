using UnityEngine;

public class PlayerTutorial : Player
{
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

    protected override void HandleCollision(GameObject other)
    {
        if (other.gameObject.GetComponent<Environment>() && other.gameObject.GetComponent<Environment>().EnvironmentType == EnvironmentType.Consumable)
        {
            GodTutorial.RegisterTask(TutorialTask.Consumable);
        }
        base.HandleCollision(other);
    }
}
