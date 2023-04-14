using Klak.Ndi.Interop;
using Unity.Collections.LowLevel.Unsafe;
using UnityEngine;
using IntPtr = System.IntPtr;
using Marshal = System.Runtime.InteropServices.Marshal;

namespace Klak.Ndi {

    [ExecuteInEditMode]
    public sealed partial class NdiReceiver : MonoBehaviour
    {
        #region Receiver objects

        Interop.Recv _recv;
        FormatConverter _converter;
        MaterialPropertyBlock _override;

        void PrepareReceiverObjects()
        {
            if (_recv == null) _recv = RecvHelper.TryCreateRecv(NdiName);
            if (_converter == null) _converter = new FormatConverter(_resources);
            if (_override == null) _override = new MaterialPropertyBlock();
        }

        void ReleaseReceiverObjects()
        {
            _recv?.Dispose();
            _recv = null;

            _converter?.Dispose();
            _converter = null;

            // We don't dispose _override because it's reusable.
        }

        #endregion

        #region Receiver implementation

        private void DisplayTexture(RenderTexture rt)
        {
            // Material property override
            if (targetRenderer != null)
            {
                targetRenderer.GetPropertyBlock(_override);
                _override.SetTexture(targetMaterialProperty, rt);
                targetRenderer.SetPropertyBlock(_override);
            }

            // External texture update
            if (targetTexture != null) Graphics.Blit(rt, targetTexture);
        }

        void ReceiveVideoTask()
        {
            // Video frame capturing
            VideoFrame? videoFrame = RecvHelper.TryCaptureVideoFrame(_recv);

            if (videoFrame == null) return;// && audioFrame == null) return;
            VideoFrame frame = (VideoFrame)videoFrame;

            // Pixel format conversion
            RenderTexture rt = _converter.Decode(frame.Width, frame.Height, Util.HasAlpha(frame.FourCC), frame.Data);

            // Metadata retrieval
            if (frame.Metadata != IntPtr.Zero)
                metadata = Marshal.PtrToStringAnsi(frame.Metadata);
            else
                metadata = null;

            // Video frame release
            _recv.FreeVideoFrame(frame);
            DisplayTexture(rt);

        }

        void ReceiveAudioTask()
        {
            AudioFrame? audioFrame = RecvHelper.TryCaptureAudioFrame(_recv);
            if (audioFrame == null) return;
            AudioFrame frame = (AudioFrame)audioFrame;

            
        }

        #endregion

        #region Component state controller

        internal void Restart() => ReleaseReceiverObjects();

        #endregion

        #region MonoBehaviour implementation

        void OnDisable() => ReleaseReceiverObjects();

        void Update()
        {
            PrepareReceiverObjects();
            if (_recv == null) return;

            ReceiveVideoTask();
            ReceiveAudioTask();

        }

        #endregion
    }

} // namespace Klak.Ndi
