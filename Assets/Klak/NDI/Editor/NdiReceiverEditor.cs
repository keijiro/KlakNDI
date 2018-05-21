// KlakNDI - NDI plugin for Unity
// https://github.com/keijiro/KlakNDI

using UnityEngine;
using UnityEditor;
using System;
using System.Collections.Generic;

namespace Klak.Ndi
{
    [CustomEditor(typeof(NdiReceiver))]
    public class NdiReceiverEditor : Editor
    {
        SerializedProperty _nameFilter;
        SerializedProperty _targetTexture;
        SerializedProperty _targetRenderer;
        SerializedProperty _targetMaterialProperty;

        static GUIContent _labelProperty = new GUIContent("Property");

        string[] _propertyList; // Cached property list
        Shader _cachedShader;   // Shader stored in the cache

        // Retrieve the shader from the target renderer.
        Shader RetrieveTargetShader(UnityEngine.Object target)
        {
            var renderer = target as Renderer;
            if (renderer == null) return null;

            var material = renderer.sharedMaterial;
            if (material == null) return null;

            return material.shader;
        }

        // Cache the properties of the given shader .
        void CachePropertyList(Shader shader)
        {
            // Do nothing if the shader is same to the cached one.
            if (_cachedShader == shader) return;

            var temp = new List<string>();

            var count = ShaderUtil.GetPropertyCount(shader);
            for (var i = 0; i < count; i++)
            {
                var propType = ShaderUtil.GetPropertyType(shader, i);
                if (propType == ShaderUtil.ShaderPropertyType.TexEnv)
                    temp.Add(ShaderUtil.GetPropertyName(shader, i));
            }

            _propertyList = temp.ToArray();
            _cachedShader = shader;
        }

        // Material property drop-down list
        void ShowMaterialPropertyDropDown()
        {
            // Try retrieving the target shader.
            var shader = RetrieveTargetShader(_targetRenderer.objectReferenceValue);

            if (shader == null)
            {
                _targetMaterialProperty.stringValue = ""; // reset on failure
                return;
            }

            // Cache the properties of the target shader.
            CachePropertyList(shader);

            // Check if there is suitable candidate.
            if (_propertyList.Length == 0)
            {
                _targetMaterialProperty.stringValue = ""; // reset on failure
                return;
            }

            // Show the drop-down list.
            var index = Array.IndexOf(_propertyList, _targetMaterialProperty.stringValue);
            var newIndex = EditorGUILayout.Popup("Property", index, _propertyList);

            // Update the property if the selection was changed.
            if (index != newIndex)
                _targetMaterialProperty.stringValue = _propertyList[newIndex];
        }

        void OnEnable()
        {
            _nameFilter = serializedObject.FindProperty("_nameFilter");
            _targetTexture = serializedObject.FindProperty("_targetTexture");
            _targetRenderer = serializedObject.FindProperty("_targetRenderer");
            _targetMaterialProperty = serializedObject.FindProperty("_targetMaterialProperty");
        }

        void OnDisable()
        {
            _propertyList = null;
            _cachedShader = null;
        }

        public override bool RequiresConstantRepaint()
        {
            return true;
        }

        public override void OnInspectorGUI()
        {
            serializedObject.Update();

            EditorGUI.BeginChangeCheck();
            EditorGUILayout.DelayedTextField(_nameFilter);
            if (EditorGUI.EndChangeCheck())
            {
                // Flip-flipping the target to reset the connection.
                // It's needed to apply the new name filter value.
                var recv = (NdiReceiver)target;
                recv.enabled = false;
                recv.enabled = true;
            }

            EditorGUILayout.PropertyField(_targetTexture);
            EditorGUILayout.PropertyField(_targetRenderer);

            EditorGUI.indentLevel++;

            if (_targetRenderer.hasMultipleDifferentValues)
            {
                // Show a simple text field if there are multiple values.
                EditorGUILayout.PropertyField(_targetMaterialProperty, _labelProperty);
            }
            else if (_targetRenderer.objectReferenceValue != null)
            {
                // Show the material property drop-down list.
                ShowMaterialPropertyDropDown();
            }

            EditorGUI.indentLevel--;

            serializedObject.ApplyModifiedProperties();
        }
    }
}
