using System.Runtime.InteropServices;
using UnityEngine;
using System;

namespace Klak.Ndi
{
    public class Sender : MonoBehaviour
    {
       IntPtr _sender;

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
    }
}
