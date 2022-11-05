using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEngine.EventSystems;
using Photon.Pun;
public class DragDrop : MonoBehaviour {
    [SerializeField]
    public GameObject parent,wall,wall1;
    [SerializeField]
    public bool isSet = false;
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
        if (photonView.IsMine&&Manager.Instance.isSpawn) {
            if (isDragging) {  DragObject(); }
        }
    }
    private void OnMouseDown() {
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
    }

    private void OnMouseUp() {
        isDragging = false;
       // Manager.Instance.EnableWallColliders();
        foreach (GameObject obj in list) {
            SizeDown(obj);
        }
        if (!isSet) {
            transform.position = startPos;
        } else {
            transform.localPosition = parent.transform.position;// new Vector3(0, 1f, 0);
            isSet = false; }
        if (wall != null){ checkSibling(wall); }
        if (wall1 != null) { checkSibling(wall1); }
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
        Manager.Instance.DisableMoves();
        for(int i = 0;i<parent.GetComponent<Slot>().moves.Length;i++)
            if (parent.GetComponent<Slot>().moves[i].GetComponent<Slot>().status == 0) {
                list.Add(parent.GetComponent<Slot>().moves[i].gameObject);
                parent.GetComponent<Slot>().moves[i].GetComponent<CapsuleCollider>().enabled = true;
               SizeUP(parent.GetComponent<Slot>().moves[i].gameObject);
            }
        
    }
    //[PunRPC]
    //private void UpdateParentStatus(GameObject obj) {
    //    obj.gameObject.GetComponent<DragDrop>().parent.GetComponent<Slot>().status = 0;
    //}
    //IEnumerator Resize(GameObject obj) {
    //    yield return new WaitForSeconds(1);
    //    int a = 0;
    //    while (a < 10) {
    //        a++;
    //        obj.transform.localScale = new Vector3(22f, 5, 22f);
    //        yield return new WaitForSeconds(0.3f);
    //        obj.transform.localScale = new Vector3(20, 5, 20);
    //    }
    //}
    private void SizeUP(GameObject obj) {
        obj.transform.localScale = new Vector3(22f, 5, 22f);

    }
    private void SizeDown(GameObject obj) {
        obj.transform.localScale = new Vector3(20, 5, 20);

    }
    private void OnTriggerEnter(Collider other) {
        if (other.gameObject.CompareTag("Wall")) {
         //   Debug.Log("wall  " + other.gameObject.name);
            if (wall != null) { CheckPreviousParent(wall); }
            wall = other.gameObject;
            //   other.gameObject.GetComponent<BoxCollider>().enabled = true;

        }
        if (other.gameObject.CompareTag("Wall1")) {
            if (wall1 != null) { CheckPreviousParent(wall1); }
          //  Debug.Log("1wall   " + other.gameObject.name);
            wall1 = other.gameObject;
            //   other.gameObject.GetComponent<BoxCollider>().enabled = true;

        }
    }
    private void checkSibling(GameObject wall) {
        if (!wall.GetComponent<Wall>().p1 && !wall.GetComponent<Wall>().p2 && !wall.GetComponent<Wall>().p3) {
            Debug.Log("I am activating p1" );
            wall.GetComponent<Wall>().g1 = gameObject;
            wall.GetComponent<Wall>().p1 = true;
        } else if (wall.GetComponent<Wall>().p1 && !wall.GetComponent<Wall>().p2 && !wall.GetComponent<Wall>().p3) {
            if (wall.GetComponent<Wall>().g1.GetComponent<PhotonView>().ViewID != gameObject.GetComponent<PhotonView>().ViewID) {
                Debug.Log("I am activating p2");
                wall.GetComponent<Wall>().g2 = gameObject;
                wall.GetComponent<Wall>().p2 = true;
            }
        } else if (wall.GetComponent<Wall>().p1 && wall.GetComponent<Wall>().p2 && !wall.GetComponent<Wall>().p3) {
            if (wall.GetComponent<Wall>().g1.GetComponent<PhotonView>().ViewID != gameObject.GetComponent<PhotonView>().ViewID && wall.GetComponent<Wall>().g2.GetComponent<PhotonView>().ViewID != gameObject.GetComponent<PhotonView>().ViewID) {
                wall.GetComponent<Wall>().g3 = gameObject;
                wall.GetComponent<Wall>().p3 = true;
            }
        }
        //if (!wall1.GetComponent<Wall>().p1 && !wall1.GetComponent<Wall>().p2 && !wall1.GetComponent<Wall>().p3) {
        //    wall1.GetComponent<Wall>().g1 = gameObject;
        //    wall1.GetComponent<Wall>().p1 = true;
        //} else if (wall1.GetComponent<Wall>().p1 && !wall1.GetComponent<Wall>().p2 && !wall1.GetComponent<Wall>().p3) {
        //    wall1.GetComponent<Wall>().g2 = gameObject;
        //    wall1.GetComponent<Wall>().p2 = true;
        //} else if (wall1.GetComponent<Wall>().p1 && wall1.GetComponent<Wall>().p2 && !wall1.GetComponent<Wall>().p3) {
        //    wall1.GetComponent<Wall>().g3 = gameObject;
        //    wall1.GetComponent<Wall>().p3 = true;
        //}
        }
        private void CheckPreviousParent(GameObject wall) {
        //if parent have p1 but no p2 and p3.
        if (wall.GetComponent<Wall>().p1 && !wall.GetComponent<Wall>().p2 && !wall.GetComponent<Wall>().p3) {
            wall.GetComponent<Wall>().g1 = null;
            wall.GetComponent<Wall>().p1 = false;
            //if parent have p1 ,p2but no  p3.
        } else if (wall.GetComponent<Wall>().p1 && wall.GetComponent<Wall>().p2 && !wall.GetComponent<Wall>().p3) {
            wall.GetComponent<Wall>().g2 = null;
            wall.GetComponent<Wall>().p2 = false;
            //if parent have p1, p2,and p3
        } else if (wall.GetComponent<Wall>().p1 && wall.GetComponent<Wall>().p2 && wall.GetComponent<Wall>().p3) {
            wall.GetComponent<Wall>().g3 = null;
            wall.GetComponent<Wall>().p3 = false;
        }
    }     
    }
