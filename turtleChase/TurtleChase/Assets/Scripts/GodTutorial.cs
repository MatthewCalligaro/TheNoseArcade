﻿using UnityEngine;
using UnityEngine.SceneManagement;

/// <summary>
/// Types of tasks which the player can be required to perform in the tutorial
/// </summary>
public enum TutorialTask
{
    Jump,
    Pause,
    MenuMove,
    PressPause,
    Distance,
    Consumable
}

/// <summary>
/// Version of God used during the tutorial
/// </summary>
public class GodTutorial : God
{
    ////////////////////////////////////////////////////////////////
    // Properties
    ////////////////////////////////////////////////////////////////

    /// <summary>
    /// Constant difficulty multiplier used during tutorial
    /// </summary>
    protected override float DifficultyMultiplier
    {
        get
        {
            return 10;
        }
    }

    /// <summary>
    /// Whether the tutorial blocks the PauseMenu's MainMenu button
    /// </summary>
    public static bool BlockingMainMenu
    {
        get
        {
            return instance != null && instance.curEventIndex == 2;
        }
    }

    ////////////////////////////////////////////////////////////////
    // Fields
    ////////////////////////////////////////////////////////////////

    /// <summary>
    /// Static reference to the one GodTutorial object in the scene to enable static methods
    /// </summary>
    private static GodTutorial instance;

    /// <summary>
    /// Constant delay between consumable objects in tutorial
    /// </summary>
    private const float consumableDelay = 8;

    /// <summary>
    /// Time until we provide the user with additional help text for jumping
    /// </summary>
    private const float jumpHelpTextDelay = 8;

    /// <summary>
    /// Message shown to the user if they cannot successfully jump after a certain amount of time
    /// </summary>
    private const string jumpHelpText = "If you are having trouble registering a jump, try pausing the game (escape key) and changing the jump sensitivity in the options menu";

    /// <summary>
    /// Defines the events which the player must complete in the tutorial
    /// </summary>
    private static readonly TutorialEvent[] events =
    {
        new TutorialEvent
        {
            Text = "Raise your nose (or press the spacebar) to jump.  This works best if you move the position of your face rather then rotating it",
            TaskCountText = "Remaining jumps",
            Task = TutorialTask.Jump,
            TaskNum = 3
        },

        new TutorialEvent
        {
            Text = "Swipe your nose left (or press the escape key) to pause the game",
            Task = TutorialTask.Pause,
            TaskNum = 1
        },

        new TutorialEvent
        {
            Text = "Toggle which button is selected by swiping your nose up and down and press the selected button by swiping right.  Press the resume button now",
            Task = TutorialTask.PressPause,
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
            TaskCountText = "Remaining silver to collect",
            Task = TutorialTask.Consumable,
            RequiredConsumable = Consumables.Silver,
            ConsumableIndex = Consumables.Silver.GetHashCode(),
            TaskNum = 2
        },

        new TutorialEvent
        {
            Text = $"You gain {consumableStats[Consumables.Gold.GetHashCode()].Score} point{(consumableStats[Consumables.Silver.GetHashCode()].Score > 1 ? "s" : string.Empty)} and a larger boost when you collect gold",
            TaskCountText = "Remaining gold to collect",
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

    /// <summary>
    /// Current tutorial event
    /// </summary>
    private TutorialEvent curEvent;

    /// <summary>
    /// Index of the current event in events
    /// </summary>
    private int curEventIndex;

    /// <summary>
    /// Times the current task has been completed
    /// </summary>
    private int taskCount = 0;

    /// <summary>
    /// X position of God at the start of this event
    /// </summary>
    private float curEventStartX;

    /// <summary>
    /// The tutorial ends when God passes this X position
    /// </summary>
    private float finishX = float.MaxValue;

    /// <summary>
    /// X position at which God will spawn the next consumable object
    /// </summary>
    private float nextConsumableX;

    /// <summary>
    /// Counts down time until we provide the user with additional help text for jumping
    /// </summary>
    private float jumpHelpCounter = jumpHelpTextDelay;



    ////////////////////////////////////////////////////////////////
    // Public Methods
    ////////////////////////////////////////////////////////////////

    /// <summary>
    /// Registers when the player completes a certain task
    /// </summary>
    /// <param name="task">The task which the player completed</param>
    public static void RegisterTask(TutorialTask task)
    {
        if (instance != null && task == instance.curEvent.Task)
        {
            instance.taskCount++;

            // Clear jump help text if they sucessfully jumped
            if (task == TutorialTask.Jump)
            {
                HUD.UpdateTutorialHelpText("");
            }

            // If this was the last time they needed to repeat this task, load the next one
            if (instance.taskCount >= instance.curEvent.TaskNum)
            {
                instance.LoadNextEvent();
            }

            // Otherwise update the HUD task counter with the number of remaining repetitions
            else if (instance.curEvent.TaskCountText != null)
            {
                HUD.UpdateTutorialTaskCountText(instance.curEvent.TaskCountText, instance.curEvent.TaskNum - instance.taskCount);
            }
        }
    }

    /// <summary>
    /// Registers when the player completes a certain task
    /// </summary>
    /// <param name="task">The task which the player completed</param>
    /// <param name="consumable">The consumable which the player collected</param>
    public static void RegisterTask(TutorialTask task, Consumables consumable)
    {
        // Only register the task if the player collected the correct consumable
        if (instance != null && instance.curEvent.RequiredConsumable.HasValue && consumable == instance.curEvent.RequiredConsumable.Value)
        {
            RegisterTask(task);
        }
    }

    /// <summary>
    /// Begins the next tutorial event
    /// </summary>
    private void LoadNextEvent()
    {
        this.curEventIndex++;
        if (this.curEventIndex == events.Length) // Intentionally not >= so that repeating the final task does not reset finishX
        {
            HUD.UpdateTutorialText("Congratulations, you finished the Tutorial!");
            this.finishX = this.transform.position.x + 10;
        }
        else if (this.curEventIndex < events.Length)
        {
            this.taskCount = 0;
            this.curEvent = events[this.curEventIndex];
            this.curEventStartX = this.transform.position.x;
            this.nextConsumableX = this.transform.position.x;

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

            // Update the instruction text and counter text if necessary
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


    ////////////////////////////////////////////////////////////////
    // Unity Methods
    ////////////////////////////////////////////////////////////////

    private void Awake()
    {
        this.generateLevel = false;
    }

    protected override void Start()
    {
        base.Start();
        instance = GameObject.FindGameObjectsWithTag("God")[0].GetComponent<GodTutorial>();

        this.curEventIndex = -1;
        this.LoadNextEvent();
    }

    protected override void Update()
    {
        // Provide jump help text if the user is unable to jump for a while
        if (this.curEventIndex == 0 && this.taskCount == 0 && this.jumpHelpCounter > 0)
        {
            this.jumpHelpCounter -= Time.deltaTime;
            if (this.jumpHelpCounter <= 0)
            {
                HUD.UpdateTutorialHelpText(jumpHelpText);
            }
        }
        
        // Handle completion of Distance events
        if (this.curEvent.RequiredDistance.HasValue && this.transform.position.x - this.curEventStartX > this.curEvent.RequiredDistance)
        {
            this.LoadNextEvent();
        }

        if (this.transform.position.x > this.finishX)
        {
            SceneManager.LoadScene("MainMenu");
        }

        // Spawn the next consumable if we surpassed nextConsumableX
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
