using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum TutorialTask
{
    Jump,
    Pause,
    MenuMove,
    MenuSelect,
    Consumable
}

public class GodTutorial : God
{
    private static readonly TutorialEvent[] events =
    {
        new TutorialEvent
        {
            Text = "Jump",
            Task = TutorialTask.Jump,
            TaskNum = 3
        }
    };

    private static GodTutorial Instance;

    private int taskCount = 0;
    private TutorialEvent curEvent;
    private int eventIndex = 0;


    public static void RegisterTask(TutorialTask task)
    {
        if (task == Instance.curEvent.Task)
        {
            Instance.taskCount++;
            if (Instance.taskCount >= Instance.curEvent.TaskNum)
            {
                Instance.LoadNextEvent();
            }
        }
    }

    private void LoadNextEvent()
    {
        eventIndex++;
        if (eventIndex >= events.Length)
        {
            // Finish Tutorial
        }
        else
        {
            taskCount = 0;
            curEvent = events[eventIndex];

            // Spawn obstacles and consumables for this event
            for (int i = 0; i < curEvent.ObstacleIndices.Length; i++)
            {
                SpawnObstacle(curEvent.ObstacleIndices[i], curEvent.ObstaclePositions[i]);
            }

            for (int i = 0; i < curEvent.ConsumableIndices.Length; i++)
            {
                SpawnConsumable(curEvent.ConsumableIndices[i], curEvent.ConsumablePositions[i]);
            }
        }
    }

    private void Awake()
    {
        this.generateLevel = false;
    }

}
