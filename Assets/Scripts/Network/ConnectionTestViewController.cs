﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using System;

public class ConnectionTestViewController : MonoBehaviour
{
    [SerializeField] ConnectionModel connectionModel;

    private string nickname = "";

    private void Start()
    {
        nickname = PlayerPrefs.GetString("Nickname", "New Player");
        connectionModel.ConnectionError += OnError;
    }

    private void OnDestroy()
    {
        if (connectionModel != null)
            connectionModel.ConnectionError -= OnError;
    }

    private void OnError(string obj)
    {
        //lastError = obj;
    }

    private void OnGUI()
    {
        GUI.color = Color.white;
        GUI.skin.label.fontSize = 20;
        GUI.skin.button.fontSize = 20;
        GUI.skin.textField.fontSize = 20;

        GUILayout.Label("State: " + PhotonNetwork.NetworkClientState);

        switch (PhotonNetwork.NetworkClientState)
        {
            case ClientState.PeerCreated:
                if (GUILayout.Button("Connect"))
                {
                    connectionModel.ConnectToServer();
                }
                break;

            case ClientState.ConnectedToMasterServer:
                if (GUILayout.Button("Join Lobby"))
                {
                    connectionModel.JoinDefaultLobby();
                }
                break;

            case ClientState.JoinedLobby:
                GUILayout.Label("Lobby: " + PhotonNetwork.CurrentLobby.Name);

                if (GUILayout.Button("Create Random"))
                {
                    connectionModel.CreateRandom("Test " + UnityEngine.Random.Range(0, int.MaxValue));
                }
                if (GUILayout.Button("Join Random Random"))
                {
                    connectionModel.JoinRandomRoom();
                }

                foreach (var roomInfo in connectionModel.GetAllRooms())
                {
                    GUILayout.BeginHorizontal();
                    GUILayout.Label("Room: " + roomInfo.Name + "(" + roomInfo.PlayerCount + "/" + roomInfo.MaxPlayers + ")");

                    if (roomInfo.IsOpen)
                    {
                        if (GUILayout.Button("Join"))
                        {
                            connectionModel.JoinRoom(roomInfo.Name);
                        }
                    }

                    GUILayout.EndHorizontal();
                }

                break;

            case ClientState.Joined:
                var room = PhotonNetwork.CurrentRoom;

                GUILayout.Label(room.Name);
                GUILayout.Label("Players: " + room.PlayerCount);
                GUILayout.Label("-----");
                foreach (var pair in room.Players)
                {
                    string desc = pair.Key + ": " + pair.Value.NickName + (pair.Value.IsMasterClient ? " (M)" : "");
                    GUILayout.Label(desc);
                }

                GUILayout.Label("-----");
                GUILayout.Label("Settings:");

                GUILayout.BeginHorizontal();
                GUILayout.Label("Name:");
                var newNickname = GUILayout.TextField(nickname);
                if (newNickname != nickname)
                {
                    nickname = newNickname;
                    connectionModel.RenameLocalPlayerTo(nickname);
                }
                GUILayout.EndHorizontal();

                GUILayout.BeginHorizontal();


                if (PhotonNetwork.IsMasterClient)
                {
                    if (GUILayout.Button("Play!"))
                    {
                        connectionModel.StartGame();
                    }
                }
                break;
        }
    }

}
