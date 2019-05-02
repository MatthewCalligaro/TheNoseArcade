﻿using System;
using System.Collections.Generic;
using UnityEngine;

public class Controller : MonoBehaviour, INoseController
{
    /// <summary>
    /// Time before the controller begins responding to input when first instantiated
    /// </summary>
    private const float initalWaitTime = 1.5f;

    /// <summary>
    /// Processing delay after a menu is opened or closed
    /// </summary>
    private const float menuDelay = 1.0f;

    /// <summary>
    /// Processing delay after a jump
    /// </summary>
    public const float jumpReloadTime = 0.5f;

    /// <summary>
    /// Processing delay after a menu is opened
    /// </summary>
    public const float menuMoveReloadTime = 1.0f;

    /// <summary>
    /// Static reference to the one Controller object in the scene to enable static methods
    /// </summary>
    private static Controller instance;

    /// <summary>
    /// The player object currently controlled by the Controller
    /// </summary>
    private Player player;

    /// <summary>
    /// Menus currently open with the "top" menu on top
    /// </summary>
    private Stack<Menu> openMenus = new Stack<Menu>();

    /// <summary>
    /// The X pixel position of the last recieved face position
    /// </summary>
    private int lastX;

    /// <summary>
    /// The Y pixel position of the last recieved face position
    /// </summary>
    private int lastY;

    /// <summary>
    /// The time at which the last face position was recieved
    /// </summary>
    private DateTime lastTime;

    /// <summary>
    /// Time until the next action can occur
    /// </summary>
    private float counter = initalWaitTime;




    ////////////////////////////////////////////////////////////////
    // Public Methods
    ////////////////////////////////////////////////////////////////

    /// <summary>
    /// Sets the player controlled by the current Controller
    /// </summary>
    /// <param name="player">Player object for the controller to control</param>
    public static void SetPlayer(Player player)
    {
        instance.player = player;
    }

    /// <summary>
    /// Adds a Menu for the Controller to control
    /// </summary>
    /// <param name="menu">The newest menu to control</param>
    public static void AddMenu(Menu menu)
    {
        instance.openMenus.Push(menu);
        instance.counter = menuDelay;
    }

    /// <summary>
    /// Removes the highest level menu from the Controller
    /// </summary>
    public static void RemoveMenu()
    {
        instance.openMenus.Pop();
        instance.counter = menuDelay;
    }

    /// <summary>
    /// Updates the controller with the current user's face position
    /// </summary>
    /// <param name="packed">32 bit signed int with bits 9-18 representing the x pixel and bits 19-28 representing the y pixel</param>
    public void UpdateFacePosition(int packed)
    {
        int x = (packed >> 9) & 0x3FF;
        int y = (packed >> 19) & 0x3FF;
        DateTime now = DateTime.Now;

        float dx = (x - lastX) / ((float)(now - lastTime).TotalMilliseconds);
        float dy = (y - lastY) / ((float)(now - lastTime).TotalMilliseconds);
        Debug.Log(dy);

        if (counter == 0 && Mathf.Abs(dy) < Settings.IgnoreSensitivity && Mathf.Abs(dx) < Settings.IgnoreSensitivity * Settings.HorizontalSensitivityFactor)
        {
            if (dy > Settings.Sensitivity)
            {
                Up();
            }
            else if (dy < -Settings.Sensitivity)
            {
                Down();
            }
            else if (dx > Settings.Sensitivity * Settings.HorizontalSensitivityFactor)
            {
                Right();
            }
            else if (dx < -Settings.Sensitivity * Settings.HorizontalSensitivityFactor)
            {
                Left();            
            }
        }

        this.lastX = x;
        this.lastY = y;
        this.lastTime = now;
    }



    ////////////////////////////////////////////////////////////////
    // Unity Methods
    ////////////////////////////////////////////////////////////////

    private void Awake()
    {
        instance = GameObject.FindGameObjectsWithTag("GameController")[0].GetComponent<Controller>();
    }

    private void Update()
    {
        counter = Mathf.Max(counter - Time.deltaTime, 0);

        if (Input.GetKey(KeyCode.LeftShift) || Input.GetKey(KeyCode.RightShift))
        {
            UpdateFacePosition((((int)Input.mousePosition.x) & 0xFFFF) | ((((int)Input.mousePosition.y) & 0xFFFF) << 16));
        }
    }



    ////////////////////////////////////////////////////////////////
    // Private Methods
    ////////////////////////////////////////////////////////////////

    /// <summary>
    /// Handles an upward face swipe (jump or menu action)
    /// </summary>
    private void Up()
    {
        if (openMenus.Count > 0)
        {
            openMenus.Peek().HandleUp();
            counter = menuMoveReloadTime;
        }
        else if (player != null)
        {
            player.JumpEnter();
            player.JumpExit();
            counter = jumpReloadTime;
        }
    }

    /// <summary>
    /// Handles a downward face swipe (menu action)
    /// </summary>
    private void Down()
    {
        if (openMenus.Count > 0)
        {
            openMenus.Peek().HandleDown();
            counter = menuMoveReloadTime;
        }
    }

    /// <summary>
    /// Handles a right face swipe (menu action)
    /// </summary>
    private void Right()
    {
        if (openMenus.Count > 0)
        {
            openMenus.Peek().HandleRight();
            counter = menuMoveReloadTime;
        }
    }
    
    /// <summary>
    /// Handles a left face swipe (pause or menu action)
    /// </summary>
    private void Left()
    {
        if (openMenus.Count > 0)
        {
            openMenus.Peek().HandleLeft();
            counter = menuMoveReloadTime;
        }
        else if (player != null)
        {
            player.Pause();
            counter = jumpReloadTime;
        }
    }
}
