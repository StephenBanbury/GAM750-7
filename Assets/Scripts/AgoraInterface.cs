using System.Collections;
using System.Collections.Generic;
using agora_gaming_rtc;
using UnityEngine;

public class AgoraInterface : MonoBehaviour
{
    private static string appId = "54f15673a8fd43318b10d4e42f8dd781";
    public IRtcEngine mRtcEngine;
    public uint mRemotePeer;
    
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

    public void JoinChannel(string channelName)
    {
        Debug.Log("Joining channel: " + channelName);

        if (mRtcEngine == null)
        {
            Debug.Log("Engine needs to be initialized before joining a channel");
        }

        // enable video
        mRtcEngine.EnableVideo();

        // allow camera output callback
        mRtcEngine.EnableVideoObserver();

        // join the channel
        mRtcEngine.JoinChannel(channelName, null, 0);
    }

    public void LeaveChannel()
    {
        Debug.Log("Leaving channel");

        if (mRtcEngine == null)
        {
            Debug.Log("Engine needs to be initialized before leaving a channel");
            return;
        }

        // Leave channel
        mRtcEngine.LeaveChannel();

        // Remove video observer();
        mRtcEngine.DisableVideoObserver();
    }

    public void UnloadEngine()
    {
        Debug.Log("Unloading Agora engine");

        if (mRtcEngine != null)
        {
            IRtcEngine.Destroy();
            mRtcEngine = null;
        }
    }
}
