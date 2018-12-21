#include <cstdlib>
#include <Processing.NDI.Lib.h>
#include "Sender.h"

using namespace KlakNDI;

extern "C" Sender *NDI_CreateSender(const char* name)
{
    return new Sender(name);
}

extern "C" void NDI_DestroySender(Sender* sender)
{
    delete sender;
}

extern "C" void NDI_SendFrame(Sender* sender, void* data, int width, int height, uint32_t fourCC)
{
    sender->sendFrame(data, width, height, fourCC);
}

extern "C" void NDI_SyncSender(Sender* sender)
{
    sender->synchronize();
}
