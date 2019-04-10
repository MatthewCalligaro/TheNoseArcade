using UnityEngine;

/// <summary>
/// Controller for the main menu
/// </summary>
public class MainMenu : Menu
{
    /// <summary>
    /// Static reference to the one MainMenu object in the scene to enable static methods
    /// </summary>
    private static MainMenu instance;

    protected override void Awake()
    {
        base.Awake();
        this.defaultActive = true;

        // Find the single MainMenu object by tag
        instance = GameObject.FindGameObjectsWithTag("MainMenu")[0].GetComponent<MainMenu>();

        // MenuButtons are the only type of interactable MenuItem in MainMenu
        this.items = this.GetComponentsInChildren<MenuButton>();
    }

    protected override void Start()
    {
        base.Start();
        Controller.AddMenu(this);
        this.items[this.curItem].HandleEnter();
    }
}
