namespace Klak.Ndi {

static class SharedInstance
{
    static public Interop.Find Find => GetFind();

    static bool _initialized;
    static Interop.Find _find;

    static Interop.Find GetFind()
    {
        Setup();
        return _find;
    }

    static void Setup()
    {
        if (_initialized) return;

    #if UNITY_EDITOR
        UnityEditor.AssemblyReloadEvents.beforeAssemblyReload += OnDomainReload;
    #endif

        _find = Interop.Find.Create();

        _initialized = true;
    }

    static void OnDomainReload()
    {
        _find?.Dispose();
        _find = null;

        _initialized = false;
    }
}

}
