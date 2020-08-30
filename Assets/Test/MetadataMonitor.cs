using UnityEngine;
using Klak.Ndi;

class MetadataMonitor : MonoBehaviour
{
    [SerializeField] UnityEngine.UI.Text _output = null;

    NdiReceiver _receiver;

    void Start()
      => _receiver = GetComponent<NdiReceiver>();

    void Update()
      => _output.text = _receiver.metadata;
}
