using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class StartManager : MonoBehaviour
{
    public GameObject ConnectingScreen;
    public GameObject NameEnterScreen;
    public Session session;
    public InputField name;
    private void Awake() {
        if (!string.IsNullOrEmpty(session.name)) {
            ConnectingScreen.SetActive(true);
            NameEnterScreen.SetActive(false);
        }
    }
    public void Go() {
        if(!string.IsNullOrEmpty(name.text)) {
            session.name= name.text;
            ConnectingScreen.SetActive(true);
            NameEnterScreen.SetActive(false);
        }
        }
}
