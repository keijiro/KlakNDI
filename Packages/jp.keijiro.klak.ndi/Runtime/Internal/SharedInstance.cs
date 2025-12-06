namespace Klak.Ndi {

// A wrangler class managing singleton-like shared object instances
static class SharedInstance
{
    #region Public properties

    // NDI find object
    static public Interop.Find Find
      => _find ?? InitializeFind();

    // NDI send object for the game view
    static public Interop.Send GameViewSend
      => _gameViewSend ?? InitializeGameViewSend();

    static public bool IsGameViewSend(Interop.Send send)
      => send != null && send == _gameViewSend;

    #endregion

    #region Shared object implementation

    static Interop.Find _find;

    static Interop.Find InitializeFind()
    {
        _find = Interop.Find.Create();
        SetFinalizer();
        return _find;
    }

    static Interop.Send _gameViewSend;

    static Interop.Send InitializeGameViewSend()
    {
        _gameViewSend = Interop.Send.Create("Game View");
        SetFinalizer();
        return _gameViewSend;
    }

    #endregion

    #region Initializer implementation

    static SharedInstance()
    {
        #if UNITY_ANDROID && !UNITY_EDITOR
        AndroidHelper.SetupNetwork();
        #endif
    }

    #endregion

    #region Finalizer implementatioin

    // We have to clean up the shared objects on a domain reload.
    // (This happens only on Editor.)

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

} // namespace Klak.Ndi
