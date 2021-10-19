using System;
using System.Runtime.InteropServices;
using Unity.Collections;
using UnityEngine.XR.ARSubsystems;
using Random = System.Random;

namespace PleaseRemainSeated
{
  public static class PRSUtils
  {
    private static Random rng = new Random();

    // Constructs a mock "native" object by copying data from a managed proxy object.
    internal static T CopyData<T>(Object data)
    {
      // TODO: support copying into an existing object to avoid allocations?

      int dataSize = Marshal.SizeOf(data);
      IntPtr buffer = Marshal.AllocHGlobal(dataSize);
      Marshal.StructureToPtr(data, buffer, false);

      var result = (T) Marshal.PtrToStructure(buffer, typeof(T));

      Marshal.FreeHGlobal(buffer);
      return result;
    }

    internal static NativeArray<T> ToNativeArray<T>(T[] data, Allocator allocator) where T : struct
    {
      var result = new NativeArray<T>(data.Length, allocator);

      for (int i = 0; i < data.Length; ++i)
        result[i] = data[i];

      return result;

    }
    
    /// <summary>
    /// Generates a new random trackable ID.
    /// </summary>
    /// <returns>Trackable ID.</returns>
    internal static TrackableId GenerateTrackableId()
    {
      return new TrackableId(GenerateRandomUlong(), GenerateRandomUlong());
    }

    /// <summary>
    /// Generates a random unsigned long integer.
    /// </summary>
    /// <returns>Unsigned long integer.</returns>
    internal static ulong GenerateRandomUlong()
    {
      return Convert.ToUInt64(rng.Next());
    }
  }
}