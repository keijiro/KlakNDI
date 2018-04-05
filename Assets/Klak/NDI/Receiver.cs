using UnityEngine;
using System;

namespace Klak.Ndi
{
    public class Receiver : MonoBehaviour
    {
        IntPtr _receiver;

        Texture2D _texture;

        void Start()
        {
            PluginEntry.NDI_Initialize();
        }

        void OnDestroy()
        {
            if (_receiver != IntPtr.Zero)
                PluginEntry.NDI_DestroyReceiver(_receiver);

            if (_texture != null) Destroy(_texture);
        }

        void Update()
        {
            if (_receiver == IntPtr.Zero)
            {
                _receiver = PluginEntry.NDI_CreateReceiver();
                if (_receiver == IntPtr.Zero) return;
            }

            var ready = PluginEntry.NDI_ReceiveFrame(_receiver);
            if (!ready) return;

            var width = PluginEntry.NDI_GetFrameWidth(_receiver);
            var height = PluginEntry.NDI_GetFrameHeight(_receiver);
            var data = PluginEntry.NDI_GetFrameData(_receiver);

            if (_texture != null) Destroy(_texture);
            _texture = new Texture2D(width, height / 2, TextureFormat.RGBA32, false);
            _texture.LoadRawTextureData(data, width * height * 2);
            _texture.Apply();

            GetComponent<Renderer>().material.mainTexture = _texture;

            PluginEntry.NDI_FreeFrame(_receiver);
        }
    }
}
