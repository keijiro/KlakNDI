using UnityEngine;
using Klak.Ndi;

sealed class MetadataWriter : MonoBehaviour
{
    void Update()
    {
        var sender = GetComponent<NdiSender>();
        sender.metadata = $"<time frame=\"{Time.frameCount}\"/>";
    }
}
