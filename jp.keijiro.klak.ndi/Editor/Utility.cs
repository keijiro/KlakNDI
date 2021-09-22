using UnityEngine;
using UnityEditor;
using System.Reflection;

namespace Klak.Ndi.Editor {

// Simple string label with GUIContent
struct Label
{
    GUIContent _guiContent;

    public static implicit operator GUIContent(Label label)
      => label._guiContent;

    public static implicit operator Label(string text)
      => new Label { _guiContent = new GUIContent(text) };
}

// Auto-scanning serialized property wrapper
struct AutoProperty
{
    SerializedProperty _prop;

    public SerializedProperty Target => _prop;

    public AutoProperty(SerializedProperty prop)
      => _prop = prop;

    public static implicit operator
      SerializedProperty(AutoProperty prop) => prop._prop;

    public static void Scan<T>(T target) where T : UnityEditor.Editor
    {
        var so = target.serializedObject;

        var flags = BindingFlags.Public | BindingFlags.NonPublic;
        flags |= BindingFlags.Instance;

        foreach (var f in typeof(T).GetFields(flags))
            if (f.FieldType == typeof(AutoProperty))
                f.SetValue(target, new AutoProperty(so.FindProperty(f.Name)));
    }
}

} // namespace Klak.Ndi.Editor
