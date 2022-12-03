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
    private List<GameObject> list = new List<GameObject>();
    private void Start() {
        myCam = Manager.Instance.Camera;
        photonView = GetComponent<PhotonView>();
    }
    private void Update() {
        if (photonView.IsMine && Manager.Instance.isSpawn) {
            if (isDragging) { DragObject(); }
        }
    }
    private void OnMouseDown() {
        if (photonView.IsMine) {
            Manager.Instance.tureText.text = "In IF";
            startPos = gameObject.transform.localPosition;
            Vector3 mousePos = Input.mousePosition;
            if (Manager.Instance.play) { asd(); }
            if (!myCam.orthographic) {
                mousePos.z = 10;
            }
            mousePos = myCam.ScreenToWorldPoint(mousePos);
            startXPos = mousePos.x - transform.position.x;
            startZPos = mousePos.z - transform.position.z;
            isDragging = true;
        } else {
            Manager.Instance.tureText.text = "In Else";
            if (Manager.Instance.isLineFormed) {
                Manager.Instance.tureText.text = "Inside Else";
                if (!gameObject.GetComponent<DragDrop>().isDestroyable) {
                    Destroy(gameObject);
                    Manager.Instance.isLineFormed = false;
                    Manager.Instance.tureText.text = gameObject.GetComponent<DragDrop>().isDestroyable.ToString();
                }
            }
        }
    }
    private void OnMouseUp() {
        if (photonView.IsMine) {
            isDragging = false;
            // Manager.Instance.EnableWallColliders();
            foreach (GameObject obj in list) {//sizing the nearby pillars down.Only they are in list
                SizeDown(obj);
            }
            if (!isSet) {
                transform.position = startPos;
            } else {
                if (temparent != null) {//checking if new parent is there
                    if (parent != null) {//if parent is there mean not assigning for 1st time
                        photonView.RPC("UpdateStatus", RpcTarget.All, parent.name, 0);//set status of previous parent to 0, Making it RPC
                    }
                    parent = temparent;//new parent assigned
                }
                gameObject.GetComponent<DragDrop>().enabled = false;

                transform.localPosition = parent.transform.position;//
                photonView.RPC("UpdateStatus", RpcTarget.All, parent.name, 1);//set status of current parent.GetComponent<Slot>().status = 1;
                if (wall != null) { CheckPreviousParent(wall); }
                if (wall1 != null) { CheckPreviousParent(wall1); }
                wall = parent.GetComponent<Slot>().wall;
                wall1 = parent.GetComponent<Slot>().wall1;
                checkSibling(parent.GetComponent<Slot>().wall);
                checkSibling(parent.GetComponent<Slot>().wall1);
                isSet = false;
                Manager.Instance.WallGottiCheck();
                photonView.RPC("UpdatePlaceCount", RpcTarget.All);//again enable drag drop
            }
        } else {
            if (Manager.Instance.isLineFormed) {
                Manager.Instance.tureText.text = "Inside Else";
                if (!gameObject.GetComponent<DragDrop>().isDestroyable) {
                    Destroy(gameObject);
                    Manager.Instance.isLineFormed = false;
                    Manager.Instance.tureText.text = gameObject.GetComponent<DragDrop>().isDestroyable.ToString();
                }
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
    private void asd() {
        Manager.Instance.DisableMoves();// Disale every pillar collider
        for (int i = 0; i < parent.GetComponent<Slot>().moves.Length; i++)
            if (parent.GetComponent<Slot>().moves[i].GetComponent<Slot>().status == 0) {//checking if parent pillar neihbour moves are free add them to list
                list.Add(parent.GetComponent<Slot>().moves[i].gameObject);
                parent.GetComponent<Slot>().moves[i].GetComponent<CapsuleCollider>().enabled = true;//enable only those who are in neighbour and free
                SizeUP(parent.GetComponent<Slot>().moves[i].gameObject);//now activate them.
            }

    }

    private void SizeUP(GameObject obj) {
        obj.transform.localScale = new Vector3(25f, 5, 25f);

    }
    private void SizeDown(GameObject obj) {
        obj.transform.localScale = new Vector3(20, 5, 20);

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
                Manager.Instance.tureText.text = Manager.Instance.isLineFormed.ToString();

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
    private void UpdateStatus(string name,int status) {
        //set previous move status to 0 so ther moves can move there.
        Manager.Instance.SetPillarStatus(name,status);
    }
    [PunRPC]
    private void UpdatePlaceCount() {
        Manager.Instance.PlacePlayers();//again enable drag drop
    }
    [PunRPC]
    private void UpdatePlayerStatus(int id, bool isdestry) {
        Manager.Instance.PlayerDestroyStatus(id, isdestry);
    }
    #endregion RPC_Calls
    public void EveryWallGottiStatus(int id, bool isdestry) {
        photonView.RPC("UpdatePlayerStatus", RpcTarget.All, id, isdestry);
    }
}
