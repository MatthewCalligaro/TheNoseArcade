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
            TaskCountText = "Remaining jumps",
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
            RequiredDistance = 35,
            ObstacleIndices = new int[]{Obstacles.Rock.GetHashCode(), Obstacles.Rock.GetHashCode(), Obstacles.Rock.GetHashCode()},
            ObstaclePositions = new Vector3[]{new Vector3(10, -3.5f, envZ), new Vector3(20, -3, envZ), new Vector3(30, 0, envZ)},
            ObstacleMovement = false,
            TaskNum = 1
        },

        new TutorialEvent
        {
            Text = $"You gain {consumableStats[Consumables.Pipe.GetHashCode()].Score} point{(consumableStats[Consumables.Pipe.GetHashCode()].Score > 1 ? "s" : string.Empty)} when you pass between pipe obstacles",
            TaskCountText = "Remaining pipes",
            Task = TutorialTask.Consumable,
            RequiredConsumable = Consumables.Pipe,
            ObstacleIndices = new int[]{Obstacles.Pipe.GetHashCode(), Obstacles.Pipe.GetHashCode(), Obstacles.Pipe.GetHashCode()},
            ObstaclePositions = new Vector3[]{new Vector3(10, -2, envZ), new Vector3(20, 0, envZ), new Vector3(30, 2, envZ)},
            ObstacleMovement = false,
            TaskNum = 3
        },

        new TutorialEvent
        {
            Text = $"You gain {consumableStats[Consumables.Silver.GetHashCode()].Score} point{(consumableStats[Consumables.Silver.GetHashCode()].Score > 1 ? "s" : string.Empty)} and a small boost when you collect silver",
            TaskCountText = "Remaining silver",
            Task = TutorialTask.Consumable,
            RequiredConsumable = Consumables.Silver,
            ConsumableIndex = Consumables.Silver.GetHashCode(),
            TaskNum = 2
        },

        new TutorialEvent
        {
            Text = $"You gain {consumableStats[Consumables.Gold.GetHashCode()].Score} point{(consumableStats[Consumables.Silver.GetHashCode()].Score > 1 ? "s" : string.Empty)} and a larger boost when you collect gold",
            TaskCountText = "Remaining gold",
            Task = TutorialTask.Consumable,
            RequiredConsumable = Consumables.Gold,
            ConsumableIndex = Consumables.Gold.GetHashCode(),
            TaskNum = 2
        },

        new TutorialEvent
        {
            Text = $"The speed power-up temporarily increases your speed, allowing you to make up lost ground",
            Task = TutorialTask.Consumable,
            RequiredConsumable = Consumables.Speed,
            ConsumableIndex = Consumables.Speed.GetHashCode(),
            TaskNum = 1
        },

        new TutorialEvent
        {
            Text = $"The pillow knocks you backward and should normally be avoided, but try hitting one now to see how it works",
            Task = TutorialTask.Consumable,
            RequiredConsumable = Consumables.Knockback,
            ConsumableIndex = Consumables.Knockback.GetHashCode(),
            TaskNum = 1
        }
    };

    private const float tutorialDifficulty = 10;

    private const float consumableDelay = 5;


    private static GodTutorial instance;

    private int taskCount = 0;
    private TutorialEvent curEvent;
    private int eventIndex;
    private float curEventStartX;

    private float finishX = float.MaxValue;

    private float nextConsumableX;


    public static void RegisterTask(TutorialTask task)
    {
        if (instance != null && task == instance.curEvent.Task)
        {
            instance.taskCount++;
            if (instance.taskCount >= instance.curEvent.TaskNum)
            {
                instance.LoadNextEvent();
            }
            else if (instance.curEvent.TaskCountText != null)
            {
                HUD.UpdateTutorialTaskCountText(instance.curEvent.TaskCountText, instance.curEvent.TaskNum - instance.taskCount);
            }
        }
    }
    public static void RegisterTask(TutorialTask task, Consumables consumable)
    {
        if (instance != null && instance.curEvent.RequiredConsumable.HasValue && consumable == instance.curEvent.RequiredConsumable.Value)
        {
            RegisterTask(task);
        }
    }

    private void LoadNextEvent()
    {
        this.eventIndex++;
        if (eventIndex >= events.Length)
        {
            HUD.UpdateTutorialText("Congratulations, you finished the Tutorial!");
            this.finishX = this.transform.position.x + 10;
        }
        else
        {
            this.taskCount = 0;
            this.curEvent = events[this.eventIndex];
            this.curEventStartX = this.transform.position.x;

            // Spawn obstacles for this event
            if (this.curEvent.ObstacleIndices != null)
            {
                for (int i = 0; i < this.curEvent.ObstacleIndices.Length; i++)
                {
                    this.SpawnObstaclePre(
                        this.curEvent.ObstacleIndices[i], 
                        this.curEvent.ObstaclePositions[i] + Vector3.right * this.transform.position.x, 
                        forceMove: this.curEvent.ObstacleMovement);
                }
            }

            this.nextConsumableX = this.transform.position.x;
            HUD.UpdateTutorialText(this.curEvent.Text);
            if (this.curEvent.TaskCountText == null)
            {
                HUD.ClearTutorialTaskCountText();
            }
            else
            {
                HUD.UpdateTutorialTaskCountText(this.curEvent.TaskCountText, this.curEvent.TaskNum - this.taskCount);
            }
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
        if (this.curEvent.RequiredDistance.HasValue && this.transform.position.x - this.curEventStartX > this.curEvent.RequiredDistance)
        {
            this.LoadNextEvent();
        }

        if (this.transform.position.x > this.finishX)
        {
            SceneManager.LoadScene(0);
        }

        if (this.curEvent.ConsumableIndex.HasValue && this.transform.position.x >= this.nextConsumableX)
        {
            this.SpawnConsumable(
                this.curEvent.ConsumableIndex.Value, 
                new Vector3(this.transform.position.x + envXLead, Random.Range(-envMaxY, envMaxY), envZ));
            this.nextConsumableX += consumableDelay;
        }

        base.Update();
    }
}
