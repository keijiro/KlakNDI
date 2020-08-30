using UnityEngine;
using Klak.Ndi;

class MetadataAdapter : MonoBehaviour
{
    [SerializeField] bool _frameNumber = false;
    [SerializeField] string _extraText = null;

    NdiSender _sender;

    void Start()
      => _sender = GetComponent<NdiSender>();

    void Update()
    {
        var text = "";

        if (_frameNumber)
            text += $" number=\"{Time.frameCount}\"";

        if (!string.IsNullOrEmpty(_extraText))
            text += $" text=\"{_extraText}\"";

        if (text.Length == 0)
            _sender.metadata = null;
        else
            _sender.metadata = $"<frame {text}/>";
    }
}
