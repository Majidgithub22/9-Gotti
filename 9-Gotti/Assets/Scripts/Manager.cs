using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Manager : Singleton<Manager> {
    private int placePlayerCount;
    public bool play;
    public void PlacePlayers() {
        placePlayerCount++;
        if (placePlayerCount == 5) {
            placePlayerCount=0;
            //Debug.Log(placePlayerCount);
            EnableDragDrop();
            play = true;
        }
    }
    private void StartPlacing() {

    }
    public void EnableDragDrop() {
        GameObject[] players = GameObject.FindGameObjectsWithTag("Player");
        foreach (GameObject p in players) {
            p.GetComponent<DragDrop>().enabled = true;
        }
    }
}
