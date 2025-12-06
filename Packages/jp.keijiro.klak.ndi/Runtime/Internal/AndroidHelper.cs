using UnityEngine;

namespace Klak.Ndi {

// Platform specific helper functions for Android

static class AndroidHelper
{
    #if UNITY_ANDROID && !UNITY_EDITOR

    static AndroidJavaObject _mcastLock;

    public static void SetupNetwork()
    {
        // Multicast lock acquisition
        var player = new AndroidJavaClass("com.unity3d.player.UnityPlayer");
        var activity = player.GetStatic<AndroidJavaObject>("currentActivity");
        var wifi = activity.Call<AndroidJavaObject>("getSystemService", "wifi");
        _mcastLock = wifi.Call<AndroidJavaObject>("createMulticastLock", "lock");
        _mcastLock.Call("acquire");
    }

    #else

    public static void SetupNetwork() {}

    #endif
}

} // namespace Klak.Ndi
