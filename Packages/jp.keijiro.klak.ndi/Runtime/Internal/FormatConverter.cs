using UnityEngine;
using UnityEngine.Rendering;
using IntPtr = System.IntPtr;

namespace Klak.NDI {

sealed class FormatConverter : System.IDisposable
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

        if (_decoderOutput != null)
        {
            if (Application.isPlaying)
                Object.Destroy(_decoderOutput);
            else
                Object.DestroyImmediate(_decoderOutput);
            _decoderOutput = null;
        }
    }

    #endregion

    #region Encoder implementation

    ComputeBuffer _encoderOutput;

    // Immediate mode version
    public ComputeBuffer Encode(Texture source, bool enableAlpha)
    {
        var width = source.width;
        var height = source.height;
        var dataCount = Util.FrameDataCount(width, height, enableAlpha);

        // Reallocate the output buffer when the output size was changed.
        if (_encoderOutput != null && _encoderOutput.count != dataCount)
            ReleaseBuffers();

        // Output buffer allocation
        if (_encoderOutput == null)
            _encoderOutput = new ComputeBuffer(dataCount, 4);

        // Compute thread dispatching
        var compute = _resources.encoderCompute;
        var pass = enableAlpha ? 1 : 0;
        compute.SetTexture(pass, "Source", source);
        compute.SetBuffer(pass, "Destination", _encoderOutput);
        compute.Dispatch(pass, width / 16, height / 8, 1);

        return _encoderOutput;
    }

    // Command buffer version
    public ComputeBuffer Encode
      (CommandBuffer cb, RenderTargetIdentifier source,
       int width, int height, bool enableAlpha)
    {
        var dataCount = Util.FrameDataCount(width, height, enableAlpha);

        // Reallocate the output buffer when the output size was changed.
        if (_encoderOutput != null && _encoderOutput.count != dataCount)
            ReleaseBuffers();

        // Output buffer allocation
        if (_encoderOutput == null)
            _encoderOutput = new ComputeBuffer(dataCount, 4);

        // Compute thread dispatching
        var compute = _resources.encoderCompute;
        var pass = enableAlpha ? 1 : 0;
        cb.SetComputeTextureParam(compute, pass, "Source", source);
        cb.SetComputeBufferParam(compute, pass, "Destination", _encoderOutput);
        cb.DispatchCompute(compute, pass, width / 16, height / 8, 1);

        return _encoderOutput;
    }

    #endregion

    #region Decoder implementation

    ComputeBuffer _decoderInput;
    ComputeDataSetter _decoderSetter;
    RenderTexture _decoderOutput;

    public RenderTexture LastDecoderOutput => _decoderOutput;

    public RenderTexture
      Decode(int width, int height, bool enableAlpha, IntPtr data)
    {
        var dataCount = Util.FrameDataCount(width, height, enableAlpha);

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
        {
            _decoderInput = new ComputeBuffer(dataCount, 4);
            _decoderSetter = new ComputeDataSetter(_decoderInput);
        }

        // Output buffer allocation
        if (_decoderOutput == null)
        {
            _decoderOutput = new RenderTexture(width, height, 0);
            _decoderOutput.enableRandomWrite = true;
            _decoderOutput.Create();
        }

        // Input buffer update
        _decoderSetter.SetData(data, dataCount, 4);

        // Decoder compute dispatching
        var compute = _resources.decoderCompute;
        var pass = enableAlpha ? 1 : 0;
        compute.SetBuffer(pass, "Source", _decoderInput);
        compute.SetTexture(pass, "Destination", _decoderOutput);
        compute.Dispatch(pass, width / 16, height / 8, 1);

        return _decoderOutput;
    }

    #endregion
}

}
