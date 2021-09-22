using UnityEngine;
using UnityEngine.Networking;

namespace Klak.Ndi {

// Platform specific helper functions for Android

static class AndroidHelper
{
    #if UNITY_ANDROID && !UNITY_EDITOR

    static AndroidJavaObject _nsdManager;

    public static void SetupNetwork()
    {
        // Enable multicasting: This also adds the network permissions to the
        // application manifest.
        NetworkTransport.SetMulticastLock(true);

        // Create a network service discovery manager object.
        var player = new AndroidJavaClass("com.unity3d.player.UnityPlayer");
        var activity = player.GetStatic<AndroidJavaObject>("currentActivity");
        _nsdManager = activity.Call<AndroidJavaObject>
          ("getSystemService", "servicediscovery");
    }

    #else

    public static void SetupNetwork() {}

    #endif
}

} // namespace Klak.Ndi
