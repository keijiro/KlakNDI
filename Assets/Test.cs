using System.Runtime.InteropServices;
using System;
using UnityEngine;
using Klak.Ndi;

public class Test : MonoBehaviour
{
    IntPtr _receiver;
    Texture2D _texture;

    void Start()
    {
        PluginEntry.NDI_Initialize();
    }

    void OnDestroy()
    {
        if (_receiver != System.IntPtr.Zero)
            PluginEntry.NDI_DestroyReceiver(_receiver);

        if (_texture != null) Destroy(_texture);

        PluginEntry.NDI_Finalize();
    }

    void Update()
    {
        if (_receiver == System.IntPtr.Zero)
        {
            _receiver = PluginEntry.NDI_CreateReceiver();
            if (_receiver == System.IntPtr.Zero) return;
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

    /*
       System.IntPtr _sender;

       void Start()
       {
       PluginEntry.NDI_Initialize();
       _sender = PluginEntry.NDI_CreateSender("NDI Test");
       }

       void OnDestroy()
       {
       PluginEntry.NDI_DestroySender(_sender);
       }

       void OnRenderImage(RenderTexture source, RenderTexture destination)
       {
       var tempRT = RenderTexture.GetTemporary(source.width, source.height);
       Graphics.Blit(source, tempRT);

       var tempTex = new Texture2D(source.width, source.height, TextureFormat.RGBA32, false);
       tempTex.ReadPixels(new Rect(0, 0, source.width, source.height), 0, 0, false);
       tempTex.Apply();

       var data = tempTex.GetRawTextureData();
       var pin = GCHandle.Alloc(data, GCHandleType.Pinned);
       PluginEntry.NDI_SendFrame(_sender, pin.AddrOfPinnedObject(), source.width, source.height);
       pin.Free();

       Destroy(tempTex);
       RenderTexture.ReleaseTemporary(tempRT);

       Graphics.Blit(source, destination);
       }
     */
}
