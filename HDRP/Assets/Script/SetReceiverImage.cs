using UnityEngine;
using UI = UnityEngine.UI;
using Klak.Ndi;

public sealed class SetReceiverImage : MonoBehaviour
{
    [SerializeField] NdiReceiver _receiver = null;

    void Update()
      => GetComponent<UI.RawImage>().texture = _receiver.texture;
}
