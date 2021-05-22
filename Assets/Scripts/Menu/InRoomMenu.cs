using Photon.Realtime;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class InRoomMenu : MonoBehaviour, IUIMenu
{
    [SerializeField] MainMenu mainMenu;
    [SerializeField] ConnectionModel connectionModel;

    [SerializeField] TMPro.TextMeshProUGUI roomName;
    [SerializeField] TMPro.TextMeshProUGUI roomInfo;

    [SerializeField] GameObject startGameButton;

    public int ID { get; set; }
    public void Close()
    {
        gameObject.SetActive(false);
    }

    public void Open()
    {
        gameObject.SetActive(true);

        RefreshRoomInfo();
        connectionModel.RoomInfoChanged += RefreshRoomInfo;
    }

    public void Leave()
    {
        connectionModel.LeaveRoom();
        mainMenu.SwitchToMain();
        connectionModel.RoomInfoChanged -= RefreshRoomInfo;
    }

    public void RefreshRoomInfo()
    {
        startGameButton.SetActive(connectionModel.IsMasterClient);
        Room currentRoom = connectionModel.CurrentRoom;

        roomName.text = currentRoom.Name;
        roomInfo.text = string.Join(", ", currentRoom.Players.Select(p => p.Value.NickName).ToArray());
    }

    public void Rename(string newName)
    {
        connectionModel.RenameLocalPlayerTo(newName);
    }


}
