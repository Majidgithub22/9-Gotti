using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Realtime;
using agora_gaming_rtc;
using System.Linq;
using UnityEngine.UI;

public class SpatialAudio : MonoBehaviour
{
    [SerializeField] float radius;

    PhotonView PV;

    static Dictionary<Player, SpatialAudio> spatialAudioFromPlayers = new Dictionary<Player, SpatialAudio>();

    IAudioEffectManager agoraAudioEffects;

    void Awake()
    {
        PV = GetComponent<PhotonView>();
        agoraAudioEffects = VoiceChatManager.Instance.GetRtcEngine().GetAudioEffectManager();

        spatialAudioFromPlayers[PV.Owner] = this;
    }

    void OnDestroy()
    {
        foreach (var item in spatialAudioFromPlayers.Where(x => x.Value == this).ToList())
        {
            spatialAudioFromPlayers.Remove(item.Key);
        }
    }

    void Update()
    {
       // Debug.Log(PV.IsMine);
        if (!PV.IsMine)
        {

            return;
        }

        foreach (Player player in PhotonNetwork.CurrentRoom.Players.Values)
        {
         //   Debug.Log("Hereagain local palayer" + player.IsLocal);
            if (player.IsLocal)
            {
                continue;
            }

            if (player.CustomProperties.TryGetValue("AgoraID", out object agoraID))
                {
                   // Debug.Log("stck");

                    if (spatialAudioFromPlayers.ContainsKey(player))
                    {
                      //  Debug.Log("contains key");

                        SpatialAudio other = spatialAudioFromPlayers[player];

                        float gain = GetGain(other.transform.position);
                        float pan = GetPan(other.transform.position);
                        //if (gain <= 0.0f)
                        //{
                        // //   Debug.Log(gain + "lower gain of payer");
                        //    DisableNames(player.NickName);
                        //}
                        //else
                        //{
                        ////    Debug.Log(gain + "greater gain of payer");
                        //    EnableNames(player.NickName);
                        //}
                        agoraAudioEffects.SetRemoteVoicePosition(uint.Parse((string)agoraID), pan, gain);
                    }
                    else
                    {
                        agoraAudioEffects.SetRemoteVoicePosition(uint.Parse((string)agoraID), 0, 0);
                    }
                }
        }
    }

    float GetGain(Vector3 otherPosition)
    {
        float distance = Vector3.Distance(transform.position, otherPosition);
        float gain = Mathf.Max(1 - (distance / radius), 0) * 100f;
        return gain;
    }

    float GetPan(Vector3 otherPosition)
    {
        Vector3 direction = otherPosition - transform.position;
        direction.Normalize();
        float dotProduct = Vector3.Dot(transform.right, direction);
        return dotProduct;
    }
    public void Mute() {
        VoiceChatManager.Instance.Mute();
        Manager.Instance.MuteButton.SetActive(false);
        Manager.Instance.UnMuteButton.SetActive(true);
    }
    public void UnMute() {
        VoiceChatManager.Instance.UnMute();
        Manager.Instance.MuteButton.SetActive(true);
        Manager.Instance.UnMuteButton.SetActive(false);
    }
    public void MuteAll() {
        VoiceChatManager.Instance.MuteAll();
        Manager.Instance.MuteButtonAll.SetActive(false);
        Manager.Instance.UnMuteButtonAll.SetActive(true);
    }
    public void UnMuteAll() {
        VoiceChatManager.Instance.UnmuteAll();
        Manager.Instance.MuteButtonAll.SetActive(true);
        Manager.Instance.UnMuteButtonAll.SetActive(false);
    }

}
