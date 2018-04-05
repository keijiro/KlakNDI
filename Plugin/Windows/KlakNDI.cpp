#include "Observer.h"
#include "Receiver.h"
#include "Sender.h"
#include "Unity/IUnityInterface.h"

using namespace KlakNDI;

// Sender functions

extern "C" Sender UNITY_INTERFACE_EXPORT *NDI_CreateSender(const char* name)
{
	Observer::getInstance();
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

extern "C" Receiver UNITY_INTERFACE_EXPORT *NDI_CreateReceiver()
{
	auto source = Observer::getInstance().getFoundSource();
	return source.p_ndi_name != nullptr ? new Receiver(source) : nullptr;
}

extern "C" void UNITY_INTERFACE_EXPORT NDI_DestroyReceiver(Receiver* receiver)
{
	delete receiver;
}

extern "C" bool UNITY_INTERFACE_EXPORT NDI_ReceiveFrame(Receiver* receiver)
{
	return receiver->receiveFrame();
}

extern "C" void UNITY_INTERFACE_EXPORT NDI_FreeFrame(Receiver* receiver)
{
	receiver->freeFrame();
}

extern "C" int UNITY_INTERFACE_EXPORT NDI_GetFrameWidth(Receiver* receiver)
{
	return receiver->getFrameWidth();
}

extern "C" int UNITY_INTERFACE_EXPORT NDI_GetFrameHeight(Receiver* receiver)
{
	return receiver->getFrameHeight();
}

extern "C" void UNITY_INTERFACE_EXPORT *NDI_GetFrameData(Receiver* receiver)
{
	return receiver->getFrameData();
}
