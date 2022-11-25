using UnityEngine;
using UnityEngine.UI;
using Klak.Ndi;

sealed class MetadataReader : MonoBehaviour
{
    [SerializeField] Text _label = null;

    void Update()
    {
        var receiver = GetComponent<NdiReceiver>();
        _label.text = receiver.metadata;
    }
}
