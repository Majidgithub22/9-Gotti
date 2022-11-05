using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;
using Photon.Realtime;

public class Manager : Singleton<Manager> {
    public Camera Camera;
    public Text[] playerNames;
    public float p1X,p1Y,p1Z,p2X,p2Y,p2Z;
    public bool play,isSpawn=false;
    private int distanceBtwPlayers=-25;
    private int placePlayerCount;
    private GameObject[] walls;
    private GameObject[] moves;
    private PhotonView photonView;
    public void PlacePlayers() {
        placePlayerCount++;
        if (placePlayerCount >= 5) {
            EnableDragDrop();
            play = true;
        }
    }
    private void Start() {
        photonView= GetComponent<PhotonView>();
        walls = GameObject.FindGameObjectsWithTag("Wall");
        moves = GameObject.FindGameObjectsWithTag("Pillar");
            photonView.RPC("showName", RpcTarget.AllBuffered);
        StartPlacing();
        EnableWallColliders();
    }
    private void StartPlacing() {
        if (PhotonNetwork.IsMasterClient) {
            StartCoroutine(InstantiatePlayer("Player1", p1X, p1Y, p1Z));
        } else {
            StartCoroutine(InstantiatePlayer("Player2", p2X, p2Y, p2Z));
        }
       
    }
    public void EnableDragDrop() {
        GameObject[] players = GameObject.FindGameObjectsWithTag("Player");
        foreach (GameObject p in players) {
            p.GetComponent<DragDrop>().enabled = true;
        }
    }
    [PunRPC]
    private void StartGame() {
        GameObject[] players = GameObject.FindGameObjectsWithTag("Player");
        if (players.Length >=5) {
        Manager.Instance.isSpawn = true;
        }
    }
    public void DisableMoves() {
        foreach (GameObject m in moves) {
            m.GetComponent<CapsuleCollider>().enabled = false;
        }
    }
    public void FindMoves(string movename) {
        foreach (GameObject m in moves) {
            if (m.name == movename) {
                m.GetComponent<Slot>().status = 0;
            }
         //   m.GetComponent<CapsuleCollider>().enabled = false;
        }
    }
    public void DisableWallColliders() {
        foreach (GameObject w in walls) {
            w.GetComponent<BoxCollider>().enabled = false;
        }
    }
    public void EnableWallColliders() {
        foreach (GameObject w in walls) {
            w.GetComponent<BoxCollider>().enabled = true;
        }
    }
    [PunRPC]
    private void showName() {
        int i = 0;
        foreach(Player p in PhotonNetwork.PlayerList) {
            playerNames[i].text = p.NickName;
            i++;
        }
        }
    IEnumerator InstantiatePlayer(string player,float x,float y,float z) {
        int totalPlayer =0;
        yield return new WaitForSeconds(1);
        while (totalPlayer < 9) {
            PhotonNetwork.Instantiate(player,new Vector3(x,y,z), Quaternion.identity);
            z += distanceBtwPlayers;
            yield return new WaitForSeconds(0.6f);
            totalPlayer++;
        }
        photonView.RPC("StartGame", RpcTarget.AllBuffered);
    }
}
