using System;
using System.Runtime.InteropServices;

namespace Klak.Ndi
{
    public static class PluginEntry
    {
        #region Common functions

        [DllImport("KlakNDI")]
        public static extern IntPtr NDI_GetTextureUpdateCallback();

        [DllImport("KlakNDI")]
        public static extern int NDI_RetrieveSourceNames(IntPtr[] destination, int maxCount);

        #endregion

        #region Sender functions

        [DllImport("KlakNDI")]
        public static extern IntPtr NDI_CreateSender(string name);

        [DllImport("KlakNDI")]
        public static extern void NDI_DestroySender(IntPtr sender);

        [DllImport("KlakNDI")]
        public static extern void NDI_SendFrame(IntPtr sender, IntPtr data, int width, int height);

        #endregion

        #region Receiver functions

        [DllImport("KlakNDI")]
        public static extern IntPtr NDI_TryOpenSourceNamedLike(string clause);

        [DllImport("KlakNDI")]
        public static extern void NDI_DestroyReceiver(IntPtr receiver);

        [DllImport("KlakNDI")]
        public static extern uint NDI_GetReceiverID(IntPtr receiver);

        [DllImport("KlakNDI")]
        public static extern int NDI_GetFrameWidth(IntPtr receiver);

        [DllImport("KlakNDI")]
        public static extern int NDI_GetFrameHeight(IntPtr receiver);

        #endregion
    }
}
