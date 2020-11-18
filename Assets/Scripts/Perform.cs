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

        public void Join(string channel, uint userId)
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

            mRtcEngine.JoinChannel(channel, null, userId);

            // Optional: if a data stream is required, here is a good place to create it
            int streamId = mRtcEngine.CreateDataStream(true, true);
            Debug.Log("CreateDataStream. streamId = " + streamId);

            mRtcEngine.SendStreamMessage(streamId, $"Message from {userId}: I have joined {channel}");
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
        }

        // implement engine callbacks
        private void OnJoinChannelSuccess(string channelName, uint uid, int elapsed)
        {
            Debug.Log("JoinChannelSuccessHandler: uid = " + uid);
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
            Debug.Log("Remote user joined: uid = " + uid + " elapsed = " + elapsed);

            // Optional: if a data stream is required, here is a good place to create it
            int streamID = mRtcEngine.CreateDataStream(true, true);
            Debug.Log("initializeEngine done, data stream id = " + streamID);

            mRtcEngine.SendStreamMessage(streamID, "'Hello!' from the Performer app.");

        }
        private void OnStreamMessage(uint userId, int streamId, string data, int length)
        {
            Debug.Log($"Message from {userId}: {data}");
        }

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