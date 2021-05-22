using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class JoinMenu : MonoBehaviour, IUIMenu
{
    [SerializeField] RoomListElement roomListElementPrefab;
    [SerializeField] MainMenu mainMenu;
    [SerializeField] ConnectionModel connectionModel;
    [SerializeField] Transform roomListParent;
    [SerializeField] TMPro.TextMeshProUGUI noRoomsAvailableText;

    public int ID { get; set; }

    public void Close()
    {
        gameObject.SetActive(false);
        connectionModel.AvailableRoomsChanged -= OnAvailableRoomsChanged;
        connectionModel.TryLeaveLobby();
    }

    public void Open()
    {
        gameObject.SetActive(true);
        RefreshRoomListElements();
        connectionModel.AvailableRoomsChanged += OnAvailableRoomsChanged;
        connectionModel.JoinDefaultLobby();

    }

    private void OnAvailableRoomsChanged()
    {
        RefreshRoomListElements();
    }

    private void RefreshRoomListElements()
    {
        for (int i = roomListParent.childCount - 1; i >= 0; i--)
        {
            Destroy(roomListParent.GetChild(i).gameObject);
        }

        var rooms = connectionModel.GetAllRooms();

        if (rooms == null)
        {
            noRoomsAvailableText.enabled = true;
        }
        else
        {
            noRoomsAvailableText.enabled = false;
            foreach (var room in rooms)
            {
                var element = Instantiate(roomListElementPrefab, roomListParent);
                element.Setup(room, connectionModel);
            }
        }
    }

    public void Back()
    {
        mainMenu.SwitchTo(MainMenu.MenuID.Main);
    }
}
