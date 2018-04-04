using System.Runtime.InteropServices;
using System.Threading;
using System;
using UnityEngine;
using Klak.Ndi;

public class Test : MonoBehaviour
{
    #region Receiver thread

    IntPtr _receiver;
    AutoResetEvent _receiverSync;
    bool _receiverStop;
    bool _frameReady;

    void ReceiverThreadEntry()
    {
        while (_receiver == IntPtr.Zero)
        {
            if (_receiverStop) return;
            _receiver = PluginEntry.NDI_CreateReceiver();
        }

        Debug.Log("start loop");

        while (true)
        {
            while (!_frameReady)
            {
                if (_receiverStop) return;
                _frameReady = PluginEntry.NDI_ReceiveFrame(_receiver);
            }

        Debug.Log("wait");

            _receiverSync.WaitOne();

            PluginEntry.NDI_FreeFrame(_receiver);
            _frameReady = false;
        }
    }

    #endregion

    Thread _receiverThread;
    Texture2D _texture;

    void Start()
    {
        PluginEntry.NDI_Initialize();

        _receiverSync = new AutoResetEvent(false);

        _receiverThread = new Thread(ReceiverThreadEntry);
        _receiverThread.Start();
    }

    void OnDestroy()
    {
        if (_receiverThread != null)
        {
            _receiverStop = true;
            _receiverSync.Set();
            _receiverThread.Join();
        }

        if (_receiver != System.IntPtr.Zero)
        {
            if (_frameReady)
                PluginEntry.NDI_FreeFrame(_receiver);

            PluginEntry.NDI_DestroyReceiver(_receiver);
        }

        if (_texture != null) Destroy(_texture);
    }

    void Update()
    {
        if (!_frameReady) return;

        Debug.Log("frame ready");

        var width = PluginEntry.NDI_GetFrameWidth(_receiver);
        var height = PluginEntry.NDI_GetFrameHeight(_receiver);
        var data = PluginEntry.NDI_GetFrameData(_receiver);

        if (_texture != null) Destroy(_texture);
        _texture = new Texture2D(width, height / 2, TextureFormat.RGBA32, false);
        _texture.LoadRawTextureData(data, width * height * 2);
        _texture.Apply();

        GetComponent<Renderer>().material.mainTexture = _texture;

        _receiverSync.Set();
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
