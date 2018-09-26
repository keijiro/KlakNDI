#include "Common.h"
#include "Receiver.h"
#include "Unity/IUnityRenderingExtensions.h"

using namespace KlakNDI;

namespace
{
    // Callback for texture update events
    void UNITY_INTERFACE_API TextureUpdateCallback(int eventID, void* data)
    {
        auto event = static_cast<UnityRenderingExtEventType>(eventID);

        if (event == kUnityRenderingExtEventUpdateTextureBeginV2)
        {
            // UpdateTextureBegin: Retrieve a received frame from the receiver.
            auto params = reinterpret_cast<UnityRenderingExtTextureUpdateParamsV2*>(data);
            auto receiver = Receiver::getInstanceFromID(params->userData);

            if (receiver->receiveFrame())
            {
                // Check if it's an alpha supported frame.
                auto alpha = (receiver->getFrameFourCC() == NDIlib_FourCC_type_UYVA);

                // Calculate the texture dimensions.
                auto width = receiver->getFrameWidth() / 2;
                auto height = receiver->getFrameHeight() * (alpha ? 3 : 2) / 2;

                // Check if the texture dimensions match.
                if (params->width == width && params->height == height)
                    params->texData = const_cast<void*>(receiver->getFrameData());
                else
                    receiver->freeFrame(); // Not match: Let this frame drop.
            }
        }
        else if (event == kUnityRenderingExtEventUpdateTextureEndV2)
        {
            // UpdateTextureEnd: Free up the frame passed to Unity.
            auto params = reinterpret_cast<UnityRenderingExtTextureUpdateParamsV2*>(data);
            if (params->texData != nullptr)
                Receiver::getInstanceFromID(params->userData)->freeFrame();
        }
    }
}

extern "C" UnityRenderingEventAndData UNITY_INTERFACE_EXPORT NDI_GetTextureUpdateCallback()
{
    return TextureUpdateCallback;
}
