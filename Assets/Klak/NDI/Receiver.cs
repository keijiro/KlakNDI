using UnityEngine;
using System;

namespace Klak.Ndi
{
    public class Receiver : MonoBehaviour
    {
        [SerializeField] Renderer _targetRenderer;
        MaterialPropertyBlock _targetOverrides;

        [SerializeField, HideInInspector] Shader _shader;
        Material _material;
        RenderTexture _tempRT;

        IntPtr _instance;

        void Start()
        {
            _targetOverrides = new MaterialPropertyBlock();
            _material = new Material(_shader);
        }

        void OnDestroy()
        {
            if (_instance != IntPtr.Zero)
                PluginEntry.NDI_DestroyReceiver(_instance);

            if (_tempRT != null) RenderTexture.ReleaseTemporary(_tempRT);
            Destroy(_material);
        }

        void Update()
        {
            if (_targetRenderer == null) return;

            if (_instance == IntPtr.Zero)
            {
                _instance = PluginEntry.NDI_CreateReceiver();
                if (_instance == IntPtr.Zero) return;
            }

            var ready = PluginEntry.NDI_ReceiveFrame(_instance);
            if (!ready) return;

            var width = PluginEntry.NDI_GetFrameWidth(_instance);
            var height = PluginEntry.NDI_GetFrameHeight(_instance);
            var data = PluginEntry.NDI_GetFrameData(_instance);

            var texture = new Texture2D(width / 2, height, TextureFormat.RGBA32, false, true);
            texture.filterMode = FilterMode.Point;
            texture.LoadRawTextureData(data, width * 2 * height);
            texture.Apply();

            PluginEntry.NDI_FreeFrame(_instance);

            if (_tempRT != null) RenderTexture.ReleaseTemporary(_tempRT);
            _tempRT = RenderTexture.GetTemporary(width, height, 0, RenderTextureFormat.ARGB32);

            Graphics.Blit(texture, _tempRT, _material, 0);
            Destroy(texture);

            _targetRenderer.GetPropertyBlock(_targetOverrides);
            _targetOverrides.SetTexture("_MainTex", _tempRT);
            _targetRenderer.SetPropertyBlock(_targetOverrides);
        }
    }
}
