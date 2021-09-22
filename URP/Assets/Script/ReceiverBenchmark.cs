using UnityEngine;
using Klak.Ndi;

class ReceiverBenchmark : MonoBehaviour
{
    [SerializeField] Mesh _mesh = null;
    [SerializeField] Material _material = null;
    [SerializeField] NdiResources _resources = null;
    [SerializeField] string _hostName = "";

    GameObject[] _instances = new GameObject[16];

    System.Collections.IEnumerator Start()
    {
        for (var index = 0; index < 16; index++)
            _instances[index] = CreateInstance(index);

        var interval = new WaitForSeconds(0.3f);

        while (true)
        {
            var index = Random.Range(0, 15);

            if (_instances[index] == null)
            {
                _instances[index] = CreateInstance(index);
            }
            else
            {
                Destroy(_instances[index]);
                _instances[index] = null;
            }

            yield return interval;
        }
    }

    GameObject CreateInstance(int index)
    {
        var components = new []
            { typeof(MeshFilter), typeof(MeshRenderer), typeof(NdiReceiver) };

        var go = new GameObject($"Receiver {index}", components);

        var x = (index % 4 + 0.5f) / 4 - 0.5f;
        var y = (index / 4 + 0.5f) / 4 - 0.5f;

        go.transform.parent = transform;
        go.transform.localPosition = new Vector3(x, y, 0);
        go.transform.localScale = Vector3.one / 4;

        var mf = go.GetComponent<MeshFilter>();
        mf.sharedMesh = _mesh;

        var mr = go.GetComponent<MeshRenderer>();
        mr.sharedMaterial = _material;

        var receiver = go.GetComponent<NdiReceiver>();
        receiver.SetResources(_resources);
        receiver.ndiName = $"{_hostName} (Sender {index})";
        receiver.targetRenderer = mr;
        receiver.targetMaterialProperty = "_MainTex";

        return go;
    }
}
