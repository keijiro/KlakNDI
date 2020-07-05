using UnityEngine;
using UnityEditor;
using UnityEditorInternal;

namespace Klak.Ndi.Editor
{
    #if DONT_COMPILE_THIS

    static class NdiResourcesEditor
    {
        [MenuItem("Assets/Create/NDI/Resources")]
        public static void CreateNdiResourcesAsset()
        {
            var asset = ScriptableObject.CreateInstance<NdiResources>();
            ProjectWindowUtil.CreateAsset(asset, "NdiResources.asset");
        }
    }

    #endif
}
