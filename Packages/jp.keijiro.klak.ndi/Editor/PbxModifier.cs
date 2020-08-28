#if UNITY_IOS

using UnityEditor;
using UnityEditor.Callbacks;
using UnityEditor.iOS.Xcode;
using System.IO;

namespace Klak.Ndi.Editor {

// Xcode project file modifier for iOS support
public class PbxModifier
{
    [PostProcessBuild]
    public static void OnPostprocessBuild(BuildTarget buildTarget, string path)
    {
        if (buildTarget == BuildTarget.iOS)
        {
            string projPath = path + "/Unity-iPhone.xcodeproj/project.pbxproj";

            PBXProject proj = new PBXProject();
            proj.ReadFromString(File.ReadAllText(projPath));

            string target = proj.GetUnityFrameworkTargetGuid();
            proj.AddBuildProperty(target, "LIBRARY_SEARCH_PATHS",
                                  "/Library/NDI\\ SDK\\ for\\ Apple/lib/iOS");
            proj.AddFrameworkToProject(target, "VideoToolbox.framework", false);
            proj.AddFrameworkToProject(target, "libndi_ios.a", false);

            File.WriteAllText(projPath, proj.WriteToString());
        }
    }
}

}

#endif
