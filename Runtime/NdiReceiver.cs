// KlakNDI - NDI plugin for Unity
// https://github.com/keijiro/KlakNDI

using UnityEngine;

namespace Klak.Ndi
{
    [ExecuteInEditMode]
    [AddComponentMenu("Klak/NDI/NDI Receiver")]
    public sealed class NdiReceiver : MonoBehaviour
    {
        #region Source settings

        [SerializeField] string _sourceName;

        public string sourceName {
            get { return _sourceName; }
            set {
                if (_sourceName == value) return;
                _sourceName = value;
                RequestReconnect();
            }
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

        [SerializeField] string _targetMaterialProperty = null;

        public string targetMaterialProperty {
            get { return _targetMaterialProperty; }
            set { _targetMaterialProperty = value; }
        }

        #endregion

        #region Runtime properties

        RenderTexture _receivedTexture;

        public Texture receivedTexture {
            get { return _targetTexture != null ? _targetTexture : _receivedTexture; }
        }

        #endregion

        #region Private members

        static System.IntPtr _callback;

        System.IntPtr _plugin;
        Texture2D _sourceTexture;
        Material _blitMaterial;
        MaterialPropertyBlock _propertyBlock;

        #endregion

        #region Internal members

        internal void RequestReconnect()
        {
            OnDisable();
        }

        #endregion

        #region MonoBehaviour implementation

        void OnDisable()
        {
            if (_plugin != System.IntPtr.Zero)
            {
                PluginEntry.DestroyReceiver(_plugin);
                _plugin = System.IntPtr.Zero;
            }
        }

        void OnDestroy()
        {
            Util.Destroy(_blitMaterial);
            Util.Destroy(_sourceTexture);
            Util.Destroy(_receivedTexture);
        }

        void Update()
        {
            if (!PluginEntry.IsAvailable) return;

            // Plugin lazy initialization
            if (_plugin == System.IntPtr.Zero)
            {
                _plugin = PluginEntry.CreateReceiver(_sourceName);
                if (_plugin == System.IntPtr.Zero) return; // No receiver support
            }

            // Texture update event invocation with lazy initialization
            if (_callback == System.IntPtr.Zero)
                _callback = PluginEntry.GetTextureUpdateCallback();

            if (_sourceTexture == null)
            {
                _sourceTexture = new Texture2D(8, 8); // Placeholder
                _sourceTexture.hideFlags = HideFlags.DontSave;
            }

            Util.IssueTextureUpdateEvent
                (_callback, _sourceTexture, PluginEntry.GetReceiverID(_plugin));

            // Texture information retrieval
            var width = PluginEntry.GetFrameWidth(_plugin);
            var height = PluginEntry.GetFrameHeight(_plugin);
            if (width == 0 || height == 0) return; // Not yet ready

            // Source data dimensions
            var alpha = PluginEntry.GetFrameFourCC(_plugin) == FourCC.UYVA;
            var sw = width / 2;
            var sh = height * (alpha ? 3 : 2) / 2;

            // Renew the textures when the dimensions are changed.
            if (_sourceTexture.width != sw || _sourceTexture.height != sh)
            {
                Util.Destroy(_sourceTexture);
                Util.Destroy(_receivedTexture);
                _sourceTexture = new Texture2D(sw, sh, TextureFormat.RGBA32, false, true);
                _sourceTexture.hideFlags = HideFlags.DontSave;
                _sourceTexture.filterMode = FilterMode.Point;
            }

            // Blit shader lazy initialization
            if (_blitMaterial == null)
            {
                _blitMaterial = new Material(Shader.Find("Hidden/KlakNDI/Receiver"));
                _blitMaterial.hideFlags = HideFlags.DontSave;
            }

            // Receiver texture lazy initialization
            if (_targetTexture == null && _receivedTexture == null)
            {
                _receivedTexture = new RenderTexture(width, height, 0);
                _receivedTexture.hideFlags = HideFlags.DontSave;
            }

            // Texture format conversion using the blit shader
            var receiver = _targetTexture != null ? _targetTexture : _receivedTexture;
            Graphics.Blit(_sourceTexture, receiver, _blitMaterial, alpha ? 1 : 0);
            receiver.IncrementUpdateCount();

            // Renderer override
            if (_targetRenderer != null)
            {
                // Material property block lazy initialization
                if (_propertyBlock == null)
                    _propertyBlock = new MaterialPropertyBlock();

                // Read-modify-write
                _targetRenderer.GetPropertyBlock(_propertyBlock);
                _propertyBlock.SetTexture(_targetMaterialProperty, receiver);
                _targetRenderer.SetPropertyBlock(_propertyBlock);
            }
        }

        #if UNITY_EDITOR

        // Invoke update on repaint in edit mode. This is needed to update the
        // shared texture without getting the object marked dirty.

        void OnRenderObject()
        {
            if (Application.isPlaying) return;

            // Graphic.Blit used in Update will change the current active RT,
            // so let us back it up and restore after Update.
            var activeRT = RenderTexture.active;
            Update();
            RenderTexture.active = activeRT;
        }

        #endif

        #endregion
    }
}
