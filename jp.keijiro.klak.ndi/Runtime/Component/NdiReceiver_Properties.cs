using UnityEngine;

namespace Klak.Ndi {

public sealed partial class NdiReceiver : MonoBehaviour
{
    #region NDI source settings

    [SerializeField] string _ndiName = null;
    string _ndiNameRuntime;

    public string ndiName
      { get => _ndiNameRuntime;
        set => SetNdiName(value); }

    void SetNdiName(string name)
    {
        if (_ndiNameRuntime == name) return;
        _ndiName = _ndiNameRuntime = name;
        Restart();
    }

    #endregion

    #region Target settings

    [SerializeField] RenderTexture _targetTexture = null;

    public RenderTexture targetTexture
      { get => _targetTexture;
        set => _targetTexture = value; }

    [SerializeField] Renderer _targetRenderer = null;

    public Renderer targetRenderer
      { get => _targetRenderer;
        set => _targetRenderer = value; }

    [SerializeField] string _targetMaterialProperty = null;

    public string targetMaterialProperty
      { get => _targetMaterialProperty;
        set => _targetMaterialProperty = value; }

    #endregion

    #region Runtime property

    public RenderTexture texture => _converter?.LastDecoderOutput;

    public string metadata { get; set; }

    private Vector2Int _resolution = new Vector2Int();
    private float _aspectRatio = 0;
    private int _frameRateD = 0;
    private int _frameRateN = 0;
    private long _timecode = 0;
    private long _timestamp = 0;

    // Pixel resolution
    public Vector2Int resolution { get => _resolution; }

    // picture aspect ratio (width/height)
    // This may be different to resolution.x / resolution.y if the pixels are not square
    public float aspectRatio { get => _aspectRatio; }

    public float frameRate
      { get
        {
          if (_frameRateD == 0) return 0;
          return _frameRateN / _frameRateD;
        }
      }

    // Frame Timecode in 100ns (trivially convertible to DateTime)
    public long timecode { get => _timecode; }
    // Timestamp when frame was submitted by the sender (100ns)
    public long timestamp { get => _timestamp; }

    public Interop.Recv internalRecvObject => _recv;

    #endregion

    #region Resources asset reference

    [SerializeField, HideInInspector] NdiResources _resources = null;

    public void SetResources(NdiResources resources)
      => _resources = resources;

    #endregion

    #region Editor change validation

    // Applies changes on the serialized fields to the runtime properties.
    // We use OnValidate on Editor, which also works as an initializer.
    // Player never call it, so we use Awake instead of it.

    #if UNITY_EDITOR
    void OnValidate()
    #else
    void Awake()
    #endif
      => ndiName = _ndiName;

    #endregion
}

} // namespace Klak.Ndi
