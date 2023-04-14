using System.Runtime.InteropServices;
using IntPtr = System.IntPtr;

namespace Klak.Ndi {

    // Small helper class for NDI recv interop
    static class RecvHelper
    {
        public static Interop.Source? FindSource(string sourceName)
        {
            foreach (var source in SharedInstance.Find.CurrentSources)
                if (source.NdiName == sourceName) return source;
            return null;
        }

        public static unsafe Interop.Recv TryCreateRecv(string sourceName)
        {
            var source = FindSource(sourceName);
            if (source == null) return null;

            var opt = new Interop.Recv.Settings { 
                Source = (Interop.Source)source,
                ColorFormat = Interop.ColorFormat.Fastest,
                Bandwidth = Interop.Bandwidth.Highest 
            };

            return Interop.Recv.Create(opt);
        }

        public static Interop.VideoFrame? TryCaptureVideoFrame(Interop.Recv recv)
        {
            Interop.VideoFrame video;
            var type = recv.CaptureVideo(out video, IntPtr.Zero, IntPtr.Zero, 0);
            if (type != Interop.FrameType.Video) return null;
            return (Interop.VideoFrame?)video;
        }

        public static Interop.AudioFrame? TryCaptureAudioFrame(Interop.Recv recv)
        {
            Interop.AudioFrame audio;
            var type = recv.CaptureAudio(out audio, IntPtr.Zero, IntPtr.Zero, 0);
            if (type != Interop.FrameType.Audio) return null;
            return (Interop.AudioFrame?)audio;
        }

        public static (Interop.VideoFrame?, Interop.AudioFrame?) TryCaptureFrame(Interop.Recv recv)
        {
            Interop.VideoFrame video;
            Interop.AudioFrame audio;
            Interop.FrameType type = recv.CaptureAll(out video, out audio, IntPtr.Zero, 0);
            if (type == Interop.FrameType.None) return (null, null);
            return ((Interop.VideoFrame?)video, (Interop.AudioFrame?)audio);
        }

        public static string GetStringData(IntPtr dataPtr)
        {
            if (dataPtr == IntPtr.Zero)
                return null;

            return Marshal.PtrToStringAnsi(dataPtr);
        }
    }

} // namespace Klak.Ndi
