using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.InteropServices;
using Microsoft.Win32.SafeHandles;
using UnityEngine;

namespace Klak.Ndi.Interop
{

    [System.Serializable]
    class SerializedNDISettings
    {

        public bool UseLocalSources;
        public List<string> ExternalIps;

        //added for completeness but not used
        public List<string> Groups;
    }


    public class Find : SafeHandleZeroOrMinusOneIsInvalid
    {
        #region SafeHandle implementation

        Find() : base(true) { }

        protected override bool ReleaseHandle()
        {
            _Destroy(handle);
            return true;
        }

        #endregion

        #region Public methods

        public static Find Create()
        {
            string jsonPath = Path.Combine(Application.streamingAssetsPath, "NDISettings.json");
            IntPtr ipPtr = IntPtr.Zero;
            bool showLocalSource = true;
            if (File.Exists(jsonPath))
            {
                SerializedNDISettings ndiSettings = JsonUtility.FromJson<SerializedNDISettings>(File.ReadAllText(jsonPath));
                if (ndiSettings.ExternalIps != null)
                {
                    if (ndiSettings.ExternalIps.Count > 0)
                    {
                        string externalIPS = string.Join(",", ndiSettings.ExternalIps);
                        ipPtr = Marshal.StringToHGlobalAnsi(externalIPS);
                    }
                }
                showLocalSource = ndiSettings.UseLocalSources;
                
            }
            var settings = new Settings { ShowLocalSources = showLocalSource, ExtraIPs = ipPtr };

            return _Create(settings);
        }

        unsafe public Span<Source> CurrentSources
        {
            get
            {
                uint count;
                var array = _GetCurrentSources(this, out count);
                return new Span<Source>((void*)array, (int)count);
            }
        }

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
