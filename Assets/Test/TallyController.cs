using UnityEngine;
using Klak.Ndi;
using Klak.Ndi.Interop;

class TallyController : MonoBehaviour
{
    [SerializeField] NdiReceiver _receiver = null;
    [SerializeField] bool _onProgram = false;
    [SerializeField] bool _onPreview = false;

    void Update()
    {
        if (_receiver == null) return;

        var recv = _receiver.internalRecvObject;
        if (recv == null || recv.IsInvalid || recv.IsClosed) return;

        recv.SetTally(new Tally { OnProgram = _onProgram,
                                  OnPreview = _onPreview });
    }
}
