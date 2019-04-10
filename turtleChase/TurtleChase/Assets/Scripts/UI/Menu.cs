﻿using UnityEngine;
using UnityEngine.SceneManagement;

/// <summary>
/// Defines generalized behavior for all menus
/// </summary>
public abstract class Menu : UIElement
{
    protected IMenuItem[] items;

    protected int curItem = 0;

    /// <summary>
    /// Handles when the Restart button is pressed by reloading the current scene
    /// </summary>
    public void HandleRestart()
    {
        Time.timeScale = 1;
        this.MenuClose();
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    /// <summary>
    /// Handles when the MainMenu button is pressed by loading the main menu scene
    /// </summary>
    public void HandleMainMenu()
    {
        Time.timeScale = 1;
        this.MenuClose();
        SceneManager.LoadScene("MainMenu");
    }

    /// <summary>
    /// Handles when the NewGame button is pressed by loading the main scene with a specified difficulty
    /// </summary>
    /// <param name="difficulty">The difficulty to apply to Settings</param>
    public void HandleNewGame(int difficulty)
    {
        Settings.Difficulty = (Difficulty)difficulty;
        this.MenuClose();
        SceneManager.LoadScene("Main");
    }

    /// <summary>
    /// Handles when the Tutorial button is pressed by loading the tutorial scene
    /// </summary>
    public void HandleTutorial()
    {
        this.MenuClose();
        SceneManager.LoadScene("Tutorial");
    }

    /// <summary>
    /// Handles when the Options button is pressed by opening the Options UI element
    /// </summary>
    public void HandleOptions()
    {
        OptionMenu.HandleOpen();
    }

    public void HandleUp()
    {
        this.items[this.curItem].HandleExit();
        this.curItem = (this.curItem - 1 + this.items.Length) % this.items.Length;
        this.items[this.curItem].HandleEnter();
    }

    public void HandleDown()
    {
        this.items[this.curItem].HandleExit();
        this.curItem = (this.curItem + 1 + this.items.Length) % this.items.Length;
        this.items[this.curItem].HandleEnter();
    }

    public void HandleRight()
    {
        this.items[this.curItem].HandleRight();
    }

    public void HandleLeft()
    {
        this.items[this.curItem].HandleLeft();
    }

    protected override void Start()
    {
        base.Start();
        this.items = this.GetComponentsInChildren<IMenuItem>();
    }

    protected virtual void MenuOpen()
    {
        Controller.AddMenu(this);
        this.items[this.curItem].HandleEnter();
    }

    protected virtual void MenuClose()
    {
        this.items[this.curItem].HandleExit();
        Controller.RemoveMenu();
    }
}
