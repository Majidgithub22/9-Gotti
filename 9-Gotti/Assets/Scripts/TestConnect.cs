using Photon.Pun;
using Photon.Realtime;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class TestConnect : MonoBehaviourPunCallbacks {

    public Session session;
    public ServerSettings serverSettings;
    public Text displayInfo;
    private void Start() {
        if (!PhotonNetwork.IsConnected) {
            displayInfo.text = "Wait! Connecting to Server";
            PhotonNetwork.AutomaticallySyncScene = true;
            PhotonNetwork.GameVersion = "1.0";
            serverSettings.DevRegion = "us";
            displayInfo.text= "Region found "+serverSettings.DevRegion;
            PhotonNetwork.NickName = session.name;
            PhotonNetwork.ConnectUsingSettings();
        }
        if (Application.internetReachability == NetworkReachability.NotReachable) {
            displayInfo.text ="Error. Check internet connection!";
        }
        if (PhotonNetwork.InLobby) {
           OnClick_CreateRoom(); 
        }
        
       
    }
    public void RefreshButton() {
        Start();
    }
    public override void OnConnectedToMaster() {
        if (!PhotonNetwork.InLobby) {
            displayInfo.text = "Joining Lobby........ ";
            PhotonNetwork.ConnectToRegion("us;");
            PhotonNetwork.JoinLobby();
        }
    }
    public override void OnJoinedLobby() {
        displayInfo.text = "Connected to Server ";
        OnClick_CreateRoom();
    }
    public void OnClick_CreateRoom() {
        displayInfo.text = "In Lobby";
        if (!PhotonNetwork.IsConnected) return;
        RoomOptions options = new RoomOptions();
        options.MaxPlayers =2;
        PhotonNetwork.JoinOrCreateRoom("Race", options, TypedLobby.Default);
    }
    public override void OnCreatedRoom() {
        displayInfo.text = "Room Created";
      //  OnCLick_StartGame();
    }
    public override void OnJoinedRoom() {
        displayInfo.text = "Room Joined";
        OnCLick_StartGame();
    }
    public void OnCLick_StartGame() {
        //    PhotonNetwork.CurrentRoom.IsOpen = false;
         //   PhotonNetwork.CurrentRoom.IsVisible = false;
            PhotonNetwork.AutomaticallySyncScene = true;
            PhotonNetwork.LoadLevel(1);
        //    Debug.Log("loadingScene");
        }

    public override void OnDisconnected(DisconnectCause cause) {
        // info.text = "disconnceted from server for reason " + cause.ToString();
        // Debug.Log("disconnceted from server for reason " + cause.ToString());
    }
   
}
