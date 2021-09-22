using System;
using System.Runtime.InteropServices;

namespace Klak.Ndi.Interop {

// Packet type enumeration (equivalent to NDIlib_frame_type_e)
public enum FrameType
{
    None = 0,
    Video = 1,
    Audio = 2,
    Metadata = 3,
    Error = 4,
    StatusChange = 100
}

// FourCC values for video/audio frames
public enum FourCC
{
    // Video
    UYVY = 0x59565955,
    UYVA = 0x41565955,
    P216 = 0x36313250,
    PA16 = 0x36314150,
    YV12 = 0x32315659,
    I420 = 0x30323449,
    NV12 = 0x3231564E,
    BGRA = 0x41524742,
    BGRX = 0x58524742,
    RGBA = 0x41424752,
    RGBX = 0x58424752,
    // Audio
    FLTp = 0x70544c46
}

// Frame format enumeration (equivalent to NDIlib_frame_format_type_e)
public enum FrameFormat
{
    Interleaved,
    Progressive,
    Field0,
    Field1
}

// NDI source descriptor (equivalent to NDIlib_source_t)
[StructLayout(LayoutKind.Sequential)]
public struct Source
{
    IntPtr _NdiName;
    IntPtr _UrlAddress;
    public string NdiName => Marshal.PtrToStringAnsi(_NdiName);
    public string UrlAddress => Marshal.PtrToStringAnsi(_UrlAddress);
}

// Video frame descriptor (equivalent to NDILib_video_frame_v2_t)
[StructLayout(LayoutKind.Sequential)]
public struct VideoFrame
{
    public int Width, Height;
    public FourCC FourCC;
    public int FrameRateN, FrameRateD;
    public float AspectRatio;
    public FrameFormat FrameFormat;
    public long Timecode;
    public IntPtr Data;
    public int LineStride;
    public IntPtr Metadata;
    public long Timestamp;
}

// Tally data structure (equivalent to NDIlib_tally_t)
[StructLayout(LayoutKind.Sequential)]
public struct Tally
{
    [MarshalAs(UnmanagedType.U1)] public bool OnProgram;
    [MarshalAs(UnmanagedType.U1)] public bool OnPreview;
}

} // namespace Klak.Ndi.Interop
