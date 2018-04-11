using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class NetworkMannager : MonoBehaviour
{
    public string versionNumber = "0.1.0";
    public static int LobbyLineNumber;
    public static string LName;

    public MenueControler LobbyMenue;

    private void Awake()
    {
        LobbyLineNumber = 0;
        LName = "TestLogs";
        PhotonNetwork.ConnectUsingSettings(versionNumber);
        PhotonNetwork.automaticallySyncScene = true;
        HandleTextFile.WriteLog((LobbyLineNumber += 1) + " Network Log: Initializing Server: " + "#SNG1", LName);
    }

    private void OnConnectedToMaster()
    {
        HandleTextFile.WriteLog((LobbyLineNumber += 1) + " Network Log: Player Connects Game : " + "#SNG1", LName);
        PhotonNetwork.JoinLobby(TypedLobby.Default);

    }

    private void OnJoinedLobby()
    {
        if (PhotonNetwork.countOfRooms == 0)
        {
            HandleTextFile.WriteLog((LobbyLineNumber += 1) + " Network Log: First Player Creates Game Room: " + "#SNG1", LName);
            PhotonNetwork.CreateRoom("Game", new RoomOptions() { MaxPlayers = 4 }, null);
            LobbyMenue.DisableJoin();
            LobbyMenue.EnablePlay();
        }
    }

    private void OnDisconnectedFromPhoton()
    {
    }

    public void OnJoinedRoom()
    {
        LobbyMenue.DisableJoin();
        PhotonPlayer[] players = PhotonNetwork.playerList;
        LobbyMenue.CleanList();
        for (int i = 0; i < players.Length; i++) { LobbyMenue.CreatePlayer(i); }
        HandleTextFile.WriteLog((LobbyLineNumber += 1) + " Network Log: Player Joins Game Room : " + "#SNG1", LName);
        Debug.Log(PhotonNetwork.player.ID);
    }

    public void OnPhotonPlayerConnected()
    {
        PhotonPlayer[] players = PhotonNetwork.playerList;
        HandleTextFile.WriteLog((LobbyLineNumber += 1) + " Network Log: Networked Player Joins the Game : " + "#SNG1", LName);
        LobbyMenue.CleanList();
        for (int i = 0; i < players.Length; i++) { LobbyMenue.CreatePlayer(i); }
        if (players.Length == 4)
        {
            HandleTextFile.WriteLog((LobbyLineNumber += 1) + " Network Log: Game Room Full launching the Game : " + "#SNG1", LName);
            PhotonNetwork.LoadLevel(1);
        }

    }
}
