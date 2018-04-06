using Unity.Collections;
using Unity.Collections.LowLevel.Unsafe;
using UnityEngine;
using UnityEngine.Experimental.Rendering;
using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;

namespace Klak.Ndi
{
    public class Sender : MonoBehaviour
    {
        IntPtr _instance;

        struct Frame
        {
            public int width, height;
            public AsyncGPUReadbackRequest readback;
        }

        Queue<Frame> _frameQueue;

        void Start()
        {
            _instance = PluginEntry.NDI_CreateSender(gameObject.name);
            _frameQueue = new Queue<Frame>(4);
        }

        void OnDestroy()
        {
            PluginEntry.NDI_DestroySender(_instance);
        }

        void Update()
        {
            while (_frameQueue.Count > 0)
            {
                var frame = _frameQueue.Peek();

                if (frame.readback.hasError)
                {
                    Debug.LogWarning("GPU readback error was detected.");
                    _frameQueue.Dequeue();
                }
                else if (frame.readback.done)
                {
                    var array = frame.readback.GetData<Byte>();
                    unsafe {
                        PluginEntry.NDI_SendFrame(
                            _instance, (IntPtr)array.GetUnsafeReadOnlyPtr(),
                            frame.width, frame.height
                        );
                    }
                    _frameQueue.Dequeue();
                }
                else
                {
                    break;
                }
            }
        }

        void OnRenderImage(RenderTexture source, RenderTexture destination)
        {
            if (_frameQueue.Count < 4)
            {
                _frameQueue.Enqueue(new Frame{
                    width = source.width, height = source.height,
                    readback = AsyncGPUReadback.Request(source)
                });
            }
            else
            {
                Debug.LogWarning("Too many GPU readback requests.");
            }

            Graphics.Blit(source, destination);
        }
    }
}
