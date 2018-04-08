#include "Finder.h"
#include "Receiver.h"
#include "Sender.h"

#include "Unity/IUnityInterface.h"
#include "Unity/IUnityRenderingExtensions.h"

using namespace KlakNDI;

namespace
{
	// Callback for texture update events
	void UNITY_INTERFACE_EXPORT UNITY_INTERFACE_API TextureUpdateFunction(int eventID, void* data)
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

// Plugin functions

#if defined(_DEBUG)

extern "C" void UNITY_INTERFACE_EXPORT UNITY_INTERFACE_API UnityPluginLoad(IUnityInterfaces* interfaces)
{
	// Open a new console for debug logging.
	FILE * pConsole;
	AllocConsole();
	freopen_s(&pConsole, "CONOUT$", "wb", stdout);
}

#endif

extern "C" UnityRenderingEventAndData UNITY_INTERFACE_EXPORT NDI_GetTextureUpdateFunction()
{
	return TextureUpdateFunction;
}

extern "C" int UNITY_INTERFACE_EXPORT NDI_RetrieveSourceNames(const char* destination[], int maxCount)
{
	return Finder::getInstance().retrieveSourceNames(destination, maxCount);
}

// Sender functions

extern "C" Sender UNITY_INTERFACE_EXPORT *NDI_CreateSender(const char* name)
{
	return new Sender(name);
}

extern "C" void UNITY_INTERFACE_EXPORT NDI_DestroySender(Sender* sender)
{
	delete sender;
}

extern "C" void UNITY_INTERFACE_EXPORT NDI_SendFrame(Sender* sender, void* data, int width, int height)
{
	sender->sendFrame(data, width, height);
}

// Receiver functions

extern "C" Receiver UNITY_INTERFACE_EXPORT *NDI_TryCreateReceiverWithClause(const char* clause)
{
	auto source = Finder::getInstance().getSourceWithClause(clause);
	return source.p_ndi_name != nullptr ? new Receiver(source) : nullptr;
}

extern "C" void UNITY_INTERFACE_EXPORT NDI_DestroyReceiver(Receiver* receiver)
{
	delete receiver;
}

extern "C" uint32_t UNITY_INTERFACE_EXPORT NDI_GetReceiverID(Receiver* receiver)
{
	return receiver->getID();
}

extern "C" int UNITY_INTERFACE_EXPORT NDI_GetFrameWidth(Receiver* receiver)
{
	return receiver->getFrameWidth();
}

extern "C" int UNITY_INTERFACE_EXPORT NDI_GetFrameHeight(Receiver* receiver)
{
	return receiver->getFrameHeight();
}
