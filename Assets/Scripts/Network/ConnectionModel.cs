using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using System;
using Photon.Realtime;

public class ConnectionModel : MonoBehaviourPunCallbacks
{
    List<RoomInfo> availableRooms;

    public event System.Action<string> ConnectionError;
    public event System.Action AvailableRoomsChanged;
    public event System.Action JoinedRoom;
    public event System.Action RoomInfoChanged;

    public bool IsConnected { get => PhotonNetwork.IsConnectedAndReady; }
    public bool IsMasterClient { get => PhotonNetwork.IsMasterClient; }

    public Room CurrentRoom { get => PhotonNetwork.CurrentRoom; }

    public void Initiate()
    {
        PhotonNetwork.AutomaticallySyncScene = true;
        ConnectToServer();
    }


    public void ConnectToServer()
    {
        PhotonNetwork.SendRate = 50;
        PhotonNetwork.ConnectUsingSettings();
    }

    public void CreateRandom(string name, RoomOptions options = null)
    {
        PhotonNetwork.CreateRoom(name, options);
    }

    internal void JoinRandomRoom()
    {
        PhotonNetwork.JoinRandomRoom();
    }

    public void JoinRoom(string name)
    {
        PhotonNetwork.JoinRoom(name);
    }

    public void RenameLocalPlayerTo(string newName)
    {
        PhotonNetwork.LocalPlayer.NickName = newName;
        PlayerPrefs.SetString("Nickname", newName);
    }

    internal void JoinDefaultLobby()
    {
        PhotonNetwork.JoinLobby();
    }

    public void TryLeaveLobby()
    {
        if (PhotonNetwork.InLobby)
            PhotonNetwork.LeaveLobby();
    }

    public override void OnRoomListUpdate(List<RoomInfo> roomList)
    {
        Debug.Log("Room list updated");
        availableRooms = roomList;
        AvailableRoomsChanged?.Invoke();
    }

    public List<RoomInfo> GetAllRooms()
    {
        return availableRooms;
    }

    public void LeaveRoom()
    {
        PhotonNetwork.LeaveRoom();
    }

    public override void OnCreateRoomFailed(short returnCode, string message)
    {
        ConnectionError?.Invoke("Create Room Failed: " + message);
    }

    public override void OnJoinRandomFailed(short returnCode, string message)
    {
        ConnectionError?.Invoke("Join Random Room Failed: " + message);
    }

    public override void OnJoinRoomFailed(short returnCode, string message)
    {
        ConnectionError?.Invoke("Join Room Failed: " + message);
    }

    public override void OnJoinedRoom()
    {
        var name = PlayerPrefs.GetString("Nickname", "New Player");
        PhotonNetwork.LocalPlayer.NickName = name;
        JoinedRoom?.Invoke();
    }

    public override void OnPlayerEnteredRoom(Player newPlayer)
    {
        RoomInfoChanged?.Invoke();
    }

    public override void OnPlayerLeftRoom(Player otherPlayer)
    {
        RoomInfoChanged?.Invoke();
    }

    public override void OnPlayerPropertiesUpdate(Player targetPlayer, ExitGames.Client.Photon.Hashtable changedProps)
    {
        RoomInfoChanged?.Invoke();
    }

    public void StartGame()
    {
        if (PhotonNetwork.IsConnectedAndReady && PhotonNetwork.IsMasterClient)
        {
            PhotonNetwork.LoadLevel(1);
        }
    }

    public void Disconnect()
    {
        PhotonNetwork.Disconnect();
    }


}
