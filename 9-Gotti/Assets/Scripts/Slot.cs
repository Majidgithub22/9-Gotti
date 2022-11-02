using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class Slot : MonoBehaviour
{
    public int status;
    public GameObject[] moves;
    private PhotonView photonView;
    private List<GameObject> slots = new List<GameObject>();
    private void OnTriggerEnter(Collider other) {
        if (other.gameObject.CompareTag("Player")) {
            status = 1;
            if (Manager.Instance.play) {
                // Manager.Instance.EnableDragDrop();
                // other.gameObject.GetComponent<DragDrop>().parent.GetComponent<Slot>().status = 0;
                //Update Status of client
                photonView.RPC("UpdateStatus", RpcTarget.AllBuffered,other);
            }
            // other.gameObject.transform.SetParent(gameObject.transform,false);
            //  other.gameObject.transform.position =new Vector3(0,4f,0);
            other.gameObject.transform.position = transform.position;
            other.gameObject.GetComponent<DragDrop>().parent = gameObject ;
            
            //other.gameObject.transform.localScale =new Vector3(1,1f,1);
            gameObject.GetComponent<CapsuleCollider>().enabled = false;
            other.gameObject.GetComponent<DragDrop>().enabled = false;
            other.gameObject.GetComponent<DragDrop>().isSet = true;
            Manager.Instance.PlacePlayers();
        }
    }
    [PunRPC]
   private void UpdateStatus(GameObject obj) {
      obj.gameObject.GetComponent<DragDrop>().parent.GetComponent<Slot>().status = 0;
    }
    //private void SetStatus(int status) {
    //    this.status= status;
    //}
    //private void FindMoveableSlots(int wall, int child) {
    //    for (int i = 0; i < wall; i++) {
    //        for (int j = 0; i < child; j++) {
    //            if (gameObject.name != moves[i].GetComponent<Wall>().moves[j].gameObject.name&& moves[i].GetComponent<Slot>().status==0) {
    //                slots.Add(moves[i]);
    //            }
    //        }
    //}
    //}
    //private void NavigateMoves() {
    //    foreach (GameObject slot in slots) {
    //        StartCoroutine(Resize(slot));
    //    }
    //}

    //IEnumerator Resize(GameObject obj) {
    //        yield return new WaitForSeconds(1);
    //        int a = 0;
    //        while (a < 10) {
    //            a++;
    //            obj.transform.localScale = new Vector3(1.3f, 1, 1.3f);
    //            yield return new WaitForSeconds(0.3f);
    //            obj.transform.localScale = new Vector3(1, 1, 1);
    //        }
    //    }
}
