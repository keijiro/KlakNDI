namespace Klak.Ndi {

static class SharedInstance
{
    #region Public properties

    static public Interop.Find Find => GetFind();
    static public Interop.Send GameViewSend => GetGameViewSend();

    #endregion

    #region Shared object implementation

    static Interop.Find _find;

    static Interop.Find GetFind()
    {
        if (_find == null)
        {
            _find = Interop.Find.Create();
            SetFinalizer();
        }
        return _find;
    }

    static Interop.Send _gameViewSend;

    static Interop.Send GetGameViewSend()
    {
        if (_gameViewSend == null)
        {
            _gameViewSend = Interop.Send.Create("Game View");
            SetFinalizer();
        }
        return _gameViewSend;
    }

    #endregion

    #region Finalizer implementatioin

    //
    // We have to clean up the shared objects on a domain reload that only
    // happens on Editor, so we do nothing on Player.
    //

    #if UNITY_EDITOR

    static bool _finalizerReady;

    static void SetFinalizer()
    {
        if (_finalizerReady) return;
        UnityEditor.AssemblyReloadEvents.beforeAssemblyReload += OnDomainReload;
        _finalizerReady = true;
    }

    static void OnDomainReload()
    {
        _find?.Dispose();
        _find = null;

        _gameViewSend?.Dispose();
        _gameViewSend = null;
    }

    #else

    static void SetFinalizer() {}

    #endif

    #endregion
}

}
