#pragma once


#include "IUnityGraphics.h"

/*
    Low-level Native Plugin Rendering Extensions
    ============================================

    On top of the Low-level native plugin interface, Unity also supports low level rendering extensions that can receive callbacks when certain events happen.
    This is mostly used to implement and control low-level rendering in your plugin and enable it to work with Unity’s multithreaded rendering.

    Due to the low-level nature of this extension the plugin might need to be preloaded before the devices get created.
    Currently the convention is name-based namely the plugin name must be prefixed by “GfxPlugin”. Example: GfxPluginMyFancyNativePlugin.

    <code>
        // Native plugin code example

        enum PluginCustomCommands
        {
            kPluginCustomCommandDownscale = kUnityRenderingExtUserEventsStart,
            kPluginCustomCommandUpscale,

            // insert your own events here

            kPluginCustomCommandCount
        };

        static IUnityInterfaces* s_UnityInterfaces = NULL;

        extern "C" void UNITY_INTERFACE_EXPORT UNITY_INTERFACE_API
        UnityPluginLoad(IUnityInterfaces* unityInterfaces)
        {
            // initialization code here...
        }

        extern "C" void UNITY_INTERFACE_EXPORT UNITY_INTERFACE_API
        UnityRenderingExtEvent(UnityRenderingExtEventType event, void* data)
        {
            switch (event)
            {
                case kUnityRenderingExtEventBeforeDrawCall:
                    // do some stuff
                    break;
                case kUnityRenderingExtEventAfterDrawCall:
                    // undo some stuff
                    break;
                case kPluginCustomCommandDownscale:
                    // downscale some stuff
                    break;
                case kPluginCustomCommandUpscale:
                    // upscale some stuff
                    break;
            }
        }
    </code>
*/


//     These events will be propagated to all plugins that implement void UnityRenderingExtEvent(UnityRenderingExtEventType event, void* data);

enum UnityRenderingExtEventType
{
    kUnityRenderingExtEventSetStereoTarget,                 // issued during SetStereoTarget and carrying the current 'eye' index as parameter
    kUnityRenderingExtEventSetStereoEye,                    // issued during stereo rendering at the beginning of each eye's rendering loop. It carries the current 'eye' index as parameter
    kUnityRenderingExtEventStereoRenderingDone,             // issued after the rendering has finished
    kUnityRenderingExtEventBeforeDrawCall,                  // issued during BeforeDrawCall and carrying UnityRenderingExtBeforeDrawCallParams as parameter
    kUnityRenderingExtEventAfterDrawCall,                   // issued during AfterDrawCall. This event doesn't carry any parameters
    kUnityRenderingExtEventCustomGrab,                      // issued during GrabIntoRenderTexture since we can't simply copy the resources
                                                            //      when custom rendering is used - we need to let plugin handle this. It carries over
                                                            //      a UnityRenderingExtCustomBlitParams params = { X, source, dest, 0, 0 } ( X means it's irrelevant )
    kUnityRenderingExtEventCustomBlit,                      // issued by plugin to insert custom blits. It carries over UnityRenderingExtCustomBlitParams as param.
    kUnityRenderingExtEventUpdateTextureBegin,              // issued to update a texture. It carries over UnityRenderingExtTextureUpdateParams
    kUnityRenderingExtEventUpdateTextureEnd,                // issued to signal the plugin that the texture update has finished. It carries over the same UnityRenderingExtTextureUpdateParams as kUnityRenderingExtEventUpdateTextureBegin

    // keep this last
    kUnityRenderingExtEventCount,
    kUnityRenderingExtUserEventsStart = kUnityRenderingExtEventCount
};


enum UnityRenderingExtCustomBlitCommands
{
    kUnityRenderingExtCustomBlitVRFlush,                    // This event is mostly used in multi GPU configurations ( SLI, etc ) in order to allow the plugin to flush all GPU's targets

    // keep this last
    kUnityRenderingExtCustomBlitCount,
    kUnityRenderingExtUserCustomBlitStart = kUnityRenderingExtCustomBlitCount
};

/*
    This will be propagated to all plugins implementing UnityRenderingExtQuery.
*/
enum UnityRenderingExtQueryType
{
    kUnityRenderingExtQueryOverrideViewport             = 1 << 0,           // The plugin handles setting up the viewport rects. Unity will skip its internal SetViewport calls
    kUnityRenderingExtQueryOverrideScissor              = 1 << 1,           // The plugin handles setting up the scissor rects. Unity will skip its internal SetScissor calls
    kUnityRenderingExtQueryOverrideVROcclussionMesh     = 1 << 2,           // The plugin handles its own VR occlusion mesh rendering. Unity will skip rendering its internal VR occlusion mask
    kUnityRenderingExtQueryOverrideVRSinglePass         = 1 << 3,           // The plugin uses its own single pass stereo technique. Unity will only traverse and render the render node graph once.
                                                                            //      and it will clear the whole render target not just per-eye on demand.
    kUnityRenderingExtQueryKeepOriginalDoubleWideWidth_DEPRECATED  = 1 << 4,           // Instructs unity to keep the original double wide width. By default unity will try and have a power-of-two width for mip-mapping requirements.
    kUnityRenderingExtQueryRequestVRFlushCallback       = 1 << 5,           // Instructs unity to provide callbacks when the VR eye textures need flushing. Useful for multi GPU synchronization.
};


enum UnityRenderingExtTextureFormat
{
    kUnityRenderingExtFormatNone = 0, kUnityRenderingExtFormatFirst = kUnityRenderingExtFormatNone,

    // sRGB formats
    kUnityRenderingExtFormatR8_SRGB,
    kUnityRenderingExtFormatRG8_SRGB,
    kUnityRenderingExtFormatRGB8_SRGB,
    kUnityRenderingExtFormatRGBA8_SRGB,

    // 8 bit integer formats
    kUnityRenderingExtFormatR8_UNorm,
    kUnityRenderingExtFormatRG8_UNorm,
    kUnityRenderingExtFormatRGB8_UNorm,
    kUnityRenderingExtFormatRGBA8_UNorm,
    kUnityRenderingExtFormatR8_SNorm,
    kUnityRenderingExtFormatRG8_SNorm,
    kUnityRenderingExtFormatRGB8_SNorm,
    kUnityRenderingExtFormatRGBA8_SNorm,
    kUnityRenderingExtFormatR8_UInt,
    kUnityRenderingExtFormatRG8_UInt,
    kUnityRenderingExtFormatRGB8_UInt,
    kUnityRenderingExtFormatRGBA8_UInt,
    kUnityRenderingExtFormatR8_SInt,
    kUnityRenderingExtFormatRG8_SInt,
    kUnityRenderingExtFormatRGB8_SInt,
    kUnityRenderingExtFormatRGBA8_SInt,

    // 16 bit integer formats
    kUnityRenderingExtFormatR16_UNorm,
    kUnityRenderingExtFormatRG16_UNorm,
    kUnityRenderingExtFormatRGB16_UNorm,
    kUnityRenderingExtFormatRGBA16_UNorm,
    kUnityRenderingExtFormatR16_SNorm,
    kUnityRenderingExtFormatRG16_SNorm,
    kUnityRenderingExtFormatRGB16_SNorm,
    kUnityRenderingExtFormatRGBA16_SNorm,
    kUnityRenderingExtFormatR16_UInt,
    kUnityRenderingExtFormatRG16_UInt,
    kUnityRenderingExtFormatRGB16_UInt,
    kUnityRenderingExtFormatRGBA16_UInt,
    kUnityRenderingExtFormatR16_SInt,
    kUnityRenderingExtFormatRG16_SInt,
    kUnityRenderingExtFormatRGB16_SInt,
    kUnityRenderingExtFormatRGBA16_SInt,

    // 32 bit integer formats
    kUnityRenderingExtFormatR32_UInt,
    kUnityRenderingExtFormatRG32_UInt,
    kUnityRenderingExtFormatRGB32_UInt,
    kUnityRenderingExtFormatRGBA32_UInt,
    kUnityRenderingExtFormatR32_SInt,
    kUnityRenderingExtFormatRG32_SInt,
    kUnityRenderingExtFormatRGB32_SInt,
    kUnityRenderingExtFormatRGBA32_SInt,

    // HDR formats
    kUnityRenderingExtFormatR16_SFloat,
    kUnityRenderingExtFormatRG16_SFloat,
    kUnityRenderingExtFormatRGB16_SFloat,
    kUnityRenderingExtFormatRGBA16_SFloat,
    kUnityRenderingExtFormatR32_SFloat,
    kUnityRenderingExtFormatRG32_SFloat,
    kUnityRenderingExtFormatRGB32_SFloat,
    kUnityRenderingExtFormatRGBA32_SFloat,

    // Packed formats
    kUnityRenderingExtFormatRGB10A2_UNorm,
    kUnityRenderingExtFormatRGB10A2_UInt,
    kUnityRenderingExtFormatRGB10A2_SInt,
    kUnityRenderingExtFormatRGB9E5_UFloat,
    kUnityRenderingExtFormatRG11B10_UFloat,

    // Alpha format
    kUnityRenderingExtFormatA8_UNorm,
    kUnityRenderingExtFormatA16_UNorm,

    // BGR formats
    kUnityRenderingExtFormatBGR8_SRGB,
    kUnityRenderingExtFormatBGRA8_SRGB,
    kUnityRenderingExtFormatBGR8_UNorm,
    kUnityRenderingExtFormatBGRA8_UNorm,
    kUnityRenderingExtFormatBGR8_SNorm,
    kUnityRenderingExtFormatBGRA8_SNorm,
    kUnityRenderingExtFormatBGR8_UInt,
    kUnityRenderingExtFormatBGRA8_UInt,
    kUnityRenderingExtFormatBGR8_SInt,
    kUnityRenderingExtFormatBGRA8_SInt,

    kUnityRenderingExtFormatBGR10A2_UNorm,
    kUnityRenderingExtFormatBGR10A2_UInt,
    kUnityRenderingExtFormatBGR10A2XR_SRGB,
    kUnityRenderingExtFormatBGR10A2XR_UNorm,
    kUnityRenderingExtFormatBGR10XR_SRGB,
    kUnityRenderingExtFormatBGR10XR_UNorm,
    kUnityRenderingExtFormatBGRA10XR_SRGB,
    kUnityRenderingExtFormatBGRA10XR_UNorm,

    // 16 bit formats
    kUnityRenderingExtFormatRGBA4_UNorm,
    kUnityRenderingExtFormatBGRA4_UNorm,
    kUnityRenderingExtFormatR5G6B5_UNorm,
    kUnityRenderingExtFormatB5G6R5_UNorm,
    kUnityRenderingExtFormatRGB5A1_UNorm,
    kUnityRenderingExtFormatBGR5A1_UNorm,
    kUnityRenderingExtFormatA1RGB5_UNorm,

    // ARGB formats... TextureFormat legacy
    kUnityRenderingExtFormatARGB8_SRGB,
    kUnityRenderingExtFormatARGB8_UNorm,
    kUnityRenderingExtFormatARGB32_SFloat,

    // Depth Stencil for formats
    kUnityRenderingExtFormatDepth16_UInt,
    kUnityRenderingExtFormatDepth24_UInt,
    kUnityRenderingExtFormatDepth24_UInt_Stencil8_UInt,
    kUnityRenderingExtFormatDepth32_SFloat,
    kUnityRenderingExtFormatD32_SFloat_Stencil8_Uint,
    kUnityRenderingExtFormatStencil8_Uint,

    // Compression formats
    kUnityRenderingExtFormatRGB_DXT1_SRGB, kUnityRenderingExtFormatDXTCFirst = kUnityRenderingExtFormatRGB_DXT1_SRGB,
    kUnityRenderingExtFormatRGB_DXT1_UNorm,
    kUnityRenderingExtFormatRGBA_DXT3_SRGB,
    kUnityRenderingExtFormatRGBA_DXT3_UNorm,
    kUnityRenderingExtFormatRGBA_DXT5_SRGB,
    kUnityRenderingExtFormatRGBA_DXT5_UNorm, kUnityRenderingExtFormatDXTCLast = kUnityRenderingExtFormatRGBA_DXT5_UNorm,
    kUnityRenderingExtFormatR_BC4_UNorm, kUnityRenderingExtFormatRGTCFirst = kUnityRenderingExtFormatR_BC4_UNorm,
    kUnityRenderingExtFormatR_BC4_SNorm,
    kUnityRenderingExtFormatRG_BC5_UNorm,
    kUnityRenderingExtFormatRG_BC5_SNorm, kUnityRenderingExtFormatRGTCLast = kUnityRenderingExtFormatRG_BC5_SNorm,
    kUnityRenderingExtFormatRGB_BC6H_UFloat, kUnityRenderingExtFormatBPTCFirst = kUnityRenderingExtFormatRGB_BC6H_UFloat,
    kUnityRenderingExtFormatRGB_BC6H_SFloat,
    kUnityRenderingExtFormatRGBA_BC7_SRGB,
    kUnityRenderingExtFormatRGBA_BC7_UNorm, kUnityRenderingExtFormatBPTCLast = kUnityRenderingExtFormatRGBA_BC7_UNorm,

    kUnityRenderingExtFormatRGB_PVRTC_2Bpp_SRGB, kUnityRenderingExtFormatPVRTCFirst = kUnityRenderingExtFormatRGB_PVRTC_2Bpp_SRGB,
    kUnityRenderingExtFormatRGB_PVRTC_2Bpp_UNorm,
    kUnityRenderingExtFormatRGB_PVRTC_4Bpp_SRGB,
    kUnityRenderingExtFormatRGB_PVRTC_4Bpp_UNorm,
    kUnityRenderingExtFormatRGBA_PVRTC_2Bpp_SRGB,
    kUnityRenderingExtFormatRGBA_PVRTC_2Bpp_UNorm,
    kUnityRenderingExtFormatRGBA_PVRTC_4Bpp_SRGB,
    kUnityRenderingExtFormatRGBA_PVRTC_4Bpp_UNorm, kUnityRenderingExtFormatPVRTCLast = kUnityRenderingExtFormatRGBA_PVRTC_4Bpp_UNorm,

    kUnityRenderingExtFormatRGB_ETC_UNorm, kUnityRenderingExtFormatETCFirst = kUnityRenderingExtFormatRGB_ETC_UNorm,
    kUnityRenderingExtFormatRGB_ETC2_SRGB,
    kUnityRenderingExtFormatRGB_ETC2_UNorm,
    kUnityRenderingExtFormatRGB_A1_ETC2_SRGB,
    kUnityRenderingExtFormatRGB_A1_ETC2_UNorm,
    kUnityRenderingExtFormatRGBA_ETC2_SRGB,
    kUnityRenderingExtFormatRGBA_ETC2_UNorm, kUnityRenderingExtFormatETCLast = kUnityRenderingExtFormatRGBA_ETC2_UNorm,

    kUnityRenderingExtFormatR_EAC_UNorm, kUnityRenderingExtFormatEACFirst = kUnityRenderingExtFormatR_EAC_UNorm,
    kUnityRenderingExtFormatR_EAC_SNorm,
    kUnityRenderingExtFormatRG_EAC_UNorm,
    kUnityRenderingExtFormatRG_EAC_SNorm, kUnityRenderingExtFormatEACLast = kUnityRenderingExtFormatRG_EAC_SNorm,

    kUnityRenderingExtFormatRGBA_ASTC4X4_SRGB, kUnityRenderingExtFormatASTCFirst = kUnityRenderingExtFormatRGBA_ASTC4X4_SRGB,
    kUnityRenderingExtFormatRGBA_ASTC4X4_UNorm,
    kUnityRenderingExtFormatRGBA_ASTC5X5_SRGB,
    kUnityRenderingExtFormatRGBA_ASTC5X5_UNorm,
    kUnityRenderingExtFormatRGBA_ASTC6X6_SRGB,
    kUnityRenderingExtFormatRGBA_ASTC6X6_UNorm,
    kUnityRenderingExtFormatRGBA_ASTC8X8_SRGB,
    kUnityRenderingExtFormatRGBA_ASTC8X8_UNorm,
    kUnityRenderingExtFormatRGBA_ASTC10X10_SRGB,
    kUnityRenderingExtFormatRGBA_ASTC10X10_UNorm,
    kUnityRenderingExtFormatRGBA_ASTC12X12_SRGB,
    kUnityRenderingExtFormatRGBA_ASTC12X12_UNorm, kUnityRenderingExtFormatASTCLast = kUnityRenderingExtFormatRGBA_ASTC12X12_UNorm,

    // Video formats
    kUnityRenderingExtFormatYUV2,

    // Automatic formats, back-end decides
    kUnityRenderingExtFormatLDRAuto,
    kUnityRenderingExtFormatHDRAuto,
    kUnityRenderingExtFormatDepthAuto,
    kUnityRenderingExtFormatShadowAuto,
    kUnityRenderingExtFormatVideoAuto, kUnityRenderingExtFormatLast = kUnityRenderingExtFormatVideoAuto,
};


struct UnityRenderingExtBeforeDrawCallParams
{
    void*   vertexShader;                           // bound vertex shader (platform dependent)
    void*   fragmentShader;                         // bound fragment shader (platform dependent)
    void*   geometryShader;                         // bound geometry shader (platform dependent)
    void*   hullShader;                             // bound hull shader (platform dependent)
    void*   domainShader;                           // bound domain shader (platform dependent)
    int     eyeIndex;                               // the index of the current stereo "eye" being currently rendered.
};


struct UnityRenderingExtCustomBlitParams
{
    UnityTextureID source;                          // source texture
    UnityRenderBuffer destination;                  // destination surface
    unsigned int command;                           // command for the custom blit - could be any UnityRenderingExtCustomBlitCommands command or custom ones.
    unsigned int commandParam;                      // custom parameters for the command
    unsigned int commandFlags;                      // custom flags for the command
};

struct UnityRenderingExtTextureUpdateParams
{
    void*        texData;                           // source data for the texture update. Must be set by the plugin
    unsigned int userData;                          // user defined data. Set by the plugin

    unsigned int textureID;                         // texture ID of the texture to be updated.
    UnityRenderingExtTextureFormat format;          // format of the texture to be updated
    unsigned int width;                             // width of the texture
    unsigned int height;                            // height of the texture
    unsigned int bpp;                               // texture bytes per pixel.
};


// Certain Unity APIs (GL.IssuePluginEventAndData, CommandBuffer.IssuePluginEventAndData) can callback into native plugins.
// Provide them with an address to a function of this signature.
typedef void (UNITY_INTERFACE_API * UnityRenderingEventAndData)(int eventId, void* data);


#ifdef __cplusplus
extern "C" {
#endif

// If exported by a plugin, this function will be called for all the events in UnityRenderingExtEventType
void UNITY_INTERFACE_EXPORT UNITY_INTERFACE_API UnityRenderingExtEvent(UnityRenderingExtEventType event, void* data);
// If exported by a plugin, this function will be called to query the plugin for the queries in UnityRenderingExtQueryType
bool UNITY_INTERFACE_EXPORT UNITY_INTERFACE_API UnityRenderingExtQuery(UnityRenderingExtQueryType query);

#ifdef __cplusplus
}
#endif
