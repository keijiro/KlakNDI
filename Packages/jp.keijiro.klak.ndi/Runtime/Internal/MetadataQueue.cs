using System.Collections.Generic;
using IDisposable = System.IDisposable;
using IntPtr = System.IntPtr;
using Marshal = System.Runtime.InteropServices.Marshal;

namespace Klak.Ndi {

sealed class MetadataQueue
{
    #region Queue entry struct

    // Note: "Data Entry" but actually only used as a return value

    public struct DataEntry : System.IDisposable
    {
        internal IntPtr _pointer;

        public static implicit operator IntPtr(DataEntry e)
          => e._pointer;

        public DataEntry(string payload)
          => _pointer = payload != null ?
               (IntPtr)Marshal.StringToHGlobalAnsi(payload) : IntPtr.Zero;

        public void Dispose()
        {
            if (_pointer != IntPtr.Zero) Marshal.FreeHGlobal(_pointer);
        }
    }

    #endregion

    #region Queue implementation

    Queue<string> _queue = new Queue<string>();

    #endregion

    #region Public methods

    public void Enqueue(string data)
      => _queue.Enqueue(string.IsNullOrEmpty(data) ? null : data);

    public DataEntry Dequeue()
      => new DataEntry(_queue.Count > 0 ? _queue.Dequeue() : null);

    #endregion
}

}
