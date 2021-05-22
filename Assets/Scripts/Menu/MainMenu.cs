using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IUIMenu
{
    public int ID { get; set; }
    void Open();
    void Close();
}

public class MainMenu : MonoBehaviour
{

    [SerializeField] ConnectionModel connectionModel;
    [SerializeField] int playSceneIndex = 1;

    [SerializeField] MenuIdPair[] menusDefinitions;

    List<IUIMenu> menus;


    private void Start()
    {
        if (!connectionModel)
            throw new UnityEngine.UnassignedReferenceException("connectionModel");

        connectionModel.Initiate();

        SetupMenus();
    }

    private void SetupMenus()
    {
        menus = new List<IUIMenu>();

        foreach (var m in menusDefinitions)
        {
            if (m.Menu.TryGetComponent(out IUIMenu uiMenu))
            {
                uiMenu.ID = (int)m.Id;
                menus.Add(uiMenu);
            }
            else
            {
                menus.Add(new SimpleOnOffMenu(m.Id, m.Menu));
            }
        }
    }

    private void SwitchTo(MenuID menuId)
    {
        bool found = false;

        for (int i = 0; i < menus.Count; i++)
        {
            var menu = menus[i];
            if (menu.ID == (int)menuId)
            {
                menu.Open();
                found = true;
            }
            else
            {
                menu.Close();
            }
        }

        if (!found)
        {
            Debug.LogError($"No menu with id {menuId} found. Reverting to Main");
            SwitchTo(MenuID.Main);
        }
    }

    public void Join()
    {
        SwitchTo(MenuID.Join);
    }

    public void Create()
    {
        SwitchTo(MenuID.Create);
    }

    public void Offline()
    {
        connectionModel.Disconnect();
        UnityEngine.SceneManagement.SceneManager.LoadScene(playSceneIndex);
    }

    public void Quit()
    {
        Application.Quit();
    }

    public enum MenuID
    {
        Main,
        Join,
        Create,
        InRoom,
        Error
    }

    [System.Serializable]
    public struct MenuIdPair
    {
        public MenuID Id;
        public GameObject Menu;
    }

    public class SimpleOnOffMenu : IUIMenu
    {
        private GameObject parentObject;
        private int id;

        public int ID { get => id; set => id = value; }

        public SimpleOnOffMenu(int id, GameObject parent)
        {
            this.id = id;
            parentObject = parent;
        }

        public SimpleOnOffMenu(MenuID menuId, GameObject parent)
        {
            this.id = (int)menuId;
            parentObject = parent;
        }

        public void Close()
        {
            parentObject?.SetActive(false);
        }

        public void Open()
        {
            parentObject?.SetActive(true);
        }
    }
}

