using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Controller : MonoBehaviour
{
    private Player player;

    private static Controller instance;

    private int lastX;
    private int lastY;

    private Stack<Menu> openMenus = new Stack<Menu>();

    private float counter = 1;

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

        if (counter == 0)
        {
            if (y - this.lastY > Settings.Sensitivity)
            {
                Up();
            }
            else if (this.lastY - y > Settings.Sensitivity)
            {
                Down();
            }
            else if (x - this.lastX > Settings.Sensitivity)
            {
                Right();
            }
            else if (this.lastX - x > Settings.Sensitivity)
            {
                Left();            
            }
        }

        this.lastX = x;
        this.lastY = y;
    }


    void Start()
    {
        instance = GameObject.FindGameObjectsWithTag("GameController")[0].GetComponent<Controller>();
    }

    void Update()
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

        }
    }

    private void Right()
    {
        if (openMenus.Count > 0)
        {

        }
    }

    private void Left()
    {

    }
}
