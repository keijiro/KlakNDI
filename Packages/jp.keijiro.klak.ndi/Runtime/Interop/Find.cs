using System;
using System.Runtime.InteropServices;
using Microsoft.Win32.SafeHandles;

namespace Klak.NDI.Interop {

public class Find : SafeHandleZeroOrMinusOneIsInvalid
{
    #region SafeHandle implementation

    Find() : base(true) {}

    protected override bool ReleaseHandle()
    {
        _Destroy(handle);
        return true;
    }

    #endregion

    #region Public methods

    public static Find Create()
      => _Create(new Settings { ShowLocalSources = true });

    unsafe public Span<Source> CurrentSources { get {
        uint count;
        var array = _GetCurrentSources(this, out count);
        return new Span<Source>((void*)array, (int)count);
    } }

    #endregion

    #region Unmanaged interface

    [StructLayoutAttribute(LayoutKind.Sequential)]
    public struct Settings 
    {
        [MarshalAsAttribute(UnmanagedType.U1)] public bool ShowLocalSources;
        public IntPtr Groups;
        public IntPtr ExtraIPs;
    }

    [DllImport(Config.DllName, EntryPoint = "NDIlib_find_create_v2")]
    static extern Find _Create(in Settings settings);

    [DllImport(Config.DllName, EntryPoint = "NDIlib_find_destroy")]
    static extern void _Destroy(IntPtr find);

    [DllImport(Config.DllName, EntryPoint = "NDIlib_find_get_current_sources")]
    static extern IntPtr _GetCurrentSources(Find find, out uint count);

    #endregion
}

}
