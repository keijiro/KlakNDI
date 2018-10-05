// KlakNDI - NDI plugin for Unity
// https://github.com/keijiro/KlakNDI

using UnityEngine;
using UnityEditor;
using System.Collections.Generic;
using System.Runtime.InteropServices;

namespace Klak.Ndi
{
    [CanEditMultipleObjects]
    [CustomEditor(typeof(NdiReceiver))]
    public sealed class NdiReceiverEditor : Editor
    {
        SerializedProperty _sourceName;
        SerializedProperty _targetTexture;
        SerializedProperty _targetRenderer;
        SerializedProperty _targetMaterialProperty;

        List<string> _sourceNames = new List<string>();

        static double _prevRepaintTime;

        static class Labels
        {
            public static readonly GUIContent Property = new GUIContent("Property");
            public static readonly GUIContent Select = new GUIContent("Select");
        }

        // Request receiver reconnection.
        void RequestReconnect()
        {
            foreach (NdiReceiver receiver in targets) receiver.RequestReconnect();
        }

        // Check and request repaint with 0.1s interval.
        void CheckRepaint()
        {
            var time = EditorApplication.timeSinceStartup;
            if (time - _prevRepaintTime < 0.1) return;
            UnityEditorInternal.InternalEditorUtility.RepaintAllViews();
            _prevRepaintTime = time;
        }

        // Create and show the source name dropdown.
        void ShowSourceNameDropdown(Rect rect)
        {
            var menu = new GenericMenu();

            NdiManager.GetSourceNames(_sourceNames);

            if (_sourceNames.Count > 0)
            {
                foreach (var name in _sourceNames)
                    menu.AddItem(new GUIContent(name), false, OnSelectSource, name);
            }
            else
            {
                menu.AddItem(new GUIContent("No source available"), false, null);
            }

            menu.DropDown(rect);
        }

        // Source name selection callback
        void OnSelectSource(object name)
        {
            serializedObject.Update();
            _sourceName.stringValue = (string)name;
            serializedObject.ApplyModifiedProperties();
            RequestReconnect();
        }

        void OnEnable()
        {
            _sourceName = serializedObject.FindProperty("_sourceName");
            _targetTexture = serializedObject.FindProperty("_targetTexture");
            _targetRenderer = serializedObject.FindProperty("_targetRenderer");
            _targetMaterialProperty = serializedObject.FindProperty("_targetMaterialProperty");

            EditorApplication.update += CheckRepaint;
        }

        void OnDisable()
        {
            EditorApplication.update -= CheckRepaint;
        }

        public override void OnInspectorGUI()
        {
            serializedObject.Update();

            EditorGUILayout.BeginHorizontal();

            // Source name text field
            EditorGUI.BeginChangeCheck();
            EditorGUILayout.DelayedTextField(_sourceName);
            if (EditorGUI.EndChangeCheck()) RequestReconnect();

            // Source name dropdown
            var rect = EditorGUILayout.GetControlRect(false, GUILayout.Width(60));
            if (EditorGUI.DropdownButton(rect, Labels.Select, FocusType.Keyboard))
                ShowSourceNameDropdown(rect);

            EditorGUILayout.EndHorizontal();

            // Target texture/renderer
            EditorGUILayout.PropertyField(_targetTexture);
            EditorGUILayout.PropertyField(_targetRenderer);

            EditorGUI.indentLevel++;

            if (_targetRenderer.hasMultipleDifferentValues)
            {
                // Multiple renderers selected: Show a simple text field.
                EditorGUILayout.PropertyField(_targetMaterialProperty, Labels.Property);
            }
            else if (_targetRenderer.objectReferenceValue != null)
            {
                // Single renderer: Show the material property selection dropdown.
                MaterialPropertySelector.DropdownList(_targetRenderer, _targetMaterialProperty);
            }

            EditorGUI.indentLevel--;

            serializedObject.ApplyModifiedProperties();
        }
    }
}
