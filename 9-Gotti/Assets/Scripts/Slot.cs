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
          gameObject.transform.position = transform.position;//set position of gotti
            other.gameObject.GetComponent<DragDrop>().temparent = gameObject ;//set parent
            other.gameObject.GetComponent<DragDrop>().isSet = true;//set bool to check whether moved to new place.
        }
    }
}
