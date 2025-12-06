using System;
using System.Runtime.InteropServices;
using Microsoft.Win32.SafeHandles;

namespace Klak.Ndi.Interop {

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

    public Span<Source> CurrentSources
      => GetCurrentSourcesAsSpan();

    #endregion

    #region Unmanaged interface

    // Constructor options (equivalent to NDIlib_find_create_t)
    [StructLayout(LayoutKind.Sequential)]
    public struct Settings 
    {
        [MarshalAs(UnmanagedType.U1)] public bool ShowLocalSources;
        public IntPtr Groups;
        public IntPtr ExtraIPs;
    }

    unsafe Span<Source> GetCurrentSourcesAsSpan()
    {
        uint count;
        var array = _GetCurrentSources(this, out count);
        return new Span<Source>((void*)array, (int)count);
    }

    [DllImport(Config.DllName, EntryPoint = "NDIlib_find_create_v2")]
    static extern Find _Create(in Settings settings);

    [DllImport(Config.DllName, EntryPoint = "NDIlib_find_destroy")]
    static extern void _Destroy(IntPtr find);

    [DllImport(Config.DllName, EntryPoint = "NDIlib_find_get_current_sources")]
    static extern IntPtr _GetCurrentSources(Find find, out uint count);

    #endregion
}

} // namespace Klak.Ndi.Interop
