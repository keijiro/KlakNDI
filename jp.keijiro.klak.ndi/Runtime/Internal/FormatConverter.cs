using UnityEngine;
using UnityEngine.Rendering;
using IDisposable = System.IDisposable;
using IntPtr = System.IntPtr;

namespace Klak.Ndi {

//
// A format converter class wrapping GPU tasks and resources
//
// We use GPU (compute) to convert textures between the renderer-friendly raw
// formats and the NDI-friendly (chroma-subsampled) formats. This class wraps
// the cumbersome things and provides a simple API.
//
sealed class FormatConverter : IDisposable
{
    #region Common members

    NdiResources _resources;

    public FormatConverter(NdiResources resources) => _resources = resources;

    public void Dispose() => ReleaseBuffers();

    void ReleaseBuffers()
    {
        _encoderOutput?.Dispose();
        _encoderOutput = null;

        _decoderInput?.Dispose();
        _decoderInput = null;

        Util.Destroy(_decoderOutput);
        _decoderOutput = null;
    }

    void CheckDimensions(int width, int height)
    {
        if ((width & 0xf) != 0)
            WarnWrongSize($"Width ({width}) must be a multiple of 16.");

        if ((height & 0x7) != 0)
            WarnWrongSize($"Height ({height}) must be a multiple of 8.");
    }

    void WarnWrongSize(string text)
      => Debug.LogWarning("[KlakNDI] Unsupported frame size: " + text);

    #endregion

    #region Encoder implementation

    ComputeBuffer _encoderOutput;

    // Immediate mode version
    public ComputeBuffer Encode(Texture source, bool enableAlpha, bool vflip)
    {
        var width = source.width;
        var height = source.height;
        var dataCount = Util.FrameDataSize(width, height, enableAlpha) / 4;

        // Reallocate the output buffer when the output size was changed.
        if (_encoderOutput != null && _encoderOutput.count != dataCount)
            ReleaseBuffers();

        // Output buffer allocation
        if (_encoderOutput == null)
        {
            CheckDimensions(width, height);
            _encoderOutput = new ComputeBuffer(dataCount, 4);
        }

        // Compute thread dispatching
        var compute = _resources.encoderCompute;
        var pass = (enableAlpha ? 2 : 0) + (Util.InGammaMode ? 0 : 1);
        compute.SetFloat("VFlip", vflip ? 1 : 0);
        compute.SetTexture(pass, "Source", source);
        compute.SetBuffer(pass, "Destination", _encoderOutput);
        compute.Dispatch(pass, width / 16, height / 8, 1);

        return _encoderOutput;
    }

    // Command buffer version
    public ComputeBuffer Encode
      (CommandBuffer cb, RenderTargetIdentifier source,
       int width, int height, bool enableAlpha, bool vflip)
    {
        var dataCount = Util.FrameDataSize(width, height, enableAlpha) / 4;

        CheckDimensions(width, height);

        // Reallocate the output buffer when the output size was changed.
        if (_encoderOutput != null && _encoderOutput.count != dataCount)
            ReleaseBuffers();

        // Output buffer allocation
        if (_encoderOutput == null)
        {
            CheckDimensions(width, height);
            _encoderOutput = new ComputeBuffer(dataCount, 4);
        }

        // Compute thread dispatching
        var compute = _resources.encoderCompute;
        var pass = (enableAlpha ? 2 : 0) + (Util.InGammaMode ? 0 : 1);
        cb.SetComputeFloatParam(compute, "VFlip", vflip ? 1 : 0);
        cb.SetComputeTextureParam(compute, pass, "Source", source);
        cb.SetComputeBufferParam(compute, pass, "Destination", _encoderOutput);
        cb.DispatchCompute(compute, pass, width / 16, height / 8, 1);

        return _encoderOutput;
    }

    #endregion

    #region Decoder implementation

    ComputeBuffer _decoderInput;
    RenderTexture _decoderOutput;

    public RenderTexture LastDecoderOutput => _decoderOutput;

    public RenderTexture
      Decode(int width, int height, bool enableAlpha, IntPtr data)
    {
        var dataCount = Util.FrameDataSize(width, height, enableAlpha) / 4;

        CheckDimensions(width, height);

        // Reallocate the input buffer when the input size was changed.
        if (_decoderInput != null && _decoderInput.count != dataCount)
            ReleaseBuffers();

        // Reallocate the output buffer when the output size was changed.
        if (_decoderOutput != null &&
            (_decoderOutput.width != width ||
             _decoderOutput.height != height))
            ReleaseBuffers();

        // Input buffer allocation
        if (_decoderInput == null)
            _decoderInput = new ComputeBuffer(dataCount, 4);

        // Output buffer allocation
        if (_decoderOutput == null)
        {
            CheckDimensions(width, height);
        #if KLAK_NDI_ISSUE200_WORKAROUND
            _decoderOutput = new RenderTexture(width, height, 0, RenderTextureFormat.ARGBHalf);
        #else
            _decoderOutput = new RenderTexture(width, height, 0);
        #endif
            _decoderOutput.enableRandomWrite = true;
            _decoderOutput.Create();
        }

        // Input buffer update
        _decoderInput.SetData(data, dataCount, 4);

        // Kenel select
        var pass = (enableAlpha ? 2 : 0);

        // As far as we know, only Metal supports sRGB write to UAV, so we
        // use the linear-color kernels only on Metal
        if (!Util.InGammaMode && Util.UsingMetal) pass++;

        // Decoder compute dispatching
        var compute = _resources.decoderCompute;
        compute.SetBuffer(pass, "Source", _decoderInput);
        compute.SetTexture(pass, "Destination", _decoderOutput);
        compute.Dispatch(pass, width / 16, height / 8, 1);

        return _decoderOutput;
    }

    #endregion
}

} // namespace Klak.Ndi
