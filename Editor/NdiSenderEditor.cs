// KlakNDI - NDI plugin for Unity
// https://github.com/keijiro/KlakNDI

using UnityEngine;
using UnityEditor;

namespace Klak.Ndi
{
    [CanEditMultipleObjects]
    [CustomEditor(typeof(NdiSender))]
    public sealed class NdiSenderEditor : Editor
    {
        SerializedProperty _sourceTexture;
        SerializedProperty _alphaSupport;

        void OnEnable()
        {
            _sourceTexture = serializedObject.FindProperty("_sourceTexture");
            _alphaSupport = serializedObject.FindProperty("_alphaSupport");
        }

        public override void OnInspectorGUI()
        {
            serializedObject.Update();

            if (targets.Length == 1)
            {
                var sender = (NdiSender)target;
                var camera = sender.GetComponent<Camera>();

                if (camera != null)
                {
                    EditorGUILayout.HelpBox(
                        "NDI Sender is running in camera capture mode.",
                        MessageType.None
                    );

                    // Hide the source texture property.
                }
                else
                {
                    EditorGUILayout.HelpBox(
                        "NDI Sender is running in render texture mode.",
                        MessageType.None
                    );

                    EditorGUILayout.PropertyField(_sourceTexture);
                }
            }
            else
                EditorGUILayout.PropertyField(_sourceTexture);

            EditorGUILayout.PropertyField(_alphaSupport);

            serializedObject.ApplyModifiedProperties();
        }
    }
}
