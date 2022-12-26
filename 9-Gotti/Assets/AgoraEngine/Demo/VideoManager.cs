using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using agora_gaming_rtc;
using agora_utilities;
public class VideoManager : MonoBehaviour
{


    [SerializeField]
    private string AppID = "your_appid"; //8b930e38405b4f8eb037b72b52f610db


    // Start is called before the first frame update
    void Start()
    {
      //  CheckAppId();
    }

    static TestHelloUnityVideo app = null;
    public GameObject videoSurfaceObj;

    public void OnjoinedCalld()
    {
        onJoinButtonClicked(true);
    }
    //join using button
    GameObject VObj;
    public void onJoinButtonClicked(bool enableVideo, bool muted = false)
    {
        if (VObj != null)
            VObj.SetActive(true);
        else
        {
            // create app if nonexistent
            if (ReferenceEquals(app, null))
            {
                app = new TestHelloUnityVideo(); // create app
                app.loadEngine(AppID); // load engine
            }
            string n1 = Photon.Pun.PhotonNetwork.CurrentRoom.Name;
            // ChannelName = inputField.text;
            //string Rname = "Room" + Random.Range(1, 5000);
            // join channel and jump to next scene
            app.join(n1, enableVideo, muted);
            //app.onSceneHelloVideoLoaded(); // call this after scene is loaded
            videoSurfaceObj.AddComponent<VideoSurface>();
            VObj = videoSurfaceObj;
          //  videoSurfaceObj.GetComponent<VideoSurface>()._enableFlipVertical = true;
         /*   VObj = Instantiate(videoSurfaceObj);
            VObj.transform.SetParent(videoSurfaceObj.transform);
            VObj.transform.localScale = Vector3.one;
            VObj.transform.localPosition = Vector3.zero;
            VObj.transform.eulerAngles = new Vector3(0,0,180);
            VObj.AddComponent<VideoSurface>();*/
        }
       
  }
    // Update is called once per frame
    void Update()
    {
        
    }


    //Leave from server
    
    public void onLeaveButtonClicked()
    {
       /* if (!ReferenceEquals(app, null))
        {
            app.leave(); // leave channel
            app.unloadEngine(); // delete engine
        }*/
        //Destroy(VObj);

        VObj.SetActive(false);
    }
}
