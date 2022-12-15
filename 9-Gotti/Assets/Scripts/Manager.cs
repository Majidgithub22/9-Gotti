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
    public float p1X, p1Y, p1Z, p2X, p2Y, p2Z;
    public bool play, isSpawn = false;
    private int distanceBtwPlayers = -25;
    private int placePlayerCount;
    private GameObject[] walls;
    private GameObject[] moves;
    public List<GameObject> emptyMovesList=new List<GameObject>();
    private PhotonView photonView;
    public bool isLineFormed;
    public bool isP1Turn = true;
    public bool takeTurn;
    private GameObject Player1, Player2;
    List<GameObject> destroyableOpponent = new List<GameObject>();

    private void Start() {
        photonView = GetComponent<PhotonView>();
        walls = GameObject.FindGameObjectsWithTag("Wall");
        moves = GameObject.FindGameObjectsWithTag("Pillar");
        photonView.RPC("showName", RpcTarget.AllBuffered);
        DisplayUserTime();
    }
    #region PlayerFunctions
    //DisplayTime for each player
    public void DisplayUserTime() {
        if (PhotonNetwork.IsMasterClient && isP1Turn&&!play) {
            isSpawn = true;
           Player1= PhotonNetwork.Instantiate("Player1", new Vector3(p1X, p1Y, p1Z), Quaternion.identity);
            StartCoroutine(DisplayTime(false,Player1,true));
        } else if (!isP1Turn && !PhotonNetwork.IsMasterClient&&!play) {
            isSpawn = true;
           Player2= PhotonNetwork.Instantiate("Player2", new Vector3(p2X, p2Y, p2Z), Quaternion.identity);
            StartCoroutine(DisplayTime(true,Player2,false));
        }
        //if (play) {
        //    //update players status to true
        //    EnablePlayerTouch(true);
        //}
    }
    //Check Player  count

    public void PlacePlayers() {
        placePlayerCount++;
        playerNames[3].text = placePlayerCount.ToString();
        if (placePlayerCount >= 18) {// If player is in room.
            EnableDragDrop();
            play = true;
        }
    }

    //Enable drag drp for each player.
    public void EnableDragDrop() {
        GameObject[] players = GameObject.FindGameObjectsWithTag("Player");
        foreach (GameObject p in players) {
            p.GetComponent<DragDrop>().enabled = true;
        }
    }
    public void EnablePlayerTouch(bool t) {
        GameObject[] players = GameObject.FindGameObjectsWithTag("Player");
        foreach (GameObject p in players) {
            if(p.GetComponent<PhotonView>().IsMine)
            p.GetComponent<DragDrop>().isTouch = t;
        }
    }
    //Update player destroyable status
    public void PlayerDestroyStatus(int id, bool isdest) {
        GameObject[] players = GameObject.FindGameObjectsWithTag("Player");
        foreach (GameObject p in players) {
            if (p.GetComponent<PhotonView>().ViewID == id) {
                p.GetComponent<DragDrop>().isDestroyable = isdest;
            }
        }
    }
    //Don't let playermove by Owner and other destroy it until its not placed.
    public void PlayerIsTouch(int id, bool stats) {
        GameObject[] players = GameObject.FindGameObjectsWithTag("Player");
        foreach (GameObject p in players) {
            if (p.GetComponent<PhotonView>().ViewID == id) {
                p.GetComponent<DragDrop>().isTouch = stats;
            }
        }
    }
    public void DestroyPlayer(int id) {
        GameObject[] players = GameObject.FindGameObjectsWithTag("Player");
        foreach (GameObject p in players) {
            if (p.GetComponent<PhotonView>().ViewID == id) {
                if (p.GetComponent<PhotonView>().IsMine) {
                photonView.RPC("SetParentMeshRPC", RpcTarget.All, p.GetComponent<DragDrop>().parent.name, true);
                }
                playerNames[3].text = "Id" + id + " " + p.GetComponent<PhotonView>().ViewID;
                Destroy(p.gameObject);
            }
        }
    }
    public List<GameObject> GetListOfDestroyablePlayers() {
       // destroyableOpponent.Clear();
        GameObject[] players = GameObject.FindGameObjectsWithTag("Player");
        foreach (GameObject p in players) {
            if (!p.GetComponent<PhotonView>().IsMine&&!p.GetComponent<DragDrop>().isDestroyable) {
                destroyableOpponent.Add(p);
            }
        }
        return destroyableOpponent;
    }

    #endregion PlayerFunctions
    public void DisableMoves() {
        foreach (GameObject m in moves) {
            m.GetComponent<CapsuleCollider>().enabled = false;
        }
    }
    public void EmptyMoves() {
        emptyMovesList.Clear();
        foreach (GameObject m in moves) {
           if( m.GetComponent<Slot>().status == 0) {
                emptyMovesList.Add(m.gameObject);//add all free moves to list
            }
        }
        playerNames[3].text = emptyMovesList.Count+"";
    }
    public void SetParentMesh(string movename, bool isEnable,int n) {
        foreach (GameObject m in moves) {
            if (m.name == movename) {
                m.GetComponent<MeshRenderer>().enabled = isEnable;
                m.GetComponent<Slot>().status = n;
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

    [PunRPC]
    public void SetParentMeshRPC(string movename, bool isEnable) {
        foreach (GameObject m in moves) {
            if (m.name == movename) {
                m.GetComponent<MeshRenderer>().enabled = isEnable;
                m.GetComponent<Slot>().status = 1;
            }
        }
    }
    //Display names of both Players
    [PunRPC]
    private void showName() {
        int i = 0;
        foreach(Player p in PhotonNetwork.PlayerList) {
            playerNames[i].text = p.NickName;
            i++;
        }
        }
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
    [PunRPC]
    private void MovePlayerAutomatically(bool isPlayer1) {
                GameObject m = emptyMovesList[0];
        playerNames[3].text = emptyMovesList[0].name + "";

        if (isPlayer1) {
                    Player1.GetComponent<DragDrop>().parent = m;
                    Player1.GetComponent<DragDrop>().isSet = true;
                    Player1.GetComponent<DragDrop>().OnMouseUp();
                } else {
                    Player2.GetComponent<DragDrop>().parent = m;
                    Player2.GetComponent<DragDrop>().isSet = true;
                    Player2.GetComponent<DragDrop>().OnMouseUp();
                }
    }
    #endregion RPC
    #region Coroutines
    IEnumerator DisplayTime(bool isTurn,GameObject player,bool isPlayer1) {
        int i = 0;
        while (i < 10) {
            yield return new WaitForSeconds(1);
            i++;
            photonView.RPC("ShowTime", RpcTarget.All,isTurn, i);
        }
        if (player.GetComponent<DragDrop>().isTouch&&isPlayer1) {
            photonView.RPC("MovePlayerAutomatically", RpcTarget.All, true);
        } else if(player.GetComponent<DragDrop>().isTouch && !isPlayer1) {
            photonView.RPC("MovePlayerAutomatically", RpcTarget.All, false);
        }
        if(isPlayer1) Player1.GetComponent<DragDrop>().SizeDownDestroyableOpponents();
        else Player2.GetComponent<DragDrop>().SizeDownDestroyableOpponents();
        isSpawn = false;
        photonView.RPC("SetMyTurn",RpcTarget.All, isTurn);
    }
    #endregion Coroutines
}
