using Unity.Collections.LowLevel.Unsafe;
using UnityEngine;
using UnityEngine.Rendering;

namespace Klak.Ndi {

[ExecuteInEditMode]
public sealed partial class NdiSender : MonoBehaviour
{
    #region Internal objects

    int _width, _height;
    Interop.Send _send;
    FormatConverter _converter;
    MetadataQueue _metadataQueue = new MetadataQueue();
    System.Action<AsyncGPUReadbackRequest> _onReadback;

    void PrepareInternalObjects()
    {
        if (_send == null)
            _send = _captureMethod == CaptureMethod.GameView ?
              SharedInstance.GameViewSend : Interop.Send.Create(_ndiName);
        if (_converter == null) _converter = new FormatConverter(_resources);
        if (_onReadback == null) _onReadback = OnReadback;
    }

    void ReleaseInternalObjects()
    {
        if (_send != null && !SharedInstance.IsGameViewSend(_send))
            _send.Dispose();
        _send = null;

        _converter?.Dispose();
        _converter = null;
    }

    #endregion

    #region Immediate capture methods

    ComputeBuffer CaptureImmediate()
    {
        PrepareInternalObjects();

        // Texture capture method
        // Simply convert the source texture and return it.
        if (_captureMethod == CaptureMethod.Texture)
        {
            if (_sourceTexture == null) return null;

            _width = _sourceTexture.width;
            _height = _sourceTexture.height;

            return _converter.Encode(_sourceTexture, _enableAlpha, true);
        }

        // Game View capture method
        // Capture the screen into a temporary RT, then convert it.
        if (_captureMethod == CaptureMethod.GameView)
        {
            _width = Screen.width;
            _height = Screen.height;

            var tempRT = RenderTexture.GetTemporary(_width, _height, 0);

            ScreenCapture.CaptureScreenshotIntoRenderTexture(tempRT);
            var converted = _converter.Encode(tempRT, _enableAlpha, false);

            RenderTexture.ReleaseTemporary(tempRT);
            return converted;
        }

        Debug.LogError("Wrong capture method.");
        return null;
    }

    // Capture coroutine: At the end of every frames, it captures the source
    // frame, convert it to the NDI frame format, then request GPU readback.
    System.Collections.IEnumerator ImmediateCaptureCoroutine()
    {
        for (var eof = new WaitForEndOfFrame(); true;)
        {
            yield return eof;

            var converted = CaptureImmediate();
            if (converted == null) continue;

            AsyncGPUReadback.Request(converted, _onReadback);
            _metadataQueue.Enqueue(metadata);
        }
    }

    #endregion

    #region SRP camera capture callback

    void OnCameraCapture(RenderTargetIdentifier source, CommandBuffer cb)
    {
        // NOTE: In some corner cases, this callback is called after object
        // destruction. To avoid these cases, we check the _attachedCamera
        // value and return if it's null. See ResetState() for details.
        if (_attachedCamera == null) return;

        PrepareInternalObjects();

        _width = _sourceCamera.pixelWidth;
        _height = _sourceCamera.pixelHeight;

        // Pixel format conversion
        var converted = _converter.Encode
          (cb, source, _width, _height, _enableAlpha, true);

        // GPU readback request
        cb.RequestAsyncReadback(converted, _onReadback);
        _metadataQueue.Enqueue(metadata);
    }

    #endregion

    #region GPU readback completion callback

    unsafe void OnReadback(AsyncGPUReadbackRequest request)
    {
        // Metadata retrieval
        using (var metadata = _metadataQueue.Dequeue())
        {
            // Ignore errors.
            if (request.hasError) return;

            // Ignore it if the NDI object has been already disposed.
            if (_send == null || _send.IsInvalid || _send.IsClosed) return;

            // Pixel format (depending on alpha mode)
            var fourcc = _enableAlpha ?
              Interop.FourCC.UYVA : Interop.FourCC.UYVY;

            // Readback data retrieval
            var data = request.GetData<byte>();
            var pdata = NativeArrayUnsafeUtility.GetUnsafeReadOnlyPtr(data);

            // Data size verification
            if (data.Length / sizeof(uint) !=
                Util.FrameDataCount(_width, _height, _enableAlpha)) return;

            // Frame data setup
            var frame = new Interop.VideoFrame
              { Width = _width, Height = _height, LineStride = _width * 2,
                FourCC = _enableAlpha ?
                  Interop.FourCC.UYVA : Interop.FourCC.UYVY,
                FrameFormat = Interop.FrameFormat.Progressive,
                Data = (System.IntPtr)pdata, Metadata = metadata };

            // Send via NDI
            _send.SendVideoAsync(frame);
        }
    }

    #endregion

    #region Component state controller

    Camera _attachedCamera;

    // Reset the component state without disposing the NDI send object.
    internal void ResetState(bool willBeActive)
    {
        // Disable the subcomponents.
        StopAllCoroutines();

        //
        // Remove the capture callback from the camera.
        //
        // NOTE: We're not able to remove the capture callback correcly when
        // the camera has been destroyed because we end up with getting a null
        // reference from _attachedCamera. To avoid causing issues in the
        // callback, we make sure that _attachedCamera has a null reference.
        //
        if (_attachedCamera != null)
        {
        #if KLAK_NDI_HAS_SRP
            CameraCaptureBridge.RemoveCaptureAction
              (_attachedCamera, OnCameraCapture);
        #endif
        }

        _attachedCamera = null;

        // The following blocks are to activate the subcomponents.
        // We can return here if willBeActive is false.
        if (!willBeActive) return;

        if (_captureMethod == CaptureMethod.Camera)
        {
            // Enable the camera capture callback.
            if (_sourceCamera != null)
            {
                _attachedCamera = _sourceCamera;
            #if KLAK_NDI_HAS_SRP
                CameraCaptureBridge.AddCaptureAction
                  (_attachedCamera, OnCameraCapture);
            #endif
            }
        }
        else
        {
            // Enable the immediate capture coroutine.
            StartCoroutine(ImmediateCaptureCoroutine());
        }
    }

    // Reset the component state and dispose the NDI send object.
    internal void Restart(bool willBeActivate)
    {
        ResetState(willBeActivate);
        ReleaseInternalObjects();
    }

    internal void ResetState() => ResetState(isActiveAndEnabled);
    internal void Restart() => Restart(isActiveAndEnabled);

    #endregion

    #region MonoBehaviour implementation

    void OnEnable() => ResetState();

    void OnDisable() => Restart(false);

    void OnDestroy() => Restart(false);

    #endregion
}

}
