using System;
using System.Runtime.InteropServices;
using Microsoft.Win32.SafeHandles;

namespace Klak.Ndi.Interop 
{
    // Bandwidth enumeration (equivalent to NDIlib_recv_bandwidth_e)
    public enum Bandwidth
    {
        MetadataOnly = -10,
        AudioOnly = 10,
        Lowest = 0,
        Highest = 100
    }

    // Color format enumeration (equivalent to NDIlib_recv_color_format_e)
    public enum ColorFormat
    {
        BGRX_BGRA = 0,
        UYVY_BGRA = 1,
        RGBX_RGBA = 2,
        UYVY_RGBA = 3,
        BGRX_BGRA_Flipped = 200,
        Fastest = 100,
        Best = 101
    }

    public class Recv : SafeHandleZeroOrMinusOneIsInvalid
    {
        #region SafeHandle implementation

        Recv() : base(true) {}

        protected override bool ReleaseHandle()
        {
            _Destroy(handle);
            return true;
        }

        #endregion

        #region Public methods

        public static Recv Create(in Settings settings)
          => _Create(settings);

        public FrameType CaptureVideo (out VideoFrame video, IntPtr audio, IntPtr metadata, uint timeout)
            => _CaptureVideo(this, out video, audio, metadata, timeout);

        public FrameType CaptureAudio (out AudioFrame audio, IntPtr video, IntPtr metadata, uint timeout)
            => _CaptureAudio(this, video, out audio, metadata, timeout);

        public FrameType CaptureAll(out VideoFrame video, out AudioFrame audio, IntPtr metadata, uint timeout) 
            => _CaptureAll(this, out video, out audio, metadata, timeout);

        public void FreeVideoFrame(in VideoFrame frame)
          => _FreeVideo(this, frame);

        public bool SetTally(in Tally tally)
          => _SetTally(this, tally);

        #endregion

        #region Unmanaged interface

        // Constructor options (equivalent to NDIlib_recv_create_v3_t)
        [StructLayout(LayoutKind.Sequential)]
        public struct Settings
        {
            public Source Source;
            public ColorFormat ColorFormat;
            public Bandwidth Bandwidth;
            [MarshalAs(UnmanagedType.U1)] public bool AllowVideoFields;
            public IntPtr Name;
        }

        [DllImport(Config.DllName, EntryPoint = "NDIlib_recv_create_v3")]
        static extern Recv _Create(in Settings Settings);

        [DllImport(Config.DllName, EntryPoint = "NDIlib_recv_destroy")]
        static extern void _Destroy(IntPtr recv);


        [DllImport(Config.DllName, EntryPoint = "NDIlib_recv_capture_v2")]
        static extern FrameType _CaptureVideo (Recv recv, out VideoFrame video, IntPtr audio, IntPtr metadata, uint timeout);

        [DllImport(Config.DllName, EntryPoint = "NDIlib_recv_capture_v2")]
        static extern FrameType _CaptureAudio(Recv recv, IntPtr video, out AudioFrame audio, IntPtr metadata, uint timeout);

        [DllImport(Config.DllName, EntryPoint = "NDIlib_recv_free_video_v2")]
        static extern void _FreeVideo(Recv recv, in VideoFrame data);

        [DllImport(Config.DllName, EntryPoint = "NDIlib_recv_free_audio_v2")]
        static extern void _FreeAudio(Recv recv, in AudioFrame data);


        [DllImport(Config.DllName, EntryPoint = "NDIlib_util_audio_to_interleaved_32f_v2")]
        static extern void _AudioFrameToInterleaved(ref AudioFrame src, ref AudioFrameInterleaved dst);

        [DllImport(Config.DllName, EntryPoint = "NDIlib_recv_set_tally")]
        [return: MarshalAs(UnmanagedType.U1)]
        static extern bool _SetTally(Recv recv, in Tally tally);


        [DllImport(Config.DllName, EntryPoint = "NDIlib_recv_capture_v2")]
        static extern FrameType _CaptureAll(Recv recv, out VideoFrame video, out AudioFrame audio, IntPtr metadata, uint timeout);

        #endregion
    }

} // namespace Klak.Ndi.Interop
