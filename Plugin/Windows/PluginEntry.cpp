#include "Finder.h"
#include "Receiver.h"
#include "Sender.h"
#include "Unity/IUnityInterface.h"

using namespace KlakNDI;

#if defined(_DEBUG)

#include <consoleapi.h>

extern "C" void UNITY_INTERFACE_EXPORT UNITY_INTERFACE_API UnityPluginLoad(IUnityInterfaces* interfaces)
{
    // Open a new console for debug logging.
    FILE * pConsole;
    AllocConsole();
    freopen_s(&pConsole, "CONOUT$", "wb", stdout);
}

#endif

// Common functions

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

extern "C" void UNITY_INTERFACE_EXPORT NDI_SendFrame(Sender* sender, void* data, int width, int height, uint32_t fourCC)
{
    sender->sendFrame(data, width, height, fourCC);
}

// Receiver functions

extern "C" Receiver UNITY_INTERFACE_EXPORT *NDI_TryOpenSourceNamedLike(const char* name)
{
    auto source = Finder::getInstance().getSourceWithNameLike(name);
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

extern "C" uint32_t UNITY_INTERFACE_EXPORT NDI_GetFrameFourCC(Receiver* receiver)
{
    return receiver->getFrameFourCC();
}
