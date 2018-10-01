using UnityEditor;
using UnityEngine;

namespace TestCards
{
    [CustomEditor(typeof(TestOverlay))]
    public class TestOverlayEditor : Editor
    {
        SerializedProperty _mode;
        SerializedProperty _color;
        SerializedProperty _scale;

        void OnEnable()
        {
            _mode = serializedObject.FindProperty("_mode");
            _color = serializedObject.FindProperty("_color");
            _scale = serializedObject.FindProperty("_scale");
        }

        public override void OnInspectorGUI()
        {
            serializedObject.Update();

            EditorGUILayout.PropertyField(_mode);

            if (_mode.intValue == (int)TestOverlay.Mode.Fill)
                EditorGUILayout.PropertyField(_color);

            if (_mode.intValue == (int)TestOverlay.Mode.Checker)
                EditorGUILayout.PropertyField(_scale);

            serializedObject.ApplyModifiedProperties();
        }
    }
}
