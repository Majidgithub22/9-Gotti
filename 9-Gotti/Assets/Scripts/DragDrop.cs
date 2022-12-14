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
           Manager.Instance. playerNames[3].text = "in else";
            if (Manager.Instance.isLineFormed) {
                Manager.Instance.playerNames[3].text = "line formed";
                if (!gameObject.GetComponent<DragDrop>().isDestroyable&& !gameObject.GetComponent<DragDrop>().isTouch) {//touch is for don't destroy gotti that is not moves yet to place.
                    Manager.Instance.playerNames[3].text = "destroying player";
                    photonView.RPC("DestroyPlayerRPC", RpcTarget.AllBuffered, gameObject.GetComponent<PhotonView>().ViewID);
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
                        photonView.RPC("SetParentMesh",RpcTarget.All, parent.name, true,0);//set status of previous parent to 0, Making it RPC
                    }
                    parent = temparent;//new parent assigned
                }
                transform.position = parent.transform.position;//
                photonView.RPC("SetIsTouch", RpcTarget.All, gameObject.GetComponent<PhotonView>().ViewID, false);//isTouch = false;//can not touch until all gotti placed.
                photonView.RPC("SetParentMesh",RpcTarget.All, parent.name, false,1);//set status of previous parent to 0, Making it RPC
                Manager.Instance.playerNames[0].text = transform.position + "";
                if (wall != null) { CheckPreviousParent(wall); }
                if (wall1 != null) { CheckPreviousParent(wall1); }
                wall = parent.GetComponent<Slot>().wall;
                wall1 = parent.GetComponent<Slot>().wall1;
                checkSibling(parent.GetComponent<Slot>().wall);
                checkSibling(parent.GetComponent<Slot>().wall1);
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
        List<GameObject> destPlayers = Manager.Instance.GetListOfDestroyablePlayers();
        foreach (GameObject p in destPlayers) {
            SizeUP(p);
        }
    }
    public void SizeDownDestroyableOpponents() {
        List<GameObject> destPlayers = Manager.Instance.GetListOfDestroyablePlayers();
        foreach (GameObject p in destPlayers) {
            SizeDown(p);
        }
        //destroy player if not destroyed yet.
        if (Manager.Instance.isLineFormed) {
            photonView.RPC("DestroyPlayerRPC", RpcTarget.AllBuffered,destPlayers[0].GetComponent<PhotonView>().ViewID);
        }
        Manager.Instance.isLineFormed = false;
    }
    private void checkSibling(GameObject wall) {
        if (!wall.GetComponent<Wall>().p1 && !wall.GetComponent<Wall>().p2 && !wall.GetComponent<Wall>().p3) {
            wall.GetComponent<Wall>().g1 = gameObject;
            wall.GetComponent<Wall>().p1 = true;
        } else if (wall.GetComponent<Wall>().p1 && !wall.GetComponent<Wall>().p2 && !wall.GetComponent<Wall>().p3) {
         //   if (wall.GetComponent<Wall>().g1.GetComponent<PhotonView>().ViewID != gameObject.GetComponent<PhotonView>().ViewID) {
                Debug.Log("I am activating for p2 " + wall);
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
    private void CheckPreviousParent(GameObject wall) {
        //if parent have p1 but no p2 and p3.
        if (wall.GetComponent<Wall>().p1 && !wall.GetComponent<Wall>().p2 && !wall.GetComponent<Wall>().p3) {
            wall.GetComponent<Wall>().g1 = null;
            wall.GetComponent<Wall>().p1 = false;
            //photonView.RPC("UpdatePlayerStatus", RpcTarget.All, wall.GetComponent<Wall>().g1.GetComponent<PhotonView>().ViewID, false);
            Debug.Log("I am dactivating for p1 " + wall);
            //if parent have p1 ,p2but no  p3.
        } else if (wall.GetComponent<Wall>().p1 && wall.GetComponent<Wall>().p2 && !wall.GetComponent<Wall>().p3) {
            wall.GetComponent<Wall>().g2 = null;
            wall.GetComponent<Wall>().p2 = false;
            //photonView.RPC("UpdatePlayerStatus", RpcTarget.All, wall.GetComponent<Wall>().g1.GetComponent<PhotonView>().ViewID, false);
            //photonView.RPC("UpdatePlayerStatus", RpcTarget.All, wall.GetComponent<Wall>().g2.GetComponent<PhotonView>().ViewID, false);
            Debug.Log("I am dactivating for p2 " + wall);
            //if parent have p1, p2,and p3
        } else if (wall.GetComponent<Wall>().p1 && wall.GetComponent<Wall>().p2 && wall.GetComponent<Wall>().p3) {
           
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
    private void UpdatePlaceCount() {
        Manager.Instance.PlacePlayers();//again enable drag drop
    }
    [PunRPC]
    private void SetIsTouch(int id,bool stat) {
        Manager.Instance.PlayerIsTouch(id,stat);//again enable drag drop
    }
    [PunRPC]
    private void UpdatePlayerStatus(int id, bool isdestry) {
        Manager.Instance.PlayerDestroyStatus(id, isdestry);
    }
    [PunRPC]
    private void SetParentMesh(string name, bool isEnable,int n) {
        Manager.Instance.SetParentMesh(name, isEnable,n);
    }
    [PunRPC]
    private void DestroyPlayerRPC(int id) {
        Manager.Instance.DestroyPlayer(id);
    }
    #endregion RPC_Calls
    public void EveryWallGottiStatus(int id, bool isdestry) {
        photonView.RPC("UpdatePlayerStatus", RpcTarget.All, id, isdestry);
    }
}
