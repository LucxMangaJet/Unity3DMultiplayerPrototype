using Photon.Realtime;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CreateMenu : MonoBehaviour, IUIMenu
{
    [SerializeField] MainMenu mainMenu;
    [SerializeField] ConnectionModel connectionModel;

    [SerializeField] TMPro.TextMeshProUGUI lobbyName;
    [SerializeField] TMPro.TextMeshProUGUI nameFeedback;

    public int ID { get; set; }

    public void Create()
    {
        RoomOptions options = new RoomOptions();
        options.MaxPlayers = 8;

        bool nameValid = IsRoomNameValid(lobbyName.text);

        if (nameValid)
        {
            string finalName = lobbyName.text;
            connectionModel.CreateRandom(finalName, options);
        }
    }

    private bool IsRoomNameValid(string text)
    {
        return text.Length >= 6 && !text.Contains(" ");
    }

    public void OnLobbyNameChanged(string text)
    {
        string msg = "";

        if (text.Length < 6)
        {
            msg = "Room name is too short.";
        }
        else if (text.Contains(" "))
        {
            msg = "Room name cannot contain spaces.";
        }

        nameFeedback.text = msg;
    }

    public void Back()
    {
        mainMenu.SwitchTo(MainMenu.MenuID.Main);
    }

    public void Open()
    {
        gameObject.SetActive(true);
    }

    public void Close()
    {
        gameObject.SetActive(false);
    }
}
