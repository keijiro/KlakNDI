// KlakNDI - NDI plugin for Unity
// https://github.com/keijiro/KlakNDI

using UnityEngine;
using UnityEngine.Rendering;
using System;

namespace Klak.Ndi
{
    public class NdiReceiver : MonoBehaviour
    {
        #region Source settings

        [SerializeField] string _nameFilter;

        public string nameFilter {
            get { return _nameFilter; }
            set { _nameFilter = value; }
        }

        #endregion

        #region Target settings

        [SerializeField] RenderTexture _targetTexture;

        public RenderTexture targetTexture {
            get { return _targetTexture; }
            set { _targetTexture = value; }
        }

        [SerializeField] Renderer _targetRenderer;

        public Renderer targetRenderer {
            get { return _targetRenderer; }
            set { _targetRenderer = value; }
        }

        [SerializeField] string _targetMaterialProperty;

        public string targetMaterialProperty {
            get { return _targetMaterialProperty; }
            set { targetMaterialProperty = value; }
        }

        #endregion

        #region Public properties

        public Texture receivedTexture {
            get { return _converted != null ? _converted : _targetTexture; }
        }

        #endregion

        #region Conversion shader

        [SerializeField, HideInInspector] Shader _shader;
        Material _material;
        RenderTexture _converted;

        #endregion

        #region Private members

        CommandBuffer _commandBuffer;
        Texture2D _sourceTexture;
        MaterialPropertyBlock _propertyBlock;
        IntPtr _plugin;

        #endregion

        #region MonoBehaviour implementation

        void Start()
        {
            _material = new Material(_shader);
            _commandBuffer = new CommandBuffer();
            _sourceTexture = new Texture2D(8, 8); // placeholder texture
            _propertyBlock = new MaterialPropertyBlock();
        }

        void OnDestroy()
        {
            Destroy(_material);
            _commandBuffer.Dispose();
            Destroy(_sourceTexture);
            if (_converted != null) RenderTexture.ReleaseTemporary(_converted);
            if (_plugin != IntPtr.Zero) PluginEntry.NDI_DestroyReceiver(_plugin);
        }

        void Update()
        {
            // Plugin lazy initialization
            if (_plugin == IntPtr.Zero)
            {
                _plugin = PluginEntry.NDI_TryOpenSourceNamedLike(_nameFilter);
                if (_plugin == IntPtr.Zero) return;
            }

            // Invoke the texture update callback in the plugin.
            _commandBuffer.IssuePluginCustomTextureUpdate(
                PluginEntry.NDI_GetTextureUpdateCallback(),
                _sourceTexture,
                PluginEntry.NDI_GetReceiverID(_plugin)
            );
            Graphics.ExecuteCommandBuffer(_commandBuffer);
            _commandBuffer.Clear();

            // Check the frame dimensions.
            var width = PluginEntry.NDI_GetFrameWidth(_plugin);
            var height = PluginEntry.NDI_GetFrameHeight(_plugin);
            if (width == 0 || height == 0) return; // not yet ready

            // Renew the texture when the dimensions are changed.
            if (_sourceTexture.width != width / 2 || _sourceTexture.height != height)
            {
                Destroy(_sourceTexture);
                _sourceTexture = new Texture2D(width / 2, height, TextureFormat.RGBA32, false, true);
                _sourceTexture.filterMode = FilterMode.Point;
            }

            // Update external objects.
            if (_converted != null) RenderTexture.ReleaseTemporary(_converted);

            if (_targetTexture != null)
            {
                Graphics.Blit(_sourceTexture, _targetTexture, _material, 0);
            }
            else
            {
                _converted = RenderTexture.GetTemporary(width, height, 0, RenderTextureFormat.ARGB32);
                Graphics.Blit(_sourceTexture, _converted, _material, 0);
            }

            if (_targetRenderer != null)
            {
                _targetRenderer.GetPropertyBlock(_propertyBlock);
                _propertyBlock.SetTexture(_targetMaterialProperty, receivedTexture);
                _targetRenderer.SetPropertyBlock(_propertyBlock);
            }
        }

        #endregion
    }
}
