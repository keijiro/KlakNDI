// KlakNDI - NDI plugin for Unity
// https://github.com/keijiro/KlakNDI

// At the moment, the NDI plugin is only available on Windows, macOS and iOS.
#if UNITY_STANDALONE_WIN || UNITY_STANDALONE_OSX || UNITY_IOS
#define NDI_ENABLED
#endif

// iOS only supports sender functionality.
#if !UNITY_EDITOR && UNITY_IOS
#define NDI_SENDER_ONLY
#endif

using System;
using System.Runtime.InteropServices;

namespace Klak.Ndi
{
    // FourCC code definitions used in NDI
    enum FourCC : uint
    {
        UYVY = 0x59565955,
        UYVA = 0x41565955
    }

    static class PluginEntry
    {
        #if !UNITY_EDITOR && UNITY_IOS
        const string _dllName = "__Internal";
        #else
        const string _dllName = "KlakNDI";
        #endif

        #region Common functions

        #if NDI_ENABLED

        internal static bool IsAvailable {
            get {
                var gapi = UnityEngine.SystemInfo.graphicsDeviceType;
                return gapi == UnityEngine.Rendering.GraphicsDeviceType.Direct3D11 ||
                       gapi == UnityEngine.Rendering.GraphicsDeviceType.Metal;
            }
        }

        #else

        internal static bool IsAvailable { get { return false; } }

        #endif

        #if NDI_ENABLED && !NDI_SENDER_ONLY

        [DllImport(_dllName, EntryPoint = "NDI_GetTextureUpdateCallback")]
        internal static extern IntPtr GetTextureUpdateCallback();

        [DllImport(_dllName, EntryPoint = "NDI_RetrieveSourceNames")]
        internal static extern int RetrieveSourceNames(IntPtr[] destination, int maxCount);

        #else

        internal static IntPtr GetTextureUpdateCallback()
        { return IntPtr.Zero; }

        internal static int RetrieveSourceNames(IntPtr[] destination, int maxCount)
        { return 0; }

        #endif

        #endregion

        #region Sender functions

        #if NDI_ENABLED

        [DllImport(_dllName, EntryPoint = "NDI_CreateSender")]
        internal static extern IntPtr CreateSender(string name);

        [DllImport(_dllName, EntryPoint = "NDI_DestroySender")]
        internal static extern void DestroySender(IntPtr sender);

        [DllImport(_dllName, EntryPoint = "NDI_SendFrame")]
        internal static extern void SendFrame(IntPtr sender, IntPtr data, int width, int height, FourCC fourCC);

        [DllImport(_dllName, EntryPoint = "NDI_SyncSender")]
        internal static extern void SyncSender(IntPtr sender);

        #else

        internal static extern IntPtr CreateSender(string name)
        { return IntPtr.Zero; }

        internal static extern void DestroySender(IntPtr sender)
        {}

        internal static extern void SendFrame(IntPtr sender, IntPtr data, int width, int height, FourCC fourCC)
        {}

        internal static extern void SyncSender(IntPtr sender)
        {}

        #endif

        #endregion

        #region Receiver functions

        #if NDI_ENABLED && !NDI_SENDER_ONLY

        [DllImport(_dllName, EntryPoint = "NDI_CreateReceiver")]
        internal static extern IntPtr CreateReceiver(string clause);

        [DllImport(_dllName, EntryPoint = "NDI_DestroyReceiver")]
        internal static extern void DestroyReceiver(IntPtr receiver);

        [DllImport(_dllName, EntryPoint = "NDI_GetReceiverID")]
        internal static extern uint GetReceiverID(IntPtr receiver);

        [DllImport(_dllName, EntryPoint = "NDI_GetFrameWidth")]
        internal static extern int GetFrameWidth(IntPtr receiver);

        [DllImport(_dllName, EntryPoint = "NDI_GetFrameHeight")]
        internal static extern int GetFrameHeight(IntPtr receiver);

        [DllImport(_dllName, EntryPoint = "NDI_GetFrameFourCC")]
        internal static extern FourCC GetFrameFourCC(IntPtr receiver);

        #else

        internal static IntPtr CreateReceiver(string clause)
        { return IntPtr.Zero; }

        internal static void DestroyReceiver(IntPtr receiver)
        {}

        internal static uint GetReceiverID(IntPtr receiver)
        { return 0; }

        internal static int GetFrameWidth(IntPtr receiver)
        { return 0; }

        internal static int GetFrameHeight(IntPtr receiver)
        { return 0; }

        internal static FourCC GetFrameFourCC(IntPtr receiver)
        { return 0; }

        #endif

        #endregion
    }
}
