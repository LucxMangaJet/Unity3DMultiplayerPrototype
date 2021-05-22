using Photon.Realtime;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class RoomListElement : MonoBehaviour
{
    [SerializeField] TMPro.TextMeshProUGUI roomName;
    [SerializeField] TMPro.TextMeshProUGUI roomAmount;
    [SerializeField] Button joinButton;


    public void Setup(RoomInfo room, ConnectionModel connectionModel)
    {
        roomName.text = room.Name;
        roomAmount.text = $"{room.PlayerCount}/{room.MaxPlayers}";
        joinButton.onClick.AddListener(delegate
        {
           connectionModel.JoinRoom(room.Name);

        });
    }
}
