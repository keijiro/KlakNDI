#include "Receiver.h"
#include "Unity/IUnityRenderingExtensions.h"

using namespace KlakNDI;

namespace
{
    // Callback for texture update events
    void UNITY_INTERFACE_EXPORT UNITY_INTERFACE_API TextureUpdateCallback(int eventID, void* data)
    {
        auto event = static_cast<UnityRenderingExtEventType>(eventID);

        if (event == kUnityRenderingExtEventUpdateTextureBegin)
        {
            // UpdateTextureBegin: Retrieve a received frame from the receiver.
            auto params = reinterpret_cast<UnityRenderingExtTextureUpdateParams*>(data);
            auto receiver = Receiver::getInstanceFromID(params->userData);

            if (receiver->receiveFrame())
            {
                // Check if the texture dimensions match.
                if (params->width == receiver->getFrameWidth() / 2 &&
                    params->height == receiver->getFrameHeight())
                {
                    params->texData = const_cast<void*>(receiver->getFrameData());
                }
                else
                {
                    // Not match: Let this frame drop.
                    receiver->freeFrame();
                }
            }
        }
        else if (event == kUnityRenderingExtEventUpdateTextureEnd)
        {
            // UpdateTextureEnd: Free up the frame passed to Unity.
            auto params = reinterpret_cast<UnityRenderingExtTextureUpdateParams*>(data);
            if (params->texData != nullptr)
                Receiver::getInstanceFromID(params->userData)->freeFrame();
        }
    }
}

extern "C" UnityRenderingEventAndData UNITY_INTERFACE_EXPORT NDI_GetTextureUpdateCallback()
{
    return TextureUpdateCallback;
}
