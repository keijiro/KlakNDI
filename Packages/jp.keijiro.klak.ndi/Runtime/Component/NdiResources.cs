using UnityEngine;

namespace Klak.NDI
{
    public sealed class NdiResources : ScriptableObject
    {
        public ComputeShader encoderCompute;
        public ComputeShader decoderCompute;
    }
}
