using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEngine.EventSystems;
using Photon.Pun;
public class DragDrop : MonoBehaviour {
    [SerializeField]
    public GameObject parent, wall, wall1, prevwall1, prevwall, temparent;
    [SerializeField]
    public bool isSet = false;
    public bool isDestroyable;
    private Camera myCam;
    private float startXPos;
    private float startZPos;
    private Vector3 startPos;
    private bool isDragging = false;
    private PhotonView photonView;
    public List<GameObject> list = new List<GameObject>();
    public bool isTouch=true;//can we touch player if game is started else no
    private void Start() {
        myCam = Manager.Instance.Camera;
        photonView = GetComponent<PhotonView>();
        Manager.Instance.EmptyMoves();//Count empty moves.
    }
    private void Update() {
        if (photonView.IsMine && Manager.Instance.isSpawn) {
            if (isDragging) { DragObject(); }
        }
    }
    public void OnMouseDown() {
        if (photonView.IsMine&&isTouch) {
            startPos = gameObject.transform.localPosition;
            Vector3 mousePos = Input.mousePosition;
            if (Manager.Instance.play) { ShowNeigbourMoves(); }
            if (!myCam.orthographic) {
                mousePos.z = 10;
            }
            mousePos = myCam.ScreenToWorldPoint(mousePos);
            startXPos = mousePos.x - transform.position.x;
            startZPos = mousePos.z - transform.position.z;
            isDragging = true;
        } else {
            if (Manager.Instance.isLineFormed) {
                if (!gameObject.GetComponent<DragDrop>().isDestroyable&& !gameObject.GetComponent<DragDrop>().isTouch) {//touch is for don't destroy gotti that is not m
                    photonView.RPC("DestroyPlayerRPC", RpcTarget.All, gameObject.GetComponent<PhotonView>().ViewID);
                    Manager.Instance.isLineFormed = false;
                }
            }
        }
    }
    public void OnMouseUp() {
        if (photonView.IsMine&&isTouch) {
            isDragging = false;
            Manager.Instance.playerNames[3].text = "name "+gameObject.name;
            foreach (GameObject obj in list) {//sizing the nearby pillars down.Only they are in list
                SizeDown(obj);
            }
            if (!isSet) {
                transform.position = startPos;
            } else {
                if (temparent != null) {//checking if new parent is there
                    if (parent != null) {//if parent is there mean not assigning for 1st time
                        photonView.RPC("SetParentMesh",RpcTarget.All, parent.name,gameObject.GetComponent<PhotonView>().ViewID, true,0,false);//set status of previous parent to 0, Making it RPC
                    }
                    parent = temparent;//new parent assigned
                }
                //MoveGotttiOnlyOnce
                if (Manager.Instance.play) {
                       Debug.Log("DISABLE");
                    Manager.Instance.DisableGottiMoveAfterPlay();//it should disable other gootiis to move
                }
                if (wall != null && wall != parent.GetComponent<Slot>().wall) { CheckPreviousParent(wall); }
                if (wall1 != null && wall != parent.GetComponent<Slot>().wall) { CheckPreviousParent(wall1); }
                transform.position = parent.transform.position;//
                photonView.RPC("SetIsTouch", RpcTarget.All, gameObject.GetComponent<PhotonView>().ViewID, false);//isTouch = false;//can not touch until all gotti placed.
                photonView.RPC("SetParentMesh",RpcTarget.All, parent.name, gameObject.GetComponent<PhotonView>().ViewID, false,1,true);//set status of  parent to 1, Making it RPC
                Manager.Instance.playerNames[0].text = transform.position + "";
                checkSibling(wall);
                checkSibling(wall1);
                isSet = false;
                Manager.Instance.WallGottiCheck();
                photonView.RPC("UpdatePlaceCount", RpcTarget.All);//again enable drag drop
                Manager.Instance.EmptyMoves();//Count empty moves.
            }
        } 
    }
    public void DragObject() {
        Vector3 mousePos = Input.mousePosition;
        if (!myCam.orthographic) {
            mousePos.z = 10;
        }
        mousePos = myCam.ScreenToWorldPoint(mousePos);
        transform.position = new Vector3(mousePos.x - startXPos, transform.position.y, mousePos.z - startZPos); //*Time.deltaTime*speed;
    }
    private void ShowNeigbourMoves() {
        Manager.Instance.DisableMoves();// Disale every pillar collider
        for (int i = 0; i < parent.GetComponent<Slot>().moves.Length; i++)
            if (parent.GetComponent<Slot>().moves[i].GetComponent<Slot>().status == 0) {//checking if parent pillar neihbour moves are free add them to list
                list.Add(parent.GetComponent<Slot>().moves[i].gameObject);
                parent.GetComponent<Slot>().moves[i].GetComponent<CapsuleCollider>().enabled = true;//enable only those who are in neighbour and free
                SizeUP(parent.GetComponent<Slot>().moves[i].gameObject);//now activate them.
            }
    }
    private void SizeUP(GameObject obj) {
        obj.transform.localScale = new Vector3(28f, 5, 28f);
    }
    private void SizeDown(GameObject obj) {
        obj.transform.localScale = new Vector3(20, 5, 20);
    }
    private void SizeUpDestroyableOpponents() {
        Manager.Instance.GetListOfDestroyablePlayers();
        List<GameObject> destPlayers = new List<GameObject>();
        //get list first
        for(int i=0;i < Manager.Instance.destroyableOpponent.Count; i++) {
            destPlayers.Add(Manager.Instance.destroyableOpponent[i]);
        }
        Manager.Instance.playerNames[1].text = "size up"+destPlayers.Count;
        for (int i = 0; i < Manager.Instance.destroyableOpponent.Count; i++) {
            SizeUP(destPlayers[i].gameObject);
        }
    }
    public void SizeDownDestroyableOpponents() {
        Manager.Instance.GetListOfDestroyablePlayers();
        List<GameObject> destPlayers = new List<GameObject>();
        //get list first
        if (Manager.Instance.destroyableOpponent.Count > 0) {
            for (int i = 0; i < Manager.Instance.destroyableOpponent.Count; i++) {
                destPlayers.Add(Manager.Instance.destroyableOpponent[i]);
            }
           // Manager.Instance.playerNames[3].text = "sizedow" + destPlayers.Count;
            for (int i = 0; i < Manager.Instance.destroyableOpponent.Count; i++) {
                SizeDown(destPlayers[i].gameObject);
            }
            //Manager.Instance.playerNames[3].text = "marasi chly gae" + destPlayers.Count;
            //destroy player if not destroyed yet.
            if (Manager.Instance.isLineFormed) {
                Manager.Instance.playerNames[3].text = "line is formed";
                photonView.RPC("DestroyPlayerRPC", RpcTarget.AllBuffered, destPlayers[0].GetComponent<PhotonView>().ViewID);
                destPlayers.RemoveAt(0);
            }
        
        Manager.Instance.isLineFormed = false;
            destPlayers.Clear();
            Manager.Instance.playerNames[3].text = "listclear";
      //  if(Manager.Instance.waltext.GetComponent<Wall>().g1!=null)
      //Debug.Log("2+"+  Manager.Instance.waltext.GetComponent<Wall>().g1.name);
      //  else { Debug.Log("1null"); }
       }
    }
    private void checkSibling(GameObject wall) {
        if (!wall.GetComponent<Wall>().p1 && !wall.GetComponent<Wall>().p2 && !wall.GetComponent<Wall>().p3) {
            wall.GetComponent<Wall>().g1 = gameObject;
            wall.GetComponent<Wall>().p1 = true;
        } else if (wall.GetComponent<Wall>().p1 && !wall.GetComponent<Wall>().p2 && !wall.GetComponent<Wall>().p3) {
         //   if (wall.GetComponent<Wall>().g1.GetComponent<PhotonView>().ViewID != gameObject.GetComponent<PhotonView>().ViewID) {
              //  Debug.Log("I am activating for p2 " + wall);
                wall.GetComponent<Wall>().g2 = gameObject;
                wall.GetComponent<Wall>().p2 = true;
           // }
        } else if (wall.GetComponent<Wall>().p1 && wall.GetComponent<Wall>().p2 && !wall.GetComponent<Wall>().p3) {
         //   if (wall.GetComponent<Wall>().g1.GetComponent<PhotonView>().ViewID != gameObject.GetComponent<PhotonView>().ViewID && wall.GetComponent<Wall>().g2.GetComponent<PhotonView>().ViewID != gameObject.GetComponent<PhotonView>().ViewID) {
                wall.GetComponent<Wall>().g3 = gameObject;
                wall.GetComponent<Wall>().p3 = true;
                //check if 3 gotttis are mine then move.
                if (wall.GetComponent<Wall>().g1.GetComponent<PhotonView>().IsMine && wall.GetComponent<Wall>().g2.GetComponent<PhotonView>().IsMine && wall.GetComponent<Wall>().g3.GetComponent<PhotonView>().IsMine) {
                    photonView.RPC("UpdatePlayerStatus", RpcTarget.All, wall.GetComponent<Wall>().g1.GetComponent<PhotonView>().ViewID, true);
                    photonView.RPC("UpdatePlayerStatus", RpcTarget.All, wall.GetComponent<Wall>().g2.GetComponent<PhotonView>().ViewID,true);
                    photonView.RPC("UpdatePlayerStatus", RpcTarget.All, wall.GetComponent<Wall>().g3.GetComponent<PhotonView>().ViewID, true);
                    Manager.Instance.isLineFormed = true;
                    SizeUpDestroyableOpponents();
            }
        }
    }
    public void CheckPreviousParentDest(GameObject wall, int id) {
        GameObject obj = null; bool isP1 = false;
        //First get our passed player
        GameObject[] players = GameObject.FindGameObjectsWithTag("Player");
        foreach (GameObject p in players) {
            if (p.GetComponent<PhotonView>().ViewID == id) {
                obj = p;
            }
        }
            //if p1 is not null then check if p is first child of wall
            if (wall.GetComponent<Wall>().p1) {
                if (obj.GetComponent<PhotonView>().ViewID == wall.GetComponent<Wall>().g1.GetComponent<PhotonView>().ViewID) {
                    isP1 = true;
                }
            }//if p2 is not null then check if p is 2nd child of wall
            if (wall.GetComponent<Wall>().p2) {
                if (obj.GetComponent<PhotonView>().ViewID == wall.GetComponent<Wall>().g2.GetComponent<PhotonView>().ViewID) {
                    isP1 = false;
                }
            }
            if (isP1) {
                wall.GetComponent<Wall>().g1 = wall.GetComponent<Wall>().g2;
            }
            //if parent have p1 but no p2 and p3.
            if (wall.GetComponent<Wall>().p1 && !wall.GetComponent<Wall>().p2 && !wall.GetComponent<Wall>().p3) {
                wall.GetComponent<Wall>().g1 = null;
                wall.GetComponent<Wall>().p1 = false;
         //       Debug.Log("I am dactivating for p1 " + wall);
            } else if (wall.GetComponent<Wall>().p1 && wall.GetComponent<Wall>().p2 && !wall.GetComponent<Wall>().p3) {
                wall.GetComponent<Wall>().g2 = null;
                wall.GetComponent<Wall>().p2 = false;
           //     Debug.Log("I am dactivating for p2 " + wall);
            } else if (wall.GetComponent<Wall>().p1 && wall.GetComponent<Wall>().p2 && wall.GetComponent<Wall>().p3) {
                //Now check if wall all 3 are complete// if my line
                if (wall.GetComponent<Wall>().g1.GetComponent<PhotonView>().IsMine && wall.GetComponent<Wall>().g2.GetComponent<PhotonView>().IsMine && wall.GetComponent<Wall>().g3.GetComponent<PhotonView>().IsMine) {
                    photonView.RPC("UpdatePlayerStatus", RpcTarget.All, wall.GetComponent<Wall>().g1.GetComponent<PhotonView>().ViewID, false);
                    photonView.RPC("UpdatePlayerStatus", RpcTarget.All, wall.GetComponent<Wall>().g2.GetComponent<PhotonView>().ViewID, false);
                    photonView.RPC("UpdatePlayerStatus", RpcTarget.All, wall.GetComponent<Wall>().g3.GetComponent<PhotonView>().ViewID, false);
                }
                wall.GetComponent<Wall>().g3 = null;
                wall.GetComponent<Wall>().p3 = false;
            }
    }
    public void CheckPreviousParent(GameObject wall) {

        //if parent have p1 but no p2 and p3.
        if (wall.GetComponent<Wall>().p1 && !wall.GetComponent<Wall>().p2 && !wall.GetComponent<Wall>().p3) {
            wall.GetComponent<Wall>().g1 = null;
            wall.GetComponent<Wall>().p1 = false;
            Debug.Log("I am dactivating for p1 " + wall);
        }
        else if (wall.GetComponent<Wall>().p1 && wall.GetComponent<Wall>().p2 && !wall.GetComponent<Wall>().p3) {
            wall.GetComponent<Wall>().g2 = null;
            wall.GetComponent<Wall>().p2 = false;
            Debug.Log("I am dactivating for p2 " + wall);
        } 
        else if (wall.GetComponent<Wall>().p1 && wall.GetComponent<Wall>().p2 && wall.GetComponent<Wall>().p3) {
            //Now check if wall all 3 are complete// if my line
            if (wall.GetComponent<Wall>().g1.GetComponent<PhotonView>().IsMine&& wall.GetComponent<Wall>().g2.GetComponent<PhotonView>().IsMine&& wall.GetComponent<Wall>().g3.GetComponent<PhotonView>().IsMine) {
                photonView.RPC("UpdatePlayerStatus", RpcTarget.All, wall.GetComponent<Wall>().g1.GetComponent<PhotonView>().ViewID,false); 
                photonView.RPC("UpdatePlayerStatus", RpcTarget.All, wall.GetComponent<Wall>().g2.GetComponent<PhotonView>().ViewID,false);
                photonView.RPC("UpdatePlayerStatus", RpcTarget.All, wall.GetComponent<Wall>().g3.GetComponent<PhotonView>().ViewID,false);
            }
            wall.GetComponent<Wall>().g3 = null;
            wall.GetComponent<Wall>().p3 = false;
        }
    }
   
    #region RPC_Calls
    [PunRPC]
    private void SetWalls(string wallName,string p) {
       // Manager.Instance.SetWalls(wall1,wall2,id);
    }
    [PunRPC]
    private void UpdatePlaceCount() {
        Manager.Instance.PlacePlayers();//again enable drag drop
    }
    
    [PunRPC]
    private void SetIsTouch(int id, bool stat) {
        Manager.Instance.PlayerIsTouch(id, stat);//again enable drag drop
    }
 
    [PunRPC]
    private void UpdatePlayerStatus(int id, bool isdestry) {
        Manager.Instance.PlayerDestroyStatus(id, isdestry);
    }
    [PunRPC]
    private void SetParentMesh(string name,int id, bool isEnable,int n,bool isSet) {
        Manager.Instance.SetParentMesh(name,id, isEnable,n,isSet);
    }
    [PunRPC]
    private void DestroyPlayerRPC(int id) {
    //    Manager.Instance.playerNames[0].text = "callind";
        Manager.Instance.DestroyPlayer(id);
    }
    #endregion RPC_Calls
    public void EveryWallGottiStatus(int id, bool isdestry) {
        photonView.RPC("UpdatePlayerStatus", RpcTarget.All, id, isdestry);
    }
}
