// KlakNDI - NDI plugin for Unity
// https://github.com/keijiro/KlakNDI

using UnityEngine;
using UnityEditor;
using System;
using System.Collections.Generic;

namespace Klak.Ndi
{
    static class MaterialPropertySelector
    {
        #region Public method

        // Material property drop-down list
        public static void DropdownList(
            SerializedProperty rendererProperty,
            SerializedProperty materialProperty
        )
        {
            // Try retrieving the target shader.
            var shader = RetrieveTargetShader(rendererProperty);

            // Abandon the current value if it failed to get a shader.
            if (shader == null)
            {
                materialProperty.stringValue = "";
                return;
            }

            // Cache property names found in the target shader.
            CachePropertyNames(shader);

            // Abandon the current value if there is no property candidate.
            if (_propertyNames.Length == 0)
            {
                materialProperty.stringValue = "";
                return;
            }

            // Show the dropdown list.
            var index = Array.IndexOf(_propertyNames, materialProperty.stringValue);
            var newIndex = EditorGUILayout.Popup("Property", index, _propertyNames);

            // Update the serialized property if the selection was changed.
            if (index != newIndex) materialProperty.stringValue = _propertyNames[newIndex];
        }

        #endregion

        #region Private members

        static string[] _propertyNames; // Property name list
        static Shader _cachedShader;    // Shader used to cache the name list

        // Retrieve a shader from a given renderer.
        static Shader RetrieveTargetShader(SerializedProperty rendererProperty)
        {
            var renderer = rendererProperty.objectReferenceValue as Renderer;
            if (renderer == null) return null;

            var material = renderer.sharedMaterial;
            if (material == null) return null;

            return material.shader;
        }

        // Cache property names provided within a specified shader.
        static void CachePropertyNames(Shader shader)
        {
            // Exit early when the shader is same to the cached one.
            if (shader == _cachedShader) return;

            var temp = new List<string>();

            var count = ShaderUtil.GetPropertyCount(shader);
            for (var i = 0; i < count; i++)
            {
                var propType = ShaderUtil.GetPropertyType(shader, i);
                if (propType == ShaderUtil.ShaderPropertyType.TexEnv)
                    temp.Add(ShaderUtil.GetPropertyName(shader, i));
            }

            _propertyNames = temp.ToArray();
            _cachedShader = shader;
        }

        #endregion
    }
}
