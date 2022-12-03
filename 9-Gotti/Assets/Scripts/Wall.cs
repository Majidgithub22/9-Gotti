using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Wall : MonoBehaviour
{
    public GameObject[] moves;
    public bool p1, p2, p3;
    public GameObject g1, g2, g3;
    public void SetGottiDestroyableStatus() {
        if (g3!=null&&g1.GetComponent<PhotonView>().IsMine && g2.GetComponent<PhotonView>().IsMine && g3.GetComponent<PhotonView>().IsMine) {
            g1.GetComponent<DragDrop>().EveryWallGottiStatus(g1.GetComponent<PhotonView>().ViewID, true);
            g2.GetComponent<DragDrop>().EveryWallGottiStatus(g2.GetComponent<PhotonView>().ViewID, true);
            g3.GetComponent<DragDrop>().EveryWallGottiStatus(g3.GetComponent<PhotonView>().ViewID, true);
        }
    }

}
