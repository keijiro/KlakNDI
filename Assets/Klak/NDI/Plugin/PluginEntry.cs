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

        [DllImport(_dllName)]
        public static extern IntPtr NDI_GetTextureUpdateCallback();

        [DllImport(_dllName)]
        public static extern int NDI_RetrieveSourceNames(IntPtr[] destination, int maxCount);

        #else

        public static IntPtr NDI_GetTextureUpdateCallback() { return IntPtr.Zero; }
        public static int NDI_RetrieveSourceNames(IntPtr[] destination, int maxCount) { return 0; }

        #endif

        #endregion

        #region Sender functions

        [DllImport(_dllName)]
        public static extern IntPtr NDI_CreateSender(string name);

        [DllImport(_dllName)]
        public static extern void NDI_DestroySender(IntPtr sender);

        [DllImport(_dllName)]
        public static extern void NDI_SendFrame(IntPtr sender, IntPtr data, int width, int height, FourCC fourCC);

        [DllImport(_dllName)]
        public static extern void NDI_SyncSender(IntPtr sender);

        #endregion

        #region Receiver functions

        #if !NDI_SENDER_ONLY

        [DllImport(_dllName)]
        public static extern IntPtr NDI_TryOpenSourceNamedLike(string clause);

        [DllImport(_dllName)]
        public static extern void NDI_DestroyReceiver(IntPtr receiver);

        [DllImport(_dllName)]
        public static extern uint NDI_GetReceiverID(IntPtr receiver);

        [DllImport(_dllName)]
        public static extern int NDI_GetFrameWidth(IntPtr receiver);

        [DllImport(_dllName)]
        public static extern int NDI_GetFrameHeight(IntPtr receiver);

        [DllImport(_dllName)]
        public static extern FourCC NDI_GetFrameFourCC(IntPtr receiver);

        #else

        public static IntPtr NDI_TryOpenSourceNamedLike(string clause) { return IntPtr.Zero; }
        public static void NDI_DestroyReceiver(IntPtr receiver) {}
        public static uint NDI_GetReceiverID(IntPtr receiver) { return 0; }
        public static int NDI_GetFrameWidth(IntPtr receiver) { return 0; }
        public static int NDI_GetFrameHeight(IntPtr receiver) { return 0; }
        public static FourCC NDI_GetFrameFourCC(IntPtr receiver) { return 0; }

        #endif

        #endregion
    }
}
