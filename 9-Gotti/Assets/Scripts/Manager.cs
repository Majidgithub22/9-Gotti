using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;
using Photon.Realtime;

public class Manager : Singleton<Manager> {
    public Camera Camera;
    public Text[] playerNames;
    public Text[] TimeText;
    public float p1X,p1Y,p1Z,p2X,p2Y,p2Z;
    public bool play,isSpawn=false;
    private int distanceBtwPlayers=-25;
    private int placePlayerCount;
    private GameObject[] walls;
    private GameObject[] moves;
    private PhotonView photonView;
    public bool isLineFormed;
    public bool isP1Turn=true;
    private void Start() {
        photonView= GetComponent<PhotonView>();
        walls = GameObject.FindGameObjectsWithTag("Wall");
        moves = GameObject.FindGameObjectsWithTag("Pillar");
            photonView.RPC("showName", RpcTarget.AllBuffered);
        //StartPlacing();
        DisplayUserTime();
    }
    #region PlayerFunctions
    //DisplayTime for each player
    public void DisplayUserTime() {
        if (PhotonNetwork.IsMasterClient&&isP1Turn) {
            isSpawn = true;
            PhotonNetwork.Instantiate("Player1", new Vector3(p1X, p1Y, p1Z), Quaternion.identity);
            StartCoroutine(DisplayTime(false));
          //  photonView.RPC("PlacePlayers", RpcTarget.Others);
        } else if(!isP1Turn&&!PhotonNetwork.IsMasterClient) {
            isSpawn = true;
            PhotonNetwork.Instantiate("Player2", new Vector3(p2X, p2Y, p2Z), Quaternion.identity);
            StartCoroutine(DisplayTime(true));
           // photonView.RPC("PlacePlayers", RpcTarget.Others);
        }
    }
    //Check Player  count
   
    public void PlacePlayers() {
        placePlayerCount++;
        if (placePlayerCount >= 18) {// If player is in room.
            EnableDragDrop();
            play = true;
        } 
    }
    //start placeing players on basis of there positions
    //private void StartPlacing() {
    //    if (PhotonNetwork.IsMasterClient) {
    //        StartCoroutine(InstantiatePlayer("Player1", p1X, p1Y, p1Z));
    //    } else {
    //        StartCoroutine(InstantiatePlayer("Player2", p2X, p2Y, p2Z));
    //    }

    //}
    //Enable drag drp for each player.
    public void EnableDragDrop() {
        GameObject[] players = GameObject.FindGameObjectsWithTag("Player");
        foreach (GameObject p in players) {
            p.GetComponent<DragDrop>().enabled = true;
        }
    }
    //Update player destroyable status
    public void PlayerDestroyStatus(int id,bool isdest) {
        GameObject[] players = GameObject.FindGameObjectsWithTag("Player");
        foreach (GameObject p in players) {
            if (p.GetComponent<PhotonView>().ViewID == id) {
                p.GetComponent<DragDrop>().isDestroyable = isdest;
            }
        }
    }
    public void DestroyPlayer(int id ) {
        GameObject[] players = GameObject.FindGameObjectsWithTag("Player");
        foreach (GameObject p in players) {
            if (p.GetComponent<PhotonView>().ViewID == id) {
                p.GetComponent<DragDrop>().parent.GetComponent<MeshRenderer>().enabled = true;
                p.GetComponent<DragDrop>().parent.GetComponent<Slot>().status = 0;
                Destroy(p.gameObject);
            }
        }
    }
    #endregion PlayerFunctions
    public void DisableMoves() {
        foreach (GameObject m in moves) {
            m.GetComponent<CapsuleCollider>().enabled = false;
        }
    }
    public void SetPillarStatus(string movename, int stats) {
        foreach (GameObject m in moves) {
            if (m.name == movename) {
                m.GetComponent<Slot>().status = stats;
            }
        }
    }
    public void SetParentMesh(string movename, bool isEnable) {
        foreach (GameObject m in moves) {
            if (m.name == movename) {
                m.GetComponent<MeshRenderer>().enabled = isEnable;
            }
        }
    }
    //check wall gotti statusss
    public void WallGottiCheck() {
        foreach (GameObject w in walls) {
            w.GetComponent<Wall>().SetGottiDestroyableStatus();
        }
    }
    #region RPC
    //Display names of both Players
    [PunRPC]
    private void showName() {
        int i = 0;
        foreach(Player p in PhotonNetwork.PlayerList) {
            playerNames[i].text = p.NickName;
            i++;
        }
        }
    //Check If all players are instantiated then moving player enable.
    [PunRPC]
    private void StartGame() {
       // GameObject[] players = GameObject.FindGameObjectsWithTag("Player");
        //if (players.Length >= 18) {
        //    Manager.Instance.isSpawn = true;
        //}
    }//Check If all players are instantiated then moving player enable.
    [PunRPC]
    private void SetMyTurn(bool isTurn) {
        isP1Turn = isTurn;
        DisplayUserTime();
    }
    [PunRPC]
    private void ShowTime(bool isTurn,int sec) {
        if (isTurn)
        TimeText[1].text = sec.ToString();
        else
        TimeText[0].text = sec.ToString();
    }
    #endregion RPC
    #region Coroutines
    //Spawn Player for each User
    //IEnumerator InstantiatePlayer(string player,float x,float y,float z) {
    //    int totalPlayer =0;
    //    yield return new WaitForSeconds(1);
    //    while (totalPlayer < 9) {
    //        PhotonNetwork.Instantiate(player,new Vector3(x,y,z), Quaternion.identity);
    //        z += distanceBtwPlayers;
    //        yield return new WaitForSeconds(0.6f);
    //        totalPlayer++;
    //    }
    //    photonView.RPC("StartGame", RpcTarget.All);
    //}
    IEnumerator DisplayTime(bool isTurn) {
        int i = 0;
        while (i < 10) {
            yield return new WaitForSeconds(1);
            i++;
            photonView.RPC("ShowTime", RpcTarget.All,isTurn, i);
        }
        isSpawn = false;
        photonView.RPC("SetMyTurn",RpcTarget.All, isTurn);
    }
    #endregion Coroutines
}
