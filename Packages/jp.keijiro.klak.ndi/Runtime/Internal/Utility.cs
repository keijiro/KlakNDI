using UnityEngine;
using BindingFlags = System.Reflection.BindingFlags;
using Delegate = System.Delegate;
using IntPtr = System.IntPtr;

namespace Klak.Ndi
{
    static class Util
    {
        public static int FrameDataCount(int width, int height, bool alpha)
          => width * height * (alpha ? 3 : 2) / 4;

        public static bool CheckAlpha(Interop.FourCC fourCC)
          => fourCC == Interop.FourCC.UYVA;
    }

    //
    // Directly load an unmanaged data array to a compute buffer via an
    // Intptr. This is not a public interface so will be broken one day.
    // DO NOT TRY AT HOME.
    //
    class ComputeDataSetter
    {
        delegate void SetDataDelegate
          (IntPtr pointer, int s_offs, int d_offs, int count, int stride);

        SetDataDelegate _setData;

        public ComputeDataSetter(ComputeBuffer buffer)
        {
            var method = typeof(ComputeBuffer).GetMethod
              ("InternalSetNativeData",
               BindingFlags.InvokeMethod |
               BindingFlags.NonPublic |
               BindingFlags.Instance);

            _setData = (SetDataDelegate)Delegate.CreateDelegate
              (typeof(SetDataDelegate), buffer, method);
        }

        public void SetData(IntPtr pointer, int count, int stride)
          => _setData(pointer, 0, 0, count, stride);
    }
}
