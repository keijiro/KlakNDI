// KlakNDI - NDI plugin for Unity
// https://github.com/keijiro/KlakNDI

using Unity.Collections.LowLevel.Unsafe;
using UnityEngine;
using UnityEngine.Experimental.Rendering;
using System;
using System.Collections;
using System.Collections.Generic;

#if UNITY_EDITOR
using UnityEditor; // Needed to use delayCall
#endif

namespace Klak.Ndi
{
    [ExecuteInEditMode]
    public class NdiSender : MonoBehaviour
    {
        #region Source texture

        [SerializeField] RenderTexture _sourceTexture;

        public RenderTexture sourceTexture {
            get { return _sourceTexture; }
            set { _sourceTexture = value; }
        }

        #endregion

        #region Format option

        [SerializeField] bool _alphaSupport;

        public bool alphaSupport {
            get { return _alphaSupport; }
            set { _alphaSupport = value; }
        }

        #endregion

        #region Conversion shader

        [SerializeField, HideInInspector] Shader _shader;
        Material _material;
        RenderTexture _converted;

        #endregion

        #region Frame readback queue

        struct Frame
        {
            public int width, height;
            public bool alpha;
            public AsyncGPUReadbackRequest readback;
        }

        Queue<Frame> _frameQueue = new Queue<Frame>(4);

        void QueueFrame(RenderTexture source)
        {
            if (_frameQueue.Count > 3)
            {
                Debug.LogWarning("Too many GPU readback requests.");
                return;
            }

            // Return the old render texture to the pool.
            if (_converted != null) RenderTexture.ReleaseTemporary(_converted);

            // Allocate a new render texture.
            _converted = RenderTexture.GetTemporary(
                source.width / 2, (_alphaSupport ? 3 : 2) * source.height / 2, 0,
                RenderTextureFormat.ARGB32, RenderTextureReadWrite.Linear
            );

            // Lazy initialization of the conversion shader.
            if (_material == null)
            {
                _material = new Material(_shader);
                _material.hideFlags = HideFlags.DontSave;
            }

            // Apply the conversion shader.
            Graphics.Blit(source, _converted, _material, _alphaSupport ? 1 : 0);

            // Request readback.
            _frameQueue.Enqueue(new Frame{
                width = source.width, height = source.height,
                alpha = _alphaSupport,
                readback = AsyncGPUReadback.Request(_converted)
            });
        }

        void ProcessQueue()
        {
            while (_frameQueue.Count > 0)
            {
                var frame = _frameQueue.Peek();

                // Skip error frames.
                if (frame.readback.hasError)
                {
                    Debug.LogWarning("GPU readback error was detected.");
                    _frameQueue.Dequeue();
                    continue;
                }

                // Edit mode: Wait for readback completion every frame.
                if (!Application.isPlaying) frame.readback.WaitForCompletion();

                // Break when found a frame that hasn't been read back yet.
                if (!frame.readback.done) break;

                // Okay, we're going to send this frame.

                // Lazy initialization of the plugin sender instance.
                if (_plugin == IntPtr.Zero) _plugin = PluginEntry.NDI_CreateSender(gameObject.name);

                // Feed the frame data to the sender.
                // It starts encoding/sending the frame asynchronously.
                var array = frame.readback.GetData<Byte>();
                unsafe {
                    PluginEntry.NDI_SendFrame(
                        _plugin, (IntPtr)array.GetUnsafeReadOnlyPtr(),
                        frame.width, frame.height, frame.alpha ? FourCC.UYVA : FourCC.UYVY
                    );
                }

                // Edit mode: Actially we don't like to do things in an async
                // fashion, so let's immediately synchronize with the sender.
                if (!Application.isPlaying) PluginEntry.NDI_SyncSender(_plugin);

                // Done. Remove the frame from the queue.
                _frameQueue.Dequeue();
            }
        }

        #if UNITY_EDITOR

        void DelayedUpdate()
        {
            // Check if it's in the render texture mode.
            if (GetComponent<Camera>() != null || _sourceTexture == null) return;

            // Queue the current frame and immediately send it.
            QueueFrame(_sourceTexture);
            ProcessQueue();
        }

        #endif

        #endregion

        #region Misc variables

        IntPtr _plugin;
        bool _hasCamera;

        #endregion

        #region MonoBehaviour implementation

        IEnumerator Start()
        {
            _hasCamera = (GetComponent<Camera>() != null);

            // Only run the sync coroutine in the play mode.
            if (!Application.isPlaying) yield break;

            // Synchronize with the async sender at the end of every frame.
            for (var wait = new WaitForEndOfFrame();;)
            {
                yield return wait;
                if (enabled && _plugin != IntPtr.Zero) PluginEntry.NDI_SyncSender(_plugin);
            }
        }

        void OnDestroy()
        {
            if (Application.isPlaying)
                Destroy(_material);
            else
                DestroyImmediate(_material);
        }

        void OnDisable()
        {
            if (_converted != null)
            {
                RenderTexture.ReleaseTemporary(_converted);
                _converted = null;
            }

            if (_plugin != IntPtr.Zero)
            {
                PluginEntry.NDI_DestroySender(_plugin);
                _plugin = IntPtr.Zero;
            }
        }

        void Update()
        {
        #if UNITY_EDITOR
            // Edit mode: Use delayed update.
            if (!Application.isPlaying)
            {
                EditorApplication.delayCall += DelayedUpdate;
                return;
            }
        #endif

            ProcessQueue();

            // Request frame readback when in the render texture mode.
            if (!_hasCamera && _sourceTexture != null) QueueFrame(_sourceTexture);
        }

        void OnRenderImage(RenderTexture source, RenderTexture destination)
        {
            if (source != null) QueueFrame(source);

            Graphics.Blit(source, destination);

            // Edit mode: Process this frame immediately.
            if (!Application.isPlaying) ProcessQueue();
        }

        #endregion
    }
}
