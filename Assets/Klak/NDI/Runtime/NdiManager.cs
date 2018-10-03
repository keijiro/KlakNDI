// KlakNDI - NDI plugin for Unity
// https://github.com/keijiro/KlakNDI

using UnityEngine;
using System.Collections.Generic;
using System.Runtime.InteropServices;

namespace Klak.Ndi
{
    public static class NdiManager
    {
        // Scan available NDI sources and return their names via a newly
        // allocated string array.
        public static string[] GetSourceNames()
        {
            var count = PluginEntry.RetrieveSourceNames(_pointers, _pointers.Length);
            var names = new string [count];
            for (var i = 0; i < count; i++)
                names[i] = Marshal.PtrToStringAnsi(_pointers[i]);
            return names;
        }

        // Scan available NDI sources and store their names into the given
        // collection object.
        public static void GetSourceNames(ICollection<string> store)
        {
            store.Clear();
            var count = PluginEntry.RetrieveSourceNames(_pointers, _pointers.Length);
            for (var i = 0; i < count; i++)
                store.Add(Marshal.PtrToStringAnsi(_pointers[i]));
        }

        static System.IntPtr [] _pointers = new System.IntPtr [256];
    }
}
