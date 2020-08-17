namespace Klak.Ndi {

static class SharedInstance
{
    static public Interop.Find Find => GetFind();
    static public Interop.Send GameViewSend => GetGameViewSend();

    static bool _initialized;
    static Interop.Find _find;
    static Interop.Send _gameViewSend;

    static Interop.Find GetFind()
    {
        Setup();
        return _find;
    }

    static Interop.Send GetGameViewSend()
    {
        Setup();
        return _gameViewSend;
    }

    static void Setup()
    {
        if (_initialized) return;

    #if UNITY_EDITOR
        UnityEditor.AssemblyReloadEvents.beforeAssemblyReload += OnDomainReload;
    #endif

        _find = Interop.Find.Create();
        _gameViewSend = Interop.Send.Create("Game View");

        _initialized = true;
    }

    static void OnDomainReload()
    {
        _find?.Dispose();
        _find = null;

        _gameViewSend?.Dispose();
        _gameViewSend = null;

        _initialized = false;
    }
}

}
