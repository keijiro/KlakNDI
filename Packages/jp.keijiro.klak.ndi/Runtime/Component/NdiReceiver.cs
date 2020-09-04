using Unity.Collections.LowLevel.Unsafe;
using UnityEngine;
using Marshal = System.Runtime.InteropServices.Marshal;

namespace Klak.Ndi {

[ExecuteInEditMode]
public sealed partial class NdiReceiver : MonoBehaviour
{
    #region Internal objects

    Interop.Recv _recv;
    FormatConverter _converter;
    MaterialPropertyBlock _override;

    void PrepareInternalObjects()
    {
        if (_recv == null) _recv = RecvHelper.TryCreateRecv(_ndiName);
        if (_converter == null) _converter = new FormatConverter(_resources);
        if (_override == null) _override = new MaterialPropertyBlock();
    }

    void ReleaseInternalObjects()
    {
        _recv?.Dispose();
        _recv = null;

        _converter?.Dispose();
        _converter = null;
    }

    #endregion

    #region Receiver implementation

    RenderTexture TryReceiveFrame()
    {
        PrepareInternalObjects();

        // Do nothing if the recv object is not ready.
        if (_recv == null) return null;

        // Try getting a video frame.
        var frameOrNull = RecvHelper.TryCaptureVideoFrame(_recv);
        if (frameOrNull == null) return null;
        var frame = (Interop.VideoFrame)frameOrNull;

        // Pixel format conversion
        var rt = _converter.Decode
          (frame.Width, frame.Height,
           Util.CheckAlpha(frame.FourCC), frame.Data);

        // Copy the metadata if any.
        if (frame.Metadata != System.IntPtr.Zero)
            metadata = Marshal.PtrToStringAnsi(frame.Metadata);
        else
            metadata = null;

        // Free the frame up.
        _recv.FreeVideoFrame(frame);

        return rt;
    }

    #endregion

    #region Component state controller

    internal void Restart() => ReleaseInternalObjects();

    #endregion

    #region MonoBehaviour implementation

    void OnDisable() => ReleaseInternalObjects();

    void Update()
    {
        var rt = TryReceiveFrame();
        if (rt == null) return;

        // Material property override
        if (_targetRenderer != null)
        {
            _targetRenderer.GetPropertyBlock(_override);
            _override.SetTexture(_targetMaterialProperty, rt);
            _targetRenderer.SetPropertyBlock(_override);
        }

        // External texture update
        if (_targetTexture != null)
            Graphics.Blit(rt, _targetTexture);
    }

    #endregion
}

}
