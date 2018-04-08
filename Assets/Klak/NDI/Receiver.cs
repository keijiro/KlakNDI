using UnityEngine;
using UnityEngine.Rendering;
using System;

namespace Klak.Ndi
{
    public class Receiver : MonoBehaviour
    {
        #region Editable properties

        [SerializeField] string _nameFilter;

        #endregion

        #region Renderer override

        [SerializeField] Renderer _targetRenderer;
        MaterialPropertyBlock _targetOverrides;

        #endregion

        #region Conversion shader

        [SerializeField, HideInInspector] Shader _shader;
        Material _material;

        #endregion

        #region Texture objects

        CommandBuffer _commandBuffer;
        Texture2D _sourceTexture;
        RenderTexture _decodedTexture;

        #endregion

        #region Native plugin instance

        IntPtr _instance;

        #endregion

        #region MonoBehaviour implementation

        void Start()
        {
            _targetOverrides = new MaterialPropertyBlock();
            _material = new Material(_shader);
            _commandBuffer = new CommandBuffer();
            _sourceTexture = new Texture2D(8, 8); // placeholder texture
        }

        void OnDestroy()
        {
            Destroy(_material);
            _commandBuffer.Dispose();
            Destroy(_sourceTexture);
            if (_decodedTexture != null) RenderTexture.ReleaseTemporary(_decodedTexture);
            if (_instance != IntPtr.Zero) PluginEntry.NDI_DestroyReceiver(_instance);
        }

        void Update()
        {
            // Plugin lazy initialization
            if (_instance == IntPtr.Zero)
            {
                _instance = PluginEntry.NDI_TryCreateReceiverWithClause(_nameFilter);
                if (_instance == IntPtr.Zero) return;
            }

            // Texture update command
            _commandBuffer.IssuePluginCustomTextureUpdate(
                PluginEntry.NDI_GetTextureUpdateFunction(),
                _sourceTexture,
                PluginEntry.NDI_GetReceiverID(_instance)
            );
            Graphics.ExecuteCommandBuffer(_commandBuffer);
            _commandBuffer.Clear();

            // Replace the texture object if the dimensions are changed.
            var width = PluginEntry.NDI_GetFrameWidth(_instance);
            var height = PluginEntry.NDI_GetFrameHeight(_instance);
            if (width == 0 || height == 0) return; // not yet ready

            if (_sourceTexture.width != width / 2 || _sourceTexture.height != height)
            {
                Destroy(_sourceTexture);
                _sourceTexture = new Texture2D(width / 2, height, TextureFormat.RGBA32, false, true);
                _sourceTexture.filterMode = FilterMode.Point;
            }

            // Apply the conversion shader.
            if (_decodedTexture != null) RenderTexture.ReleaseTemporary(_decodedTexture);
            _decodedTexture = RenderTexture.GetTemporary(width, height, 0, RenderTextureFormat.ARGB32);
            Graphics.Blit(_sourceTexture, _decodedTexture, _material, 0);

            // Target override
            if (_targetRenderer != null)
            {
                _targetRenderer.GetPropertyBlock(_targetOverrides);
                _targetOverrides.SetTexture("_MainTex", _decodedTexture);
                _targetRenderer.SetPropertyBlock(_targetOverrides);
            }
        }

        #endregion
    }
}
