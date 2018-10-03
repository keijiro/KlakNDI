// KlakNDI - NDI plugin for Unity
// https://github.com/keijiro/KlakNDI

#if !UNITY_EDITOR && UNITY_IOS
#define NDI_SENDER_ONLY
#endif

using System;
using System.Runtime.InteropServices;

namespace Klak.Ndi
{
    public enum FourCC : uint
    {
        UYVY = 0x59565955,
        UYVA = 0x41565955
    }

    public static class PluginEntry
    {
        #if !UNITY_EDITOR && UNITY_IOS
        const string _dllName = "__Internal";
        #else
        const string _dllName = "KlakNDI";
        #endif

        #region Common functions

        #if !NDI_SENDER_ONLY

        [DllImport(_dllName, EntryPoint = "NDI_GetTextureUpdateCallback")]
        public static extern IntPtr GetTextureUpdateCallback();

        [DllImport(_dllName, EntryPoint = "NDI_RetrieveSourceNames")]
        public static extern int RetrieveSourceNames(IntPtr[] destination, int maxCount);

        #else

        public static IntPtr GetTextureUpdateCallback()
        { return IntPtr.Zero; }

        public static int RetrieveSourceNames(IntPtr[] destination, int maxCount)
        { return 0; }

        #endif

        #endregion

        #region Sender functions

        [DllImport(_dllName, EntryPoint = "NDI_CreateSender")]
        public static extern IntPtr CreateSender(string name);

        [DllImport(_dllName, EntryPoint = "NDI_DestroySender")]
        public static extern void DestroySender(IntPtr sender);

        [DllImport(_dllName, EntryPoint = "NDI_SendFrame")]
        public static extern void SendFrame(IntPtr sender, IntPtr data, int width, int height, FourCC fourCC);

        [DllImport(_dllName, EntryPoint = "NDI_SyncSender")]
        public static extern void SyncSender(IntPtr sender);

        #endregion

        #region Receiver functions

        #if !NDI_SENDER_ONLY

        [DllImport(_dllName, EntryPoint = "NDI_TryOpenSourceNamedLike")]
        public static extern IntPtr TryOpenSourceNamedLike(string clause);

        [DllImport(_dllName, EntryPoint = "NDI_DestroyReceiver")]
        public static extern void DestroyReceiver(IntPtr receiver);

        [DllImport(_dllName, EntryPoint = "NDI_GetReceiverID")]
        public static extern uint GetReceiverID(IntPtr receiver);

        [DllImport(_dllName, EntryPoint = "NDI_GetFrameWidth")]
        public static extern int GetFrameWidth(IntPtr receiver);

        [DllImport(_dllName, EntryPoint = "NDI_GetFrameHeight")]
        public static extern int GetFrameHeight(IntPtr receiver);

        [DllImport(_dllName, EntryPoint = "NDI_GetFrameFourCC")]
        public static extern FourCC GetFrameFourCC(IntPtr receiver);

        #else

        public static IntPtr TryOpenSourceNamedLike(string clause)
        { return IntPtr.Zero; }

        public static void DestroyReceiver(IntPtr receiver)
        {}

        public static uint GetReceiverID(IntPtr receiver)
        { return 0; }

        public static int GetFrameWidth(IntPtr receiver)
        { return 0; }

        public static int GetFrameHeight(IntPtr receiver)
        { return 0; }

        public static FourCC GetFrameFourCC(IntPtr receiver)
        { return 0; }

        #endif

        #endregion
    }
}
