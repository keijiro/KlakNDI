// KlakNDI - NDI plugin for Unity
// https://github.com/keijiro/KlakNDI

#if UNITY_IOS

using UnityEngine;
using UnityEditor;
using UnityEditor.Callbacks;
using UnityEditor.iOS.Xcode;
using System.IO;

namespace Klak.NdiLite
{
    sealed class PbxModifier
    {
        [PostProcessBuild]
        static void OnPostprocessBuild(BuildTarget buildTarget, string path)
        {
            if (buildTarget == BuildTarget.iOS)
            {
                string projPath = path + "/Unity-iPhone.xcodeproj/project.pbxproj";

                PBXProject proj = new PBXProject();
                proj.ReadFromString(File.ReadAllText(projPath));

                string target = proj.TargetGuidByName("Unity-iPhone");

                // Add the header/library search path.
                proj.AddBuildProperty(target, "HEADER_SEARCH_PATHS", "/NewTek\\ NDI\\ SDK/include");
                proj.AddBuildProperty(target, "LIBRARY_SEARCH_PATHS", "/NewTek\\ NDI\\ SDK/lib/iOS");

                // Add the NDI library to the build phase.
                proj.AddFrameworkToProject(target, "libndi_ios.a", false);

                File.WriteAllText(projPath, proj.WriteToString());
            }
        }
    }
}

#endif
