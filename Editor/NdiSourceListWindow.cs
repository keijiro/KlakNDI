using UnityEngine;
using UnityEditor;
using System;
using System.Runtime.InteropServices;

namespace Klak.Ndi
{
    // NDI source list window
    public class NdiSourceListWindow : EditorWindow
    {
        [MenuItem("Window/Klak/NDI Source List")]
        static void Init()
        {
            EditorWindow.GetWindow<NdiSourceListWindow>("NDI Sources").Show();
        }

        IntPtr[] _sources;
        int _updateCount;

        void OnInspectorUpdate()
        {
            // Update once per eight calls.
            if ((_updateCount++ & 7) == 0) Repaint();
        }

        void OnGUI()
        {
            if (_sources == null) _sources = new IntPtr[128];

            var count = PluginEntry.NDI_RetrieveSourceNames(_sources, _sources.Length);

            EditorGUILayout.Space();
            EditorGUI.indentLevel++;

            if (count == 0)
                EditorGUILayout.LabelField("No source found.");
            else
                EditorGUILayout.LabelField(count + " source(s) found.");

            for (var i = 0; i < count; i++)
            {
                var name = Marshal.PtrToStringAnsi(_sources[i]);
                if (name != null) EditorGUILayout.LabelField("- " + name);
            }

            EditorGUI.indentLevel--;
        }
    }
}
