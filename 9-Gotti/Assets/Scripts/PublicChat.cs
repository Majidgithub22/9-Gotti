using ExitGames.Client.Photon;
using Photon.Chat;
using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PublicChat : MonoBehaviour,IChatClientListener {

    ChatClient chatClient;
    bool isConnected;
    string UserName;
    PhotonView photonView;
    public void UsernameOnValueChange(string valueIn) {
        //=valueIn;
    }
    public void ChatConectOnCLick() {
        UserName = PhotonNetwork.NickName;
        isConnected = true;
        chatClient = new ChatClient(this);
        chatClient.Connect(PhotonNetwork.PhotonServerSettings.AppSettings.AppIdChat, PhotonNetwork.AppVersion, new AuthenticationValues(UserName));
        Debug.Log("Conecting");
    }
   // public GameObject ChatPanel;
    string privateReceiever = "";
    string currentChat;
    public InputField chatField;
    public Text chatDisplay;
    public GameObject CHatPanels;
    void Start() {
        ChatConectOnCLick();
        photonView= GetComponent<PhotonView>();
    }
    void Update() {
        if (this.chatClient != null) {
            this.chatClient.Service();
        }
        //if (!string.IsNullOrEmpty(chatField.text) && (Input.GetKey(KeyCode.Return) || Input.GetKey(KeyCode.KeypadEnter))) {
        //    chatClient.Service();
        //    SubmitPublicChat();
        //}
    }
    public void SubmitPublicChat() {
            chatClient.PublishMessage("Region", currentChat);
            currentChat = "";
            chatField.text = "";
            chatField.ActivateInputField();
         photonView.RPC("NotifyUser", RpcTarget.Others, PhotonNetwork.NickName);
    }
    [PunRPC]
    private void NotifyUser(string name) {
        Manager.Instance.NotificationOfMsg.text = name + " sent a message";
        Manager.Instance.NotificationOfMsg.gameObject.transform.parent.gameObject.SetActive(true);
        StartCoroutine(DisableNotifyMSG());
    }
    IEnumerator DisableNotifyMSG() {
        yield return new WaitForSeconds(3);
        Manager.Instance.NotificationOfMsg.gameObject.transform.parent.gameObject.SetActive(false);
    }
    public void TypeValueChanged(string valueIn) {
        currentChat=valueIn;
    }
    public void OpenCHatPanel() {
        CHatPanels.SetActive(true);
    }
    public void CloseCHatPanel() {
        CHatPanels.SetActive(false);
    }
    #region callBack
    public void DebugReturn(DebugLevel level, string message) {
    }

    public void OnDisconnected() {
    }

    public void OnConnected() {
        chatClient.Subscribe(new string[] { "Region" });
       // Debug.Log("connecting to chanel");

    }

    public void OnChatStateChange(ChatState state) {
    }

    public void OnGetMessages(string channelName, string[] senders, object[] messages) {
        string msg = "";
        for (int i = 0; i < senders.Length; i++) {
            //msg = string.Format( "{0}: {1}", "<color=purple>"+senders[i]+"</color>", messages[i]);
            chatDisplay.text += "\n" + "<color=purple>" + senders[i] + "</color>"+": "+ messages[i];
           // Debug.Log(msg);
        
        
        
        
        }
    }
    public void OnPrivateMessage(string sender, object message, string channelName) {
    }

    public void OnSubscribed(string[] channels, bool[] results) {
    }

    public void OnUnsubscribed(string[] channels) {
    }

    public void OnStatusUpdate(string user, int status, bool gotMessage, object message) {
    }

    public void OnUserSubscribed(string channel, string user) {
    }

    public void OnUserUnsubscribed(string channel, string user) {
    }
    #endregion callback
}
