using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using PleaseRemainSeated.Simulation;
using Unity.Collections;
using UnityEngine;
using UnityEngine.Scripting;
using UnityEngine.XR.ARSubsystems;

namespace PleaseRemainSeated
{
  /// <summary>
  /// The plane subsystem implementation for Please Remain Seated.
  /// </summary>
  [Preserve]
  public sealed class PRSPlaneSubsystem : XRPlaneSubsystem
  {
    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.SubsystemRegistration)]
    static void RegisterDescriptor()
    {
#if UNITY_EDITOR || UNITY_STANDALONE
      var cinfo = new XRPlaneSubsystemDescriptor.Cinfo
      {
        id = "PRS-Plane",
        providerType = typeof(PRSProvider),
        subsystemTypeOverride = typeof(PRSPlaneSubsystem),
        supportsHorizontalPlaneDetection = true,
        supportsVerticalPlaneDetection = true,
        supportsArbitraryPlaneDetection = false,
        supportsBoundaryVertices = true,
        supportsClassification = false
      };

      XRPlaneSubsystemDescriptor.Create(cinfo);
#endif
    }

    class PRSProvider : Provider
    {
      public override void Destroy()
      {
      }

      public override void Start()
      {
      }

      public override void Stop()
      {
      }

      // TODO
      public override PlaneDetectionMode requestedPlaneDetectionMode
      {
        get => PlaneDetectionMode.Horizontal | PlaneDetectionMode.Vertical;
        set { }
      }

      // TODO
      public override PlaneDetectionMode currentPlaneDetectionMode => PlaneDetectionMode.Horizontal | PlaneDetectionMode.Vertical;
      
      public override TrackableChanges<BoundedPlane> GetChanges(BoundedPlane defaultPlane, Allocator allocator)
      {
        SimulationAPI.GetPlaneData(out var added, out var updated, out var removed, allocator);
        return TrackableChanges<BoundedPlane>.CopyFrom(added, updated, removed, allocator);
      }
      
      public override void GetBoundary(TrackableId trackableId, Allocator allocator, ref NativeArray<Vector2> boundary)
      {
        var points = SimulationAPI.GetPlaneBoundary(trackableId, allocator);
        
        CreateOrResizeNativeArrayIfNecessary(points.Length, allocator, ref boundary);

        for (int i = 0; i < points.Length; ++i)
          boundary[i] = points[i];
        
        points.Dispose();
      }
    }
    
    /// <summary>
    /// Interface between the plane subsystem and the AR simulation.
    /// </summary>
    static class SimulationAPI
    {
      // Equivalent of <c>BoundedPlane</c>.
      [StructLayout(LayoutKind.Sequential)]
      private struct BoundedPlaneData
      {
        public TrackableId trackableId;
        public TrackableId subsumedById;
        public Vector2 center;
        public Pose pose;
        public Vector2 size;
        public PlaneAlignment alignment;
        public TrackingState trackingState;
        public IntPtr nativePtr;
        public PlaneClassification classification;
      }

      internal static void GetPlaneData(
        out NativeArray<BoundedPlane> added,
        out NativeArray<BoundedPlane> updated,
        out NativeArray<TrackableId> removed,
        Allocator allocator)
      {
        PRSSimulation.instance.ConsumePlaneUpdates(
          out var simAdded,
          out var simUpdated,
          out var simRemoved);

        added = GetBoundedPlanes(simAdded, allocator);
        updated = GetBoundedPlanes(simUpdated, allocator);
        removed = new NativeArray<TrackableId>(simRemoved.ToArray(), allocator);
      }

      /// <summary>
      /// Gets the boundary points of a tracked plane.
      /// </summary>
      /// <param name="trackableId">Plane trackable ID.</param>
      /// <param name="allocator">Memory allocator.</param>
      /// <returns>Native array of points.</returns>
      internal static NativeArray<Vector2> GetPlaneBoundary(TrackableId trackableId, Allocator allocator)
      {
        var plane = PRSSimulation.instance.planes.Find(p => p.identifier == trackableId);
        return PRSUtils.ToNativeArray(plane.localBoundary.ToArray(), allocator);
      }
      
      /// <summary>
      /// Creates an array of <c>ARSubsystems.BoundedPlane</c>s from a set of simulated planes.
      /// </summary>
      /// <param name="planes"></param>
      /// <param name="allocator"></param>
      /// <returns></returns>
      private static NativeArray<BoundedPlane> GetBoundedPlanes(List<SimulatedPlane> planes, Allocator allocator)
      {
        if (planes.Count == 0)
          return new NativeArray<BoundedPlane>(new BoundedPlane[] {}, allocator);
        
        Debug.Log($"GetBoundedPlanes called for {planes.Count} simulated planes.");
        
        var array = planes
          .Select(ConvertSimulatedPlane)
          .ToArray();
        
        return new NativeArray<BoundedPlane>(array, allocator);
      }
    
      // Creates a <c>ARSubsystems.BoundedPlane</c> from a simulated plane.
      private static BoundedPlane ConvertSimulatedPlane(SimulatedPlane p)
      {
        var data = new BoundedPlaneData();
        data.trackableId = p.identifier;
        data.subsumedById = TrackableId.invalidId;
        data.alignment = p.alignment;
        data.trackingState = TrackingState.Tracking;
        data.pose = p.pose;
        data.center = p.center;
        data.size = p.size;
        data.classification = PlaneClassification.None;

        return PRSUtils.CopyData<BoundedPlane>(data);
      }
    }
  }
}