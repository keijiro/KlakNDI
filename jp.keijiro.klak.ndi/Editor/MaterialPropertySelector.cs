using UnityEngine;
using UnityEditor;
using System.Linq;

namespace Klak.Ndi.Editor {

static class MaterialPropertySelector
{
    #region Public method

    // Material property dropdown list
    public static void DropdownList
      (SerializedProperty rendererProperty,
       SerializedProperty materialProperty)
    {
        var shader = GetShaderFromRenderer(rendererProperty);

        // Abandon the current value if there is no shader assignment.
        if (shader == null)
        {
            materialProperty.stringValue = "";
            return;
        }

        var names = CachePropertyNames(shader);

        // Abandon the current value if there is no option.
        if (names.Length == 0)
        {
            materialProperty.stringValue = "";
            return;
        }

        // Dropdown GUI
        var index = System.Array.IndexOf(names, materialProperty.stringValue);
        var newIndex = EditorGUILayout.Popup("Property", index, names);
        if (index != newIndex) materialProperty.stringValue = names[newIndex];
    }

    #endregion

    #region Utility function

    // Shader retrieval function
    static Shader GetShaderFromRenderer(SerializedProperty property)
    {
        var renderer = property.objectReferenceValue as Renderer;
        if (renderer == null) return null;

        var material = renderer.sharedMaterial;
        if (material == null) return null;

        return material.shader;
    }

    #endregion

    #region Property name cache

    static Shader _cachedShader;
    static string[] _cachedPropertyNames;

    static bool IsPropertyTexture(Shader shader, int index)
      => ShaderUtil.GetPropertyType(shader, index) ==
         ShaderUtil.ShaderPropertyType.TexEnv;

    static string[] CachePropertyNames(Shader shader)
    {
        if (shader == _cachedShader) return _cachedPropertyNames;

        var names =
          Enumerable.Range(0, ShaderUtil.GetPropertyCount(shader))
          .Where(i => IsPropertyTexture(shader, i))
          .Select(i => ShaderUtil.GetPropertyName(shader, i));

        _cachedShader = shader;
        _cachedPropertyNames = names.ToArray();

        return _cachedPropertyNames;
    }

    #endregion
}

} // namespace Klak.Ndi.Editor
