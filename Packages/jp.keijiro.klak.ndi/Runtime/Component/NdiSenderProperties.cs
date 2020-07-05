using UnityEngine;

namespace Klak.NDI {

public enum CaptureMethod { GameView, Camera, Texture }

public sealed partial class NdiSender : MonoBehaviour
{
    #region NDI source settings

    [SerializeField] string _ndiName = "NDI Sender";

    public string ndiName
      { get => _ndiName;
        set { _ndiName = value; Restart(); } }

    [SerializeField] bool _enableAlpha = false;

    public bool enableAlpha
      { get => _enableAlpha;
        set => _enableAlpha = value; }

    #endregion

    #region Capture target settings

    [SerializeField] CaptureMethod _captureMethod = CaptureMethod.GameView;

    public CaptureMethod captureMethod
      { get => _captureMethod;
        set { _captureMethod = value; ResetState(); } }

    [SerializeField] Camera _sourceCamera = null;

    public Camera sourceCamera
      { get => _sourceCamera;
        set { _sourceCamera = value; ResetState(); } }

    [SerializeField] Texture _sourceTexture = null;

    public Texture sourceTexture
      { get => _sourceTexture;
        set => _sourceTexture = value; }

    #endregion

    #region Resources asset reference

    [SerializeField, HideInInspector] NdiResources _resources = null;

    public void SetResources(NdiResources resources)
      => _resources = resources;

    #endregion
}

}
