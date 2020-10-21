using UnityEngine;
using agora_gaming_rtc;
using UnityEngine.UI;


namespace Assets.Scripts
{
    public class Perform
    {
        private IRtcEngine mRtcEngine;

        public void LoadEngine(string appId)
        {
            // start sdk
            Debug.Log("initializeEngine");

            if (mRtcEngine != null)
            {
                Debug.Log("Engine exists. Please unload it first!");
                return;
            }

            // init engine
            mRtcEngine = IRtcEngine.GetEngine(appId);

            // enable log
            mRtcEngine.SetLogFilter(LOG_FILTER.DEBUG | LOG_FILTER.INFO | LOG_FILTER.WARNING | LOG_FILTER.ERROR |
                                    LOG_FILTER.CRITICAL);
        }

        public void Join(string channel)
        {
            Debug.Log("calling join (channel = " + channel + ")");

            if (mRtcEngine == null)
                return;

            // set callbacks (optional)
            mRtcEngine.OnJoinChannelSuccess = OnJoinChannelSuccess;
            //mRtcEngine.OnUserJoined = OnUserJoined;
            mRtcEngine.OnUserOffline = OnUserOffline;

            // Added by me
            //mRtcEngine.OnStreamMessage = OnStreamMessage;

            mRtcEngine.EnableVideo();

            // allow camera output callback
            mRtcEngine.EnableVideoObserver();

            mRtcEngine.JoinChannel(channel, null, 0);

            // Optional: if a data stream is required, here is a good place to create it
            int streamID = mRtcEngine.CreateDataStream(true, true);
            Debug.Log("initializeEngine done, data stream id = " + streamID);
        }

        public string GetSdkVersion()
        {
            string ver = IRtcEngine.GetSdkVersion();
            if (ver == "2.9.1.45")
            {
                ver = "2.9.2"; // A conversion for the current internal version#
            }
            else
            {
                if (ver == "2.9.1.46")
                {
                    ver = "2.9.2.2"; // A conversion for the current internal version#
                }
            }

            return ver;
        }

        public void Leave()
        {
            Debug.Log("calling leave");

            if (mRtcEngine == null)
                return;

            // leave channel
            mRtcEngine.LeaveChannel();
            // deregister video frame observers
            mRtcEngine.DisableVideoObserver();
        }

        public void UnloadEngine()
        {
            Debug.Log("calling unloadEngine");

            if (mRtcEngine != null)
            {
                IRtcEngine.Destroy(); // Place this call in ApplicationQuit
                mRtcEngine = null;
            }
        }


        public void EnableVideo(bool pauseVideo)
        {
            if (mRtcEngine != null)
            {
                if (!pauseVideo)
                {
                    mRtcEngine.EnableVideo();
                }
                else
                {
                    mRtcEngine.DisableVideo();
                }
            }
        }

        // accessing GameObject in Scene1
        // set video transform delegate for statically created GameObject
        public void OnScenePerformLoaded()
        {
            GameObject quadStandAloneRectangle = GameObject.Find("QuadStandAloneRectangle");
            GameObject quadStandAloneSquare = GameObject.Find("QuadStandAloneSquare");
            if (!ReferenceEquals(quadStandAloneRectangle, null))
            {
                Debug.Log("Attach to QuadStandAloneRectangle");
                quadStandAloneRectangle.AddComponent<VideoSurface>();
            }
            if (!ReferenceEquals(quadStandAloneSquare, null))
            {
                Debug.Log("Attach to QuadStandAloneSquare");
                quadStandAloneSquare.AddComponent<VideoSurface>();
            }
            //else
            //{
            //    Debug.Log("Attach to quad in screen");
            //    quadStandAloneRectangle.AddComponent<VideoSurface>();
            //}

            //GameObject screenObject = GameObject.Find("Screen");
            //var canvasDisplay = screenObject.transform.Find("CanvasDisplay");
            //canvasDisplay.gameObject.SetActive(true);
            //var go = canvasDisplay.transform.GetChild(0).gameObject;
            //VideoSurface videoSurface = go.GetComponent<VideoSurface>();
            //go.AddComponent<VideoSurface>();



            //GameObject quadInScreen = screen.transform.Find("QuadInScreen").gameObject;
            //if (ReferenceEquals(quadInScreen, null))
            //{
            //    Debug.Log("BBBB: failed to find QuadInScreen");
            //    return;
            //}
            //else
            //{
            //    Debug.Log("Attach to quad in screen");
            //    quadInScreen.AddComponent<VideoSurface>();
            //}
        }

        // implement engine callbacks
        private void OnJoinChannelSuccess(string channelName, uint uid, int elapsed)
        {
            //ShowMessage("Got to OnJoinChannelSuccess");
            //Debug.Log("JoinChannelSuccessHandler: uid = " + uid);
            //GameObject textVersionGameObject = GameObject.Find("VersionText");
            //textVersionGameObject.GetComponent<Text>().text = "SDK Version : " + getSdkVersion();
        }

        // implement engine callbacks
        private void ShowMessage(string message)
        {
            Debug.Log("ShowMessage: message = " + message);
            GameObject textMessageGameObject= GameObject.Find("MessageText");
            textMessageGameObject.GetComponent<Text>().text += " " + message;
        }

        // When a remote user joined, this delegate will be called. Typically
        // create a GameObject to render video on it
        private void OnUserJoined(uint uid, int elapsed)
        {
            ShowMessage("Got to OnUserJoined");
            Debug.Log("onUserJoined: uid = " + uid + " elapsed = " + elapsed);
            // this is called in main thread

            // Not sure why we even need the below code and method makeImageSurface.
            // Perhaps if we are dynamically creating screens for clients connecting IN ?


            // find a game object to render video stream from 'uid'
            //GameObject go = GameObject.Find(uid.ToString());
            //if (!ReferenceEquals(go, null))
            //{
            //    Debug.Log("go != null");
            //    return; // reuse
            //}

            // create a GameObject and assign to this new user
            //VideoSurface videoSurface = makeImageSurface(uid.ToString());

            //if (!ReferenceEquals(videoSurface, null))
            //{
            //    // configure videoSurface
            //    videoSurface.SetForUser(uid);
            //    videoSurface.SetEnable(true);
            //    videoSurface.SetVideoSurfaceType(AgoraVideoSurfaceType.RawImage);
            //    videoSurface.SetGameFps(30);
            //}

            // Optional: if a data stream is required, here is a good place to create it
            int streamID = mRtcEngine.CreateDataStream(true, true);
            Debug.Log("initializeEngine done, data stream id = " + streamID);

            mRtcEngine.SendStreamMessage(streamID, "'Hello!' from the Performer app.");

        }
        private void OnStreamMessage(uint userId, int streamId, string data, int length)
        {
            Debug.Log($"Message from {userId}: {data}");
        }

        //public VideoSurface makePlaneSurface(string goName)
        //{
        //    GameObject go = GameObject.CreatePrimitive(PrimitiveType.Plane);

        //    if (go == null)
        //    {
        //        return null;
        //    }

        //    go.name = goName;
        //    // set up transform
        //    go.transform.Rotate(-90.0f, 0.0f, 0.0f);
        //    float yPos = Random.Range(3.0f, 5.0f);
        //    float xPos = Random.Range(-2.0f, 2.0f);
        //    go.transform.position = new Vector3(xPos, yPos, 0f);
        //    go.transform.localScale = new Vector3(0.25f, 0.5f, .5f);

        //    // configure videoSurface
        //    VideoSurface videoSurface = go.AddComponent<VideoSurface>();
        //    return videoSurface;
        //}

        
        //public VideoSurface makeImageSurface(string goName)
        //{
        //    GameObject go = new GameObject();

        //    if (go == null)
        //    {
        //        return null;
        //    }

        //    go.name = goName;

        //    // to be renderered onto
        //    go.AddComponent<RawImage>();

        //    // make the object draggable
        //    go.AddComponent<UIElementDragger>();

        //    GameObject canvas = GameObject.Find("Canvas");
        //    if (canvas != null)
        //    {
        //        go.transform.parent = canvas.transform;
        //    }

        //    // set up transform
        //    //go.transform.Rotate(0f, 0.0f, 180.0f);
        //    //float xPos = Random.Range(Offset - Screen.width / 2f, Screen.width / 2f - Offset);
        //    //float yPos = Random.Range(Offset, Screen.height / 2f - Offset);
        //    //go.transform.localPosition = new Vector3(xPos, yPos, 0f);
        //    //go.transform.localScale = new Vector3(3f, 4f, 1f);

        //    // configure videoSurface
        //    VideoSurface videoSurface = go.AddComponent<VideoSurface>();
        //    return videoSurface;
        //}

        // When remote user is offline, this delegate will be called. Typically
        // delete the GameObject for this user
        private void OnUserOffline(uint uid, USER_OFFLINE_REASON reason)
        {
            // remove video stream
            Debug.Log("onUserOffline: uid = " + uid + " reason = " + reason);
            // this is called in main thread
            GameObject go = GameObject.Find(uid.ToString());
            if (!ReferenceEquals(go, null))
            {
                Object.Destroy(go);
            }
        }
    }
}