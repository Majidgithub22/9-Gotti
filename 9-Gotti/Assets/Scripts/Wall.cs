using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Wall : MonoBehaviour
{
    public GameObject[] moves;
    public int lineP1Count,lineP2Count;
    private List<GameObject> P1Ids = new List<GameObject>();
    private List<GameObject> P2Ids = new List<GameObject>();
    private bool touch;
    public bool p1, p2, p3;
    public GameObject g1, g2, g3;
    private void OnTriggerEnter(Collider other) {
        //Debug.Log("COunt"+P1Ids.Count+"by "+gameObject.name);
        //if (other.gameObject.name == "Player1(Clone)") {
        //    if (P1Ids.Count == 0) { Debug.Log("InIffff"); lineP1Count++; P1Ids.Add(other.gameObject); SetPreviousAndNexParent(other.gameObject, 0); } else {
        //        for (int i=0;i<P1Ids.Count;i++) {
        //            if ((P1Ids[i].gameObject.GetComponent<PhotonView>().ViewID != other.gameObject.GetComponent<PhotonView>().ViewID)) {
        //                Debug.Log("ID1 ====" + P1Ids[i].gameObject.GetComponent<PhotonView>().ViewID + "ID2 ====" + other.gameObject.GetComponent<PhotonView>().ViewID);
        //                P1Ids.Add(other.gameObject);
        //                lineP1Count++;
        //                if (lineP1Count == 3) {
        //                    Debug.Log("Destroy game from 1:green.b");
        //                }
        //                SetPreviousAndNexParent(other.gameObject, 0);
        //            }
        //        }
        //    }
        //    if (other.gameObject.name == "Player2(Clone)") {
        //        if (P2Ids.Count == 0) { lineP2Count++; P2Ids.Add(other.gameObject); SetPreviousAndNexParent(other.gameObject, 1); } else {
        //            foreach (GameObject id in P2Ids) {
        //                if (!(id.gameObject.GetComponent<PhotonView>().ViewID == other.gameObject.GetComponent<PhotonView>().ViewID)) {
        //                    P2Ids.Add(other.gameObject);
        //                    lineP2Count++;
        //                    if (lineP2Count == 3) {
        //                        Debug.Log("Destroy game from 2");
        //                    }
        //                    SetPreviousAndNexParent(other.gameObject, 1);
        //                }
        //            }
        //        }
        //    }
        //}
    }
    //private void SetPreviousAndNexParent(GameObject obj, int num) {
    //    if (obj.GetComponent<DragDrop>().wall != null) {
    //        if (num == 0) {
    //            obj.GetComponent<DragDrop>().wall.GetComponent<Wall>().lineP1Count -= 1;
    //            obj.GetComponent<DragDrop>().wall = gameObject;
    //        } else {
    //            obj.GetComponent<DragDrop>().wall.GetComponent<Wall>().lineP2Count -= 1;
    //            obj.GetComponent<DragDrop>().wall = gameObject;
    //        }
    //    }
    //    Manager.Instance.DisableWallColliders();
    //   // gameObject.GetComponent<BoxCollider>().enabled = false;
    //}
    
}
