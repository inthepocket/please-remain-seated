using System;
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
  /// The anchor subsystem implementation for Please Remain Seated.
  /// </summary>
  [Preserve]
  public sealed class PRSAnchorSubsystem : XRAnchorSubsystem
  {
    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.SubsystemRegistration)]
    static void RegisterDescriptor()
    {
#if UNITY_EDITOR || UNITY_STANDALONE
      var cinfo = new XRAnchorSubsystemDescriptor.Cinfo
      {
        id = "PRS-Anchor",
        providerType = typeof(PRSProvider),
        subsystemTypeOverride = typeof(PRSAnchorSubsystem),
        supportsTrackableAttachments = true
      };

      XRAnchorSubsystemDescriptor.Create(cinfo);
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

      public override TrackableChanges<XRAnchor> GetChanges(XRAnchor defaultAnchor, Allocator allocator)
      {
        SimulationAPI.GetAnchorData(out var added, out var updated, out var removed, allocator);
        return TrackableChanges<XRAnchor>.CopyFrom(added, updated, removed, allocator);
      }

      public override bool TryAddAnchor(Pose pose, out XRAnchor anchor)
      {
        anchor = SimulationAPI.AddAnchor(pose, null);
        return true;
      }

      public override bool TryAttachAnchor(TrackableId trackableToAffix, Pose pose, out XRAnchor anchor)
      {
        anchor = SimulationAPI.AddAnchor(pose, trackableToAffix);
        return true;
      }

      public override bool TryRemoveAnchor(TrackableId anchorId)
      {
        return SimulationAPI.RemoveAnchor(anchorId);
      }
    }

    /// <summary>
    /// Interface between the anchor subsystem and the AR simulation.
    /// </summary>
    static class SimulationAPI
    {
      // Equivalent of <c>XRAnchor</c>.
      [StructLayout(LayoutKind.Sequential)]
      private struct XRAnchorData
      {
        public TrackableId trackableId;
        public Pose pose;
        public TrackingState trackingState;
        public IntPtr nativePtr;
        public Guid sessionId;
      }

      internal static XRAnchor AddAnchor(Pose pose, TrackableId? trackableToAffix)
      {
        var simAnchor = PRSSimulation.instance.AddAnchor(pose.position, pose.rotation, trackableToAffix);
        return ConvertSimulatedAnchor(simAnchor);
      }

      internal static bool RemoveAnchor(TrackableId identifier)
      {
        if (PRSSimulation.instance == null)
        {
          // Can happen if TryRemoveAnchor called during app shutdown. 
          return false;
        }

        return PRSSimulation.instance.RemoveAnchor(identifier);
      }

      internal static void GetAnchorData(
        out NativeArray<XRAnchor> added,
        out NativeArray<XRAnchor> updated,
        out NativeArray<TrackableId> removed,
        Allocator allocator)
      {
        PRSSimulation.instance.ConsumeAnchorUpdates(
          out var simAdded,
          out var simUpdated,
          out var simRemoved);

        added = new NativeArray<XRAnchor>(simAdded.Select(ConvertSimulatedAnchor).ToArray(), allocator);
        updated = new NativeArray<XRAnchor>(simUpdated.Select(ConvertSimulatedAnchor).ToArray(), allocator);
        removed = new NativeArray<TrackableId>(simRemoved.ToArray(), allocator);
      }

      // Creates a <c>ARSubsystems.XRAnchor</c> from a simulated anchor.
      private static XRAnchor ConvertSimulatedAnchor(PRSSimulatedAnchor a)
      {
        var data = new XRAnchorData
        {
          trackableId = a.identifier,
          pose = a.pose,
          trackingState = TrackingState.Tracking,
          nativePtr = IntPtr.Zero,
          sessionId = Guid.Empty
        };

        return PRSUtils.CopyData<XRAnchor>(data);
      }
    }
  }
}