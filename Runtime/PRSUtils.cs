using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using UnityEngine;
using UnityEngine.XR.ARSubsystems;
using Random = System.Random;

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
    /// Checks whether the given plane alignment matches the given detection mode.
    /// </summary>
    /// <param name="alignment">Plane alignment.</param>
    /// <param name="mode">Plane detection mode.</param>
    /// <returns>True if both match.</returns>
    internal static bool PlaneAlignmentMatchesDetectionMode(PlaneAlignment alignment, PlaneDetectionMode mode)
    {
      switch (mode)
      {
        case PlaneDetectionMode.Horizontal:
          return alignment == PlaneAlignment.HorizontalDown || alignment == PlaneAlignment.HorizontalUp;
        
        case PlaneDetectionMode.Vertical:
          return alignment == PlaneAlignment.Vertical;
      }

      return false;
    }
    
    /// <summary>
    /// Generates a new, random Trackable ID.
    /// </summary>
    /// <returns>Trackable ID.</returns>
    internal static TrackableId GenerateTrackableId()
    {
      return new TrackableId(Convert.ToUInt64(rng.Next()), Convert.ToUInt64(rng.Next()));
    }

    /// <summary>
    /// Triangulates a convex polygon.
    /// </summary>
    /// <param name="vertices">Ordered list of hull points.</param>
    /// <returns>List of triangle indices.</returns>
    internal static int[] TriangulateConvexPolygon(Vector3[] vertices)
    {
      var indices = new List<int>(vertices.Length * 3);
      
      for (int i = 2; i < vertices.Length; i++)
      {
        indices.Add(0);
        indices.Add(i - 1);
        indices.Add(i);
      }

      return indices.ToArray();
    }
    
  }
}