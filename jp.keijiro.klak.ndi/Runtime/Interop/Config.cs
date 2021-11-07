namespace Klak.Ndi.Interop {

static class Config
{
    #if UNITY_EDITOR_WIN || (!UNITY_EDITOR && UNITY_STANDALONE_WIN)
    public const string DllName = "Processing.NDI.Lib.x64";
    #elif UNITY_EDITOR_OSX || (!UNITY_EDITOR && UNITY_STANDALONE_OSX)
    public const string DllName = "libndi";
    #elif UNITY_EDITOR_LINUX || UNITY_STANDALONE_LINUX || UNITY_ANDROID
    public const string DllName = "ndi";
    #else
    public const string DllName = "__Internal";
    #endif
}

} // namespace Klak.Ndi.Interop
