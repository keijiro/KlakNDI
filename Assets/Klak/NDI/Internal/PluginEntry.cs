using System;
using System.Runtime.InteropServices;

namespace Klak.Ndi
{
    internal static class PluginEntry
    {
        #region Plugin functions

        [DllImport("KlakNDI")]
        public static extern IntPtr NDI_GetTextureUpdateFunction();

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
        public static extern IntPtr NDI_CreateReceiver();

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
