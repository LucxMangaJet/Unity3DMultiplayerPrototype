using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using System;
using Photon.Realtime;
using ExitGames.Client.Photon;

public class GameHandler : MonoBehaviourPunCallbacks
{
    [SerializeField] GameObject playerCamera;


    private void Start()
    {
        if (!PhotonNetwork.IsConnected)
        {
            PhotonNetwork.OfflineMode = true;
            Debug.Log("Not connected, starting in offline mode.");
        }
        else
        {
            Debug.Log("Initializing...");
            Initialize();
        }
    }

    private void Initialize()
    {
        if (PhotonNetwork.IsConnected)
        {
            if (PhotonNetwork.IsMasterClient)
            {
                InitializeGame();
            }

            InitializePlayer();
        }
        else
        {
            Debug.LogError("Not connected to photon network");
        }
    }

    private void InitializePlayer()
    {
        if (playerCamera != null)
            PhotonNetwork.Instantiate(playerCamera.name, Vector3.zero, Quaternion.identity);
    }

    private void InitializeGame()
    {


    }

    public override void OnConnectedToMaster()
    {
        if (PhotonNetwork.OfflineMode)
        {
            PhotonNetwork.CreateRoom("OfflineMode");
        }
    }

    public override void OnCreatedRoom()
    {
        if (PhotonNetwork.OfflineMode)
        {
            Debug.Log("Offline mode. Initializing...");
            PhotonNetwork.LocalPlayer.NickName = "Local Player";
            Initialize();
        }
    }

    public static void Spawn(GameObject prefab, Vector3 position, Quaternion rotation = default(Quaternion))
    {
        PhotonNetwork.Instantiate(System.IO.Path.Combine("Spawnables", prefab.name), position, rotation);
    }

}
