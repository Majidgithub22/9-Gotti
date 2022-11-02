using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Wall : MonoBehaviour
{
    public GameObject[] moves;
    public int lineP1Count,lineP2Count;
    private bool touch;
    private void OnCollisionEnter(Collision collision) {
        if (collision.gameObject.CompareTag("player1")) {
            if (lineP1Count > 3) { lineP1Count = 0; }
            Debug.Log("I touch p1"+lineP1Count);
            lineP1Count++;
            Manager.Instance.DisableWallColliders();
            return;
        }
        if (collision.gameObject.CompareTag("player2")) {
            if (lineP1Count > 3) { lineP1Count = 0; }
            Debug.Log("I touch p2"+lineP2Count);
            lineP2Count++;
            Manager.Instance.DisableWallColliders();
            return;
        }
    }
}
