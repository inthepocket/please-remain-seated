using System;
using System.Runtime.InteropServices;

namespace PleaseRemainSeated
{
  public static class PRSUtils
  {
    // Constructs a mock "native" object by copying data from a managed proxy object.
    internal static T CopyData<T>(System.Object data)
    {
      // TODO: support copying into an existing object to avoid allocations?

      int dataSize = Marshal.SizeOf(data);
      IntPtr buffer = Marshal.AllocHGlobal(dataSize);
      Marshal.StructureToPtr(data, buffer, false);

      var result = (T) Marshal.PtrToStructure(buffer, typeof(T));

      Marshal.FreeHGlobal(buffer);
      return result;
    }
  }
}