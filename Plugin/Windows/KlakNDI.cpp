#include "Observer.h"
#include "Receiver.h"
#include "Sender.h"
#include "Unity/IUnityInterface.h"

// Global functions

extern "C" void UNITY_INTERFACE_EXPORT NDI_Initialize()
{
    KlakNDI::Observer::getInstance().start();
}

extern "C" void UNITY_INTERFACE_EXPORT NDI_Finalize()
{
	KlakNDI::Observer::getInstance().stop();
}

// Sender functions

extern "C" KlakNDI::Sender UNITY_INTERFACE_EXPORT *NDI_CreateSender(const char* name)
{
	return new KlakNDI::Sender(name);
}

extern "C" void UNITY_INTERFACE_EXPORT NDI_DestroySender(KlakNDI::Sender* sender)
{
	delete sender;
}

extern "C" void UNITY_INTERFACE_EXPORT NDI_SendFrame(KlakNDI::Sender* sender, void* data, int width, int height)
{
	sender->sendFrame(data, width, height);
}

// Receiver functions

extern "C" KlakNDI::Receiver UNITY_INTERFACE_EXPORT *NDI_CreateReceiver()
{
	auto source = KlakNDI::Observer::getInstance().getFoundSource();
	return source.p_ndi_name != nullptr ? new KlakNDI::Receiver(source) : nullptr;
}

extern "C" void UNITY_INTERFACE_EXPORT NDI_DestroyReceiver(KlakNDI::Receiver* receiver)
{
	delete receiver;
}

extern "C" bool UNITY_INTERFACE_EXPORT NDI_ReceiveFrame(KlakNDI::Receiver* receiver)
{
	return receiver->receiveFrame();
}

extern "C" void UNITY_INTERFACE_EXPORT NDI_FreeFrame(KlakNDI::Receiver* receiver)
{
	receiver->freeFrame();
}

extern "C" int UNITY_INTERFACE_EXPORT NDI_GetFrameWidth(KlakNDI::Receiver* receiver)
{
	return receiver->getFrameWidth();
}

extern "C" int UNITY_INTERFACE_EXPORT NDI_GetFrameHeight(KlakNDI::Receiver* receiver)
{
	return receiver->getFrameHeight();
}

extern "C" void UNITY_INTERFACE_EXPORT *NDI_GetFrameData(KlakNDI::Receiver* receiver)
{
	return receiver->getFrameData();
}