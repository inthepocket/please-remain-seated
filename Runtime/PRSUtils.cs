using System;
using System.Runtime.InteropServices;
using Unity.Collections;
using UnityEngine.XR.ARSubsystems;

namespace PleaseRemainSeated
{
  public static class PRSUtils
  {
    private static Random rng = new Random();
    
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
    
    /// <summary>
    /// Generates a new, random Trackable ID.
    /// </summary>
    /// <returns>Trackable ID.</returns>
    internal static TrackableId GenerateTrackableId()
    {
      return new TrackableId(Convert.ToUInt64(rng.Next()), Convert.ToUInt64(rng.Next()));
    }
  }
}