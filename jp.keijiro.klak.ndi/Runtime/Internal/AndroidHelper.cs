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
        // Create a network service discovery manager object.
        var player = new AndroidJavaClass("com.unity3d.player.UnityPlayer");
        var activity = player.GetStatic<AndroidJavaObject>("currentActivity");
        _nsdManager = activity.Call<AndroidJavaObject>
          ("getSystemService", "servicediscovery");

        // Enable multicasting:
        var wifiManager = activity.Call<AndroidJavaObject>("getSystemService", "wifi");
            // Check if the multicast lock is active.
            var lockActive = wifiManager.Call<AndroidJavaObject>("createMulticastLock", "klak-ndi").Call<bool>("isHeld");

            // If not, acquire the lock.
            if (!lockActive)
            {
                wifiManager.Call<AndroidJavaObject>("createMulticastLock", "klak-ndi").Call("acquire");
            }
    }

    #else

    public static void SetupNetwork() {}

    #endif
}

} // namespace Klak.Ndi
