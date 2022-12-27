using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using agora_gaming_rtc;
using System;
using Photon.Pun;
using Hashtable = ExitGames.Client.Photon.Hashtable;
using UnityEngine.UI;
using Photon.Realtime;

public class VoiceChatManager : MonoBehaviourPunCallbacks
{
    public string chanel;
    public static VoiceChatManager Instance;
    IRtcEngine rtcEngine;
    string appID = "9c4f0644fb1c4d03a480a106e8ed3fca";

    void Awake()
    {
        if (Instance)
        {
            Destroy(gameObject);
        }
        else
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
    }

    void Start()
    {
        if (string.IsNullOrEmpty(appID))
        {
            Debug.LogError("App ID not set in VoiceChatManager script");
            return;
        }

        rtcEngine = IRtcEngine.GetEngine(appID);
        rtcEngine.EnableAudio();
        rtcEngine.OnJoinChannelSuccess += OnJoinChannelSuccess;

        rtcEngine.OnLeaveChannel += OnLeaveChannel;
        rtcEngine.OnError += OnError;
        Debug.Log("hERE Voice Chat");
        rtcEngine.EnableSoundPositionIndication(true);
    }

    void OnError(int error, string msg)
    {
        Debug.LogError("Error with Agora: " + msg);
    }

    void OnLeaveChannel(RtcStats stats)
    {
        Debug.Log("Left channel with duration " + stats.duration);
    }

    void OnJoinChannelSuccess(string channelName, uint uid, int elapsed)
    {
        Hashtable hash = new Hashtable();
        hash.Add("AgoraID", uid.ToString());
        //PhotonNetwork.SetPlayerCustomProperties(hash);
        PhotonNetwork.LocalPlayer.SetCustomProperties(hash);
       // channelName = "044";
        chanel = channelName;
        Debug.Log(PhotonNetwork.LocalPlayer.CustomProperties["AgoraID"]);
        foreach (Player p in PhotonNetwork.CurrentRoom.Players.Values)
        {
            Debug.Log((string)p.CustomProperties["AgoraID"] + " Name " + p.NickName);
        }
        //-----------------------------//
           Debug.Log("Joined channel " + uid + "id" + uid);
    }

    public IRtcEngine GetRtcEngine()
    {
        return rtcEngine;
    }

    public override void OnJoinedRoom()
    {
        rtcEngine.JoinChannel(PhotonNetwork.CurrentRoom.Name);
        //rtcEngine.OnUserJoined += (uint uid, int elapsed) =>
        //{
        //    string userJoinedMessage = string.Format("onUserJoined with uid {0}", uid);
        //    Debug.Log(userJoinedMessage);
        //    remoteStreams.Add(uid); // add remote stream id to list of users
        //    remoteUid = (int)uid;
        //};
    }
    public override void OnLeftRoom()
    {
        rtcEngine.LeaveChannel();
    }

    void OnDestroy()
    {
        IRtcEngine.Destroy();
    }


    public void Mute()
    {
        rtcEngine.MuteLocalAudioStream(true);
        //  rtcEngine.AdjustUserPlaybackSignalVolume((uint)remoteUid, 0);
    }
    public void UnMute()
    {
        rtcEngine.MuteLocalAudioStream(false);
        //  rtcEngine.AdjustUserPlaybackSignalVolume((uint)remoteUid,100);
    }
    public void MuteAll()
    {
        rtcEngine.MuteAllRemoteAudioStreams(true);
    }
    public void UnmuteAll()
    {
        rtcEngine.MuteAllRemoteAudioStreams(false);
    }
    //public void MuteRemoteClient(string name)
    //{
    //    foreach (Player p in PhotonNetwork.CurrentRoom.Players.Values)
    //    {
    //        if (name == p.NickName)
    //        {
    //            if (p.CustomProperties.TryGetValue("AgoraID", out object agoraID))
    //            {
    //              //  UIManager.Instance.chneltest.GetComponent<Text>().text = "totalPlayer " + PhotonNetwork.CurrentRoom.PlayerCount + " " + uint.Parse((string)agoraID);
    //                rtcEngine.MuteRemoteAudioStream(uint.Parse((string)agoraID), true);

    //            }
    //        }
    //    }

    //}
    //public void UnMuteRemoteClient(string name)
    //{
    //    foreach (Player p in PhotonNetwork.CurrentRoom.Players.Values)
    //    {
    //        if (name == p.NickName)
    //        {
    //            if (p.CustomProperties.TryGetValue("AgoraID", out object agoraID))
    //            {
    //            //    UIManager.Instance.chneltest.GetComponent<Text>().text = "totalPlayer " + PhotonNetwork.CurrentRoom.PlayerCount + " " + uint.Parse((string)agoraID);
                  
    //                rtcEngine.MuteRemoteAudioStream(uint.Parse((string)agoraID), false);
    //            }
    //        }

    //    }
    //}
}


