using System;
using System.Collections.Generic;
using UnityEngine;

public class Controller : MonoBehaviour, INoseController
{
    private const float initalWaitTime = 0.5f;

    private Player player;

    private static Controller instance;

    private int lastX;
    private int lastY;

    private DateTime lastTime;

    private Stack<Menu> openMenus = new Stack<Menu>();

    private float counter = initalWaitTime;

    public static void SetPlayer(Player player)
    {
        instance.player = player;
    }

    public static void AddMenu(Menu menu)
    {
        instance.openMenus.Push(menu);
    }

    public static void RemoveMenu()
    {
        instance.openMenus.Pop();
    }

    public void UpdateFacePosition(int packed)
    {
        int x = packed & 0xFFFF;
        int y = (packed >> 16) & 0xFFFF;
        DateTime now = DateTime.Now;

        float dx = (x - lastX) / ((float)(now - lastTime).TotalMilliseconds);
        float dy = (y - lastY) / ((float)(now - lastTime).TotalMilliseconds);

        if (counter == 0)
        {
            if (dy > Settings.Sensitivity)
            {
                Up();
            }
            else if (dy < -Settings.Sensitivity)
            {
                Down();
            }
            else if (dx > Settings.Sensitivity)
            {
                Right();
            }
            else if (dx < -Settings.Sensitivity)
            {
                Left();            
            }
        }

        this.lastX = x;
        this.lastY = y;
        this.lastTime = now;
    }


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

    private void Up()
    {
        if (openMenus.Count > 0)
        {
            openMenus.Peek().HandleUp();
            counter = Settings.MenuMoveReloadTime;
        }
        else if (player != null)
        {
            player.JumpEnter();
            player.JumpExit();
            counter = Settings.JumpReloadTime;
        }
    }

    private void Down()
    {
        if (openMenus.Count > 0)
        {
            openMenus.Peek().HandleDown();
            counter = Settings.MenuMoveReloadTime;
        }
    }

    private void Right()
    {
        if (openMenus.Count > 0)
        {
            openMenus.Peek().HandleRight();
            counter = Settings.MenuMoveReloadTime;
        }
    }

    private void Left()
    {
        if (openMenus.Count > 0)
        {
            openMenus.Peek().HandleLeft();
            counter = Settings.MenuMoveReloadTime;
        }
        else if (player != null)
        {
            player.Pause();
            counter = Settings.JumpReloadTime;
        }
    }
}
