// KlakNDI - NDI plugin for Unity
// https://github.com/keijiro/KlakNDI

using UnityEngine;
using UnityEditor;

namespace Klak.Ndi
{
    [CustomEditor(typeof(NdiSender))]
    public class NdiSenderEditor : Editor
    {
        SerializedProperty _sourceTexture;

        void OnEnable()
        {
            _sourceTexture = serializedObject.FindProperty("_sourceTexture");
        }

        public override void OnInspectorGUI()
        {
            serializedObject.Update();

            var sender = (NdiSender)target;
            var camera = sender.GetComponent<Camera>();

            if (camera != null)
            {
                EditorGUILayout.HelpBox(
                    "NDI Sender is running in camera capture mode.",
                    MessageType.None
                );
            }
            else
            {
                EditorGUILayout.HelpBox(
                    "NDI Sender is running in render texture mode.",
                    MessageType.None
                );

                EditorGUILayout.PropertyField(_sourceTexture);
            }

            serializedObject.ApplyModifiedProperties();
        }
    }
}
