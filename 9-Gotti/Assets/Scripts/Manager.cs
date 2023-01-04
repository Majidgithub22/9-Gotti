using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;
using Photon.Realtime;
using Photon.Voice.Unity;

public class Manager : Singleton<Manager> {
    public Recorder recorder;
    #region UI
    [Header("UI_Elements")]
    public Text[] playerNames;
    public Text[] TimeText;
    public Text WinnerName;
    public Text LoserName;
    public GameObject MuteButton;
    public GameObject UnMuteButton;
    public GameObject MuteButtonAll;
    public GameObject UnMuteButtonAll;
    public GameObject resultScreen;
    #endregion UI
    public Camera Camera;
    public float p1X, p1Y, p1Z, p2X, p2Y, p2Z;
    public bool play, isSpawn = false;
    //private int distanceBtwPlayers = -25;
    private int placePlayerCount;
    private GameObject[] walls;
    private GameObject[] moves;
    public List<GameObject> emptyMovesList = new List<GameObject>();
    public List<GameObject> ShowDummyGotti1 = new List<GameObject>();
    public List<GameObject> ShowDummyGotti2 = new List<GameObject>();
    private PhotonView photonView;
    public bool isLineFormed;
    public bool isP1Turn = true;
    public bool takeTurn;
    public bool only1Move;
    public bool isStop;
    public GameObject waltext;
    private int playerCount;
    private GameObject Player1, Player2;
    public List<GameObject> destroyableOpponent = new List<GameObject>();
    private void Start() {
        photonView = GetComponent<PhotonView>();
        walls = GameObject.FindGameObjectsWithTag("Wall");
        moves = GameObject.FindGameObjectsWithTag("Pillar");
        photonView.RPC("showName", RpcTarget.AllBuffered);
        DisplayUserTime();
    }
    public void Mute() {
        recorder.TransmitEnabled= false;
        MuteButton.SetActive(false);
        UnMuteButton.SetActive(true);
    }
    public void UnMute() {
        recorder.TransmitEnabled = true;
        MuteButton.SetActive(true);
        UnMuteButton.SetActive(false);
    }
    #region PlayerFunctions
    //DisplayTime for each player
    public void DisplayUserTime() {
        if (!play) { 
          //  Debug.Log("i am in first mlve");
            if (PhotonNetwork.IsMasterClient && isP1Turn) {
                isSpawn = true;
                Player1 = PhotonNetwork.Instantiate("Player1", new Vector3(p1X, p1Y, p1Z), Quaternion.identity);
                StartCoroutine(DisplayTime(false, Player1));
               // photonView.RPC("DestroyGottiP1", RpcTarget.All);
            } else if (!isP1Turn && !PhotonNetwork.IsMasterClient) {
                isSpawn = true;
                Player2 = PhotonNetwork.Instantiate("Player2", new Vector3(p2X, p2Y, p2Z), Quaternion.identity);
                StartCoroutine(DisplayTime(true, Player2));
            }
        } 
        //else {
        //    //Call player for the first time aftyer play
        //    photonView.RPC("SetTurns", RpcTarget.Others, true);
        //}
    }
    //Check Player  count
    public void PlacePlayers() {
        placePlayerCount++;
        if (placePlayerCount >= 18) {// If player is in room.
            EnableDragDrop();
            play = true;
        }
     ////   Debug.Log("PlaerCount" + placePlayerCount);
    }
    //Enable drag drp for each player.
    public void EnableDragDrop() {
        GameObject[] players = GameObject.FindGameObjectsWithTag("Player");
        foreach (GameObject p in players) {
            p.GetComponent<DragDrop>().enabled = true;
        }
    }
    //Enable gooti move only after all gottis are placed
    public void EnableGottiMoveAfterPlay() {
        GameObject[] players = GameObject.FindGameObjectsWithTag("Player");
        foreach (GameObject p in players) {
            if (p.GetComponent<PhotonView>().IsMine) {
                p.GetComponent<DragDrop>().enabled = true;
                p.GetComponent<DragDrop>().isTouch = true;//can 
            }
        }
        only1Move = true;
        isSpawn = true;
    }
    public void DisableGottiMoveAfterPlay() {
        GameObject[] players = GameObject.FindGameObjectsWithTag("Player");
        foreach (GameObject p in players) {
            if (p.GetComponent<PhotonView>().IsMine) {
                p.GetComponent<DragDrop>().enabled = false;
                p.GetComponent<DragDrop>().isTouch = false;//can 
            }
        }
        only1Move = false;
    }
    public void EnablePlayerTouch(bool t) {
        GameObject[] players = GameObject.FindGameObjectsWithTag("Player");
        foreach (GameObject p in players) {
            if (p.GetComponent<PhotonView>().IsMine)
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
        bool found=false;
        GameObject[] players = GameObject.FindGameObjectsWithTag("Player");
        foreach (GameObject p in players) {
            if (p.GetComponent<PhotonView>().ViewID == id) {
                p.GetComponent<DragDrop>().parent.GetComponent<Slot>().status = 0;
                p.GetComponent<DragDrop>().parent.GetComponent<MeshRenderer>().enabled = true;
             //   playerNames[3].text = "" + p.GetComponent<DragDrop>().wall;
                if (p.GetComponent<PhotonView>().IsMine) {
                    p.GetComponent<DragDrop>().CheckPreviousParentDest(p.GetComponent<DragDrop>().wall, p.GetComponent<PhotonView>().ViewID);
                    p.GetComponent<DragDrop>().CheckPreviousParentDest(p.GetComponent<DragDrop>().wall1, p.GetComponent<PhotonView>().ViewID);
                }
                //playerNames[3].text = "";
               // Debug.Log("wall p1" + p.GetComponent<DragDrop>().wall.GetComponent<Wall>().g1);
                Destroy(p.gameObject);
              //  Debug.Log("wall p1" + p.GetComponent<DragDrop>().wall.GetComponent<Wall>().g1);
                //playerNames[3].text = "done";
                found = true;
               // Debug.Log("wall p1" + p.GetComponent<DragDrop>().wall.GetComponent<Wall>().g1);
            }
            if (found) {
              break;
            }
        }
    }
    public void GetListOfDestroyablePlayers() {
        destroyableOpponent.Clear();
        GameObject[] players = GameObject.FindGameObjectsWithTag("Player");
        foreach (GameObject p in players) {
            if (!p.GetComponent<PhotonView>().IsMine && !p.GetComponent<DragDrop>().isDestroyable) {
                destroyableOpponent.Add(p);
            }
        }
    }

    #endregion PlayerFunctions
    int io=0, jo=0;
    [PunRPC]
    private void DestroyGottiP1() {
        if (ShowDummyGotti1.Count > io) {
            Debug.Log(ShowDummyGotti1.Count);
            Destroy(ShowDummyGotti1[io].gameObject);
            io++;
        }
        
    }
    [PunRPC]
    private void DestroyGottiP2() {
        if (ShowDummyGotti2.Count >jo) {
            Destroy(ShowDummyGotti2[jo].gameObject);
            // ShowDummyGotti1.RemoveAt(jo);
            jo++;
        }
    }
    public void DisableMoves() {
        foreach (GameObject m in moves) {
            m.GetComponent<CapsuleCollider>().enabled = false;
        }
    }
    public void EmptyMoves() {
        emptyMovesList.Clear();
        foreach (GameObject m in moves) {
            if (m.GetComponent<Slot>().status == 0) {
                emptyMovesList.Add(m.gameObject);//add all free moves to list
            }
        }
    }
   
    public void SetParentMesh(string movename, int id, bool isEnable, int n, bool isSetParent) {
        GameObject prnt = null;
        foreach (GameObject m in moves) {
            if (m.name == movename) {
                prnt = m;
                m.GetComponent<MeshRenderer>().enabled = isEnable;
                m.GetComponent<Slot>().status = n;
            }
        }
        if (isSetParent) {
            GameObject[] players = GameObject.FindGameObjectsWithTag("Player");
            foreach (GameObject p in players) {
                if (p.GetComponent<PhotonView>().ViewID == id) {
                    p.GetComponent<DragDrop>().parent = prnt;
                    p.GetComponent<DragDrop>().wall = prnt.GetComponent<Slot>().wall;
                    p.GetComponent<DragDrop>().wall1 = prnt.GetComponent<Slot>().wall1;
                }
            }
        }
    }
    public void SetParentFree(string parentName) {
        foreach (GameObject m in moves) {
            if (m.name == parentName) {
                m.GetComponent<MeshRenderer>().enabled = true;
                m.GetComponent<Slot>().status = 0;
            //    Manager.Instance.playerNames[3].text = "parent free";
            }
        }
    }
    //check wall gotti statusss
    public void WallGottiCheck() {
        foreach (GameObject w in walls) {
            w.GetComponent<Wall>().SetGottiDestroyableStatus();
        }
    }
    private void CheckPlayerCount() {
        playerCount = 0;
        GameObject[] players = GameObject.FindGameObjectsWithTag("Player");
        foreach (GameObject p in players) {
            if (p.GetComponent<PhotonView>().IsMine) {
                playerCount++;
            }
        }
        if (playerCount <= 1) {
            photonView.RPC("ShowResult", RpcTarget.All, PhotonNetwork.LocalPlayer.ActorNumber);
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
        foreach (Player p in PhotonNetwork.PlayerList) {
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
    private void ShowTime(bool isTurn, int sec) {
        if (isTurn)
            TimeText[1].text = sec.ToString();
        else
            TimeText[0].text = sec.ToString();
    }
    [PunRPC]
    private void ShowTimeAfterPlay(bool isTurn, int sec) {
        if (!isStop) {
            if (!isTurn)
                TimeText[1].text = sec.ToString();
            else
                TimeText[0].text = sec.ToString();
        }
    }
    [PunRPC]
    private void MovePlayerAutomatically(int id) {
        GameObject m = emptyMovesList[0];
        GameObject[] players = GameObject.FindGameObjectsWithTag("Player");
        foreach (GameObject p in players) {
            Debug.Log("move auto" + p.GetComponent<PhotonView>().ViewID);
            if (p.GetComponent<PhotonView>().ViewID == id) {
                p.GetComponent<DragDrop>().temparent = m;
                p.GetComponent<DragDrop>().isSet = true;
                p.GetComponent<DragDrop>().OnMouseUp();
            }
        }
    }
    [PunRPC]
    private void ShowResult(int id) {
        isStop = true;
        Photon.Realtime.Player[] players = PhotonNetwork.PlayerList;
        foreach (Player p in players) {
            if (p.ActorNumber == id) { 
                LoserName.text = p.NickName + " Lose";//lose the game
            } else {
                WinnerName.text = p.NickName + " Won";
            }
        }
        resultScreen.SetActive(true);
        }

    private void MovePlayerAfterPlay() {
        GameObject[] players = GameObject.FindGameObjectsWithTag("Player");
        List<GameObject> movePlayer=new List<GameObject>();
        movePlayer.Clear();
        foreach (GameObject p in players) {
            if (p.GetComponent<PhotonView>().IsMine) {
                movePlayer.Add(p);
            }
        }
     //   Debug.Log("move player count"+movePlayer.Count);
        
        for(int i = 0; i < movePlayer.Count; i++) {
        //    Debug.Log("MOVE SLOTS COUNTS" + movePlayer[i].GetComponent<Slot>().moves.Length);
            for(int j = 0; j < movePlayer[i].GetComponent<DragDrop>().parent.GetComponent<Slot>().moves.Length; j++) {
                if (movePlayer[i].GetComponent<DragDrop>().parent.GetComponent<Slot>().moves[j].GetComponent<Slot>().status == 0) {
                    movePlayer[i].GetComponent<DragDrop>().temparent = movePlayer[i].GetComponent<DragDrop>().parent.GetComponent<Slot>().moves[j];
                    movePlayer[i].GetComponent<DragDrop>().isSet = true;
                    movePlayer[i].GetComponent<DragDrop>().OnMouseUp();
                    movePlayer[i].GetComponent<DragDrop>().SizeDownDestroyableOpponents();
                    return;
                }
            }
        }
    }
    #endregion RPC
    #region Coroutines
    IEnumerator DisplayTime(bool isTurn, GameObject player) {
        int i = 0;
        while (i < 15) {
            yield return new WaitForSeconds(1);
            i++;
            photonView.RPC("ShowTime", RpcTarget.All, isTurn, i);
        }//it means player is not moved yet.
        if (player.GetComponent<DragDrop>().isTouch) {
            photonView.RPC("MovePlayerAutomatically", RpcTarget.All, player.GetComponent<PhotonView>().ViewID);
        }
        if (!isTurn) Player1.GetComponent<DragDrop>().SizeDownDestroyableOpponents();
        else Player2.GetComponent<DragDrop>().SizeDownDestroyableOpponents();
                
        if (isTurn) { photonView.RPC("DestroyGottiP2", RpcTarget.All); } else { photonView.RPC("DestroyGottiP1", RpcTarget.All); }
        isSpawn = false;
        if (!play) {
            photonView.RPC("SetMyTurn", RpcTarget.All, isTurn);
        } else {  //Call player for the first time after play
            photonView.RPC("SetTurns", RpcTarget.Others, true);
        }
    }

    [PunRPC]
    private void SetTurns(bool pl) {
        StartCoroutine(MoveGottiPlay(pl));
    }
    IEnumerator MoveGottiPlay(bool P1) {
        if (!isStop) {
            //Debug.Log("MoveGotti");
            EnableGottiMoveAfterPlay();
            int i = 0;
            while (i < 15) {
                yield return new WaitForSeconds(1);
                i++;
                photonView.RPC("ShowTimeAfterPlay", RpcTarget.All, P1, i);
            }
            if (only1Move) {
                //  photonView.RPC("" +
                MovePlayerAfterPlay();
            }//
            Debug.Log(only1Move);
            //DestroyAutoMatically or size down
            GameObject[] players = GameObject.FindGameObjectsWithTag("Player");
            foreach (GameObject p in players) {
                if (p.GetComponent<PhotonView>().IsMine) {
                    p.GetComponent<DragDrop>().SizeDownDestroyableOpponents();
                    break;
                }
            }
            isSpawn = false;
            P1 = !P1;
            CheckPlayerCount();
            photonView.RPC("SetTurns", RpcTarget.Others, P1);
        }
    }
    #endregion Coroutines
}

