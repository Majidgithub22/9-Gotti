using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class Slot : MonoBehaviour
{
    public int status;
    public GameObject[] moves;
    public GameObject wall, wall1;
    private PhotonView photonView;
    private List<GameObject> slots = new List<GameObject>();
    private void Awake() {
        photonView = GetComponent<PhotonView>();
    }
    private void OnTriggerEnter(Collider other) {
        if (other.gameObject.CompareTag("Player")&&status==0) {
            //in progress
           // status = 1;
            //if (Manager.Instance.play) {
            //    //Update Status of client
            //   photonView.RPC("UpdateStatus", RpcTarget.AllBuffered, other.gameObject.GetComponent<DragDrop>().parent.name);
            //}
            //------------------
          //  other.gameObject.transform.position = transform.position;//set position of gotti
            other.gameObject.GetComponent<DragDrop>().temparent = gameObject ;//set parent
           // gameObject.GetComponent<CapsuleCollider>().enabled = false;//deactivate gameobject collider to disable further interaction

            //other.gameObject.GetComponent<DragDrop>().enabled = false;//disable drag drop scrip of parent
            other.gameObject.GetComponent<DragDrop>().isSet = true;//set bool to check whether moved to new place.
            ////Manager.Instance.PlacePlayers();//again enable drag drop
        }
    }
   //// [PunRPC]
   ////private void UpdateStatus(string name) {
   ////     //set previous move status to 0 so ther moves can move there.
   ////     Manager.Instance.FindMoves(name);
   //// }
   
}
