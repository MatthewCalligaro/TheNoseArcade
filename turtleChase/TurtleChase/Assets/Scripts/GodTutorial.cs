using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public enum TutorialTask
{
    Jump,
    Pause,
    MenuMove,
    MenuSelect,
    Distance,
    Consumable
}

public class GodTutorial : God
{
    protected override float DifficultyMultiplier
    {
        get
        {
            return tutorialDifficulty;
        }
    }

    private static readonly TutorialEvent[] events =
    {
        new TutorialEvent
        {
            Text = "Raise your nose or press the spacebar to jump",
            Task = TutorialTask.Jump,
            TaskNum = 3
        },

        new TutorialEvent
        {
            Text = "Press the escape key to pause",
            Task = TutorialTask.Pause,
            TaskNum = 1
        },

        new TutorialEvent
        {
            Text = "Left click the resume button with the mouse to resume",
            Task = TutorialTask.MenuSelect,
            TaskNum = 1
        },

        new TutorialEvent
        {
            Text = "Jump to avoid getting caught behind obstacles",
            Task = TutorialTask.Distance,
            Distance = 35,
            ObstacleIndices = new int[]{1, 1, 1},
            ObstaclePositions = new Vector3[]{new Vector3(10, -3.5f, envZ), new Vector3(20, -3, envZ), new Vector3(30, 0, envZ)},
            TaskNum = 1
        },

        new TutorialEvent
        {
            Text = $"You gain {consumableStats[Consumables.Pipe.GetHashCode()].Score} point{(consumableStats[Consumables.Pipe.GetHashCode()].Score > 1 ? "s" : "")} when you pass between pipe obstacles",
            Task = TutorialTask.Consumable,
            Distance = 35,
            ObstacleIndices = new int[]{0, 0, 0},
            ObstaclePositions = new Vector3[]{new Vector3(10, -2, envZ), new Vector3(20, 0, envZ), new Vector3(30, 2, envZ)},
            TaskNum = 3
        }
    };
    private const float tutorialDifficulty = 10;


    private static GodTutorial instance;

    private int taskCount = 0;
    private TutorialEvent curEvent;
    private int eventIndex;
    private float curEventStartX;

    private float finishX = float.MaxValue;


    public static void RegisterTask(TutorialTask task)
    {
        if (instance != null && task == instance.curEvent.Task)
        {
            instance.taskCount++;
            if (instance.taskCount >= instance.curEvent.TaskNum)
            {
                instance.LoadNextEvent();
            }
        }
    }

    private void LoadNextEvent()
    {
        eventIndex++;
        if (eventIndex >= events.Length)
        {
            HUD.UpdateTutorialText("Congratulations, you finished the Tutorial!");
            this.finishX = this.transform.position.x + 10;
        }
        else
        {
            taskCount = 0;
            curEvent = events[eventIndex];
            curEventStartX = this.transform.position.x;

            // Spawn obstacles and consumables for this event
            if (curEvent.ObstacleIndices != null)
            {
                for (int i = 0; i < curEvent.ObstacleIndices.Length; i++)
                {
                    SpawnObstaclePre(curEvent.ObstacleIndices[i], curEvent.ObstaclePositions[i] + Vector3.right * this.transform.position.x, forceStatic: true);
                }
            }

            if (curEvent.ConsumableIndices != null)
            {
                for (int i = 0; i < curEvent.ConsumableIndices.Length; i++)
                {
                    SpawnConsumable(curEvent.ConsumableIndices[i], curEvent.ConsumablePositions[i] + Vector3.right * this.transform.position.x);
                }
            }

            HUD.UpdateTutorialText(curEvent.Text);
        }
    }

    private void Awake()
    {
        this.generateLevel = false;
    }

    protected override void Start()
    {
        base.Start();
        instance = GameObject.FindGameObjectsWithTag("GameController")[0].GetComponent<GodTutorial>();

        this.eventIndex = -1;
        this.LoadNextEvent();
    }

    protected override void Update()
    {
        if (this.curEvent.Task == TutorialTask.Distance && this.transform.position.x - this.curEventStartX > this.curEvent.Distance)
        {
            this.LoadNextEvent();
        }

        if (this.transform.position.x > this.finishX)
        {
            SceneManager.LoadScene(0);
        }

        base.Update();
    }
}
