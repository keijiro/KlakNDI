using UnityEngine;
using UnityEditor;

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

// Utilities for finding serialized properties
struct PropertyFinder
{
    SerializedObject _so;

    public PropertyFinder(SerializedObject so)
      => _so = so;

    public SerializedProperty this[string name]
      => _so.FindProperty(name);
}

}
