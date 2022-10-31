using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEngine.EventSystems;

public class DragDrop : MonoBehaviour {


    public Camera myCam;
    private float startXPos;
    private float startZPos;
    private Vector3 startPos;
    private bool isDragging = false;
    private bool isSet = false;
    private List<GameObject> list = new List<GameObject>();
    private void Update() {
        if (isDragging) {
            DragObject();
        }
    }

    private void OnMouseDown() {
        startPos = gameObject.transform.position;
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
        foreach (GameObject obj in list) {
            SizeDown(obj);
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
        for(int i = 0;i< transform.parent.GetComponent<Slot>().moves.Length;i++)
            if (transform.parent.GetComponent<Slot>().moves[i].GetComponent<Slot>().status == 0) {
                list.Add(transform.parent.GetComponent<Slot>().moves[i].gameObject);
                transform.parent.GetComponent<Slot>().moves[i].GetComponent<CapsuleCollider>().enabled = true;
               SizeUP(transform.parent.GetComponent<Slot>().moves[i].gameObject);
            }
        
    }
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
}
