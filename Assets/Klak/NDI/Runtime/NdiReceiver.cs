// KlakNDI - NDI plugin for Unity
// https://github.com/keijiro/KlakNDI

using UnityEngine;
using UnityEngine.Rendering;
using System;

namespace Klak.Ndi
{
    [ExecuteInEditMode]
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

        static IntPtr _callback;
        IntPtr _plugin;

        CommandBuffer _commandBuffer;
        Texture2D _sourceTexture;
        MaterialPropertyBlock _propertyBlock;

        void DestroyAsset(UnityEngine.Object o)
        {
            if (Application.isPlaying)
                Destroy(o);
            else
                DestroyImmediate(o);
        }

        #endregion

        #region MonoBehaviour implementation

        void OnDisable()
        {
            if (_commandBuffer != null)
            {
                _commandBuffer.Dispose();
                _commandBuffer = null;
            }

            if (_plugin != IntPtr.Zero)
            {
                PluginEntry.NDI_DestroyReceiver(_plugin);
                _plugin = IntPtr.Zero;
            }
        }

        void OnDestroy()
        {
            if (_material != null)
            {
                DestroyAsset(_material);
                _material = null;
            }

            if (_converted != null)
            {
                RenderTexture.ReleaseTemporary(_converted);
                _converted = null;
            }

            if (_sourceTexture != null)
            {
                DestroyAsset(_sourceTexture);
                _sourceTexture = null;
            }
        }

        void Update()
        {
            // Lazy initialization
            if (_material == null)
            {
                _material = new Material(_shader);
                _material.hideFlags = HideFlags.DontSave;
            }

            if (_callback == IntPtr.Zero)
                _callback = PluginEntry.NDI_GetTextureUpdateCallback();

            if (_plugin == IntPtr.Zero)
            {
                _plugin = PluginEntry.NDI_TryOpenSourceNamedLike(_nameFilter);
                if (_plugin == IntPtr.Zero) return;
            }

            if (_commandBuffer == null)
                _commandBuffer = new CommandBuffer();

            if (_sourceTexture == null)
            {
                _sourceTexture = new Texture2D(8, 8); // placeholder
                _sourceTexture.hideFlags = HideFlags.DontSave;
            }

            if (_propertyBlock == null)
                _propertyBlock = new MaterialPropertyBlock();

            // Invoke the texture update callback in the plugin.
            _commandBuffer.IssuePluginCustomTextureUpdate(
                _callback, _sourceTexture, PluginEntry.NDI_GetReceiverID(_plugin)
            );
            Graphics.ExecuteCommandBuffer(_commandBuffer);
            _sourceTexture.IncrementUpdateCount();
            _commandBuffer.Clear();

            // Check the frame dimensions.
            var width = PluginEntry.NDI_GetFrameWidth(_plugin);
            var height = PluginEntry.NDI_GetFrameHeight(_plugin);
            if (width == 0 || height == 0) return; // not yet ready

            // Calculate the source data dimensions.
            var alpha = PluginEntry.NDI_GetFrameFourCC(_plugin) == FourCC.UYVA;
            var sw = width / 2;
            var sh = height * (alpha ? 3 : 2) / 2;

            // Renew the texture when the dimensions are changed.
            if (_sourceTexture.width != sw || _sourceTexture.height != sh)
            {
                DestroyAsset(_sourceTexture);
                _sourceTexture = new Texture2D(sw, sh, TextureFormat.RGBA32, false, true);
                _sourceTexture.hideFlags = HideFlags.DontSave;
                _sourceTexture.filterMode = FilterMode.Point;
            }

            // Update external objects.
            if (_converted != null) RenderTexture.ReleaseTemporary(_converted);

            if (_targetTexture != null)
            {
                Graphics.Blit(_sourceTexture, _targetTexture, _material, alpha ? 1 : 0);
                _targetTexture.IncrementUpdateCount();
            }
            else
            {
                _converted = RenderTexture.GetTemporary(width, height, 0, RenderTextureFormat.ARGB32);
                Graphics.Blit(_sourceTexture, _converted, _material, alpha ? 1 : 0);
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
