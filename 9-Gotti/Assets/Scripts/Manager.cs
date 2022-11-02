using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class Manager : Singleton<Manager> {
    public Camera Camera;
    public float p1X,p1Y,p1Z,p2X,p2Y,p2Z;
    public bool play;
    private int distanceBtwPlayers=-25;
    private int placePlayerCount;
    private GameObject[] walls;
    public void PlacePlayers() {
        placePlayerCount++;
        if (placePlayerCount >= 5) {
            EnableDragDrop();
            play = true;
        }
    }
    private void Start() {
        walls = GameObject.FindGameObjectsWithTag("Wall");
        StartPlacing();
    }
    private void StartPlacing() {
        if (PhotonNetwork.IsMasterClient) {
            StartCoroutine(InstantiatePlayer("Player1", p1X, p1Y, p1Z));
        } else {
            StartCoroutine(InstantiatePlayer("Player2", p2X, p2Y, p2Z));
        }
    }
    public void EnableDragDrop() {
        GameObject[] players = GameObject.FindGameObjectsWithTag("Player");
        foreach (GameObject p in players) {
            p.GetComponent<DragDrop>().enabled = true;
        }
    }
    public void DisableMoves() {
        GameObject[] moves = GameObject.FindGameObjectsWithTag("Pillar");
        foreach (GameObject m in moves) {
            m.GetComponent<CapsuleCollider>().enabled = false;
        }
    }
    public void DisableWallColliders() {
        foreach (GameObject w in walls) {
            w.GetComponent<BoxCollider>().enabled = false;
        }
    }
    public void EnableWallColliders() {
        foreach (GameObject w in walls) {
            w.GetComponent<BoxCollider>().enabled = false;
        }
    }

    IEnumerator InstantiatePlayer(string player,float x,float y,float z) {
        int totalPlayer =0;
        yield return new WaitForSeconds(1);
        while (totalPlayer < 9) {
            PhotonNetwork.Instantiate(player,new Vector3(x,y,z), Quaternion.identity);
            z += distanceBtwPlayers;
            yield return new WaitForSeconds(0.6f);
            totalPlayer++;
        }
    }
}
