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
  public sealed class PRSRaycastSubsystem : XRRaycastSubsystem
  {
    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.SubsystemRegistration)]
    static void RegisterDescriptor()
    {
#if UNITY_EDITOR || UNITY_STANDALONE
      var cinfo = new XRRaycastSubsystemDescriptor.Cinfo
      {
        id = "PRS-Raycast",
        providerType = typeof(PRSProvider),
        subsystemTypeOverride = typeof(PRSRaycastSubsystem),
        supportsViewportBasedRaycast = true,
        supportsWorldBasedRaycast = false,
        supportedTrackableTypes = TrackableType.PlaneWithinPolygon,
        supportsTrackedRaycasts = false
      };

      XRRaycastSubsystemDescriptor.RegisterDescriptor(cinfo);
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
      
      public override unsafe NativeArray<XRRaycastHit> Raycast(
        XRRaycastHit defaultRaycastHit,
        Vector2 screenPoint,
        TrackableType trackableTypeMask,
        Allocator allocator)
      {
        if (trackableTypeMask == TrackableType.PlaneWithinBounds)
        {
          return SimulationAPI.TryRaycastPlanesWithinBounds(defaultRaycastHit, screenPoint, allocator);
        }
        else
        {
          return new NativeArray<XRRaycastHit>(new XRRaycastHit[] {}, allocator);
        }
      }
    }
    
    /// <summary>
    /// Interface between the plane subsystem and the AR simulation.
    /// </summary>
    static class SimulationAPI
    {
      // Equivalent of <c>XRRaycastHit</c>.
      [StructLayout(LayoutKind.Sequential)]
      private struct XRRaycastHitData
      {
        public TrackableId trackableId;
        public Pose pose;
        public float distance;
        public TrackableType hitType;
      }
      
      internal static NativeArray<XRRaycastHit> TryRaycastPlanesWithinBounds(XRRaycastHit defaultRaycastHit, Vector2 screenPoint,
        Allocator allocator)
      {
        Ray ray = PRSSimulation.instance.device.Camera.ViewportPointToRay(screenPoint);
        RaycastHit hit;
        
        if (Physics.Raycast(ray, out hit, Mathf.Infinity))
        {
          var plane = hit.collider.gameObject.GetComponent<SimulatedPlane>();
          if (plane)
          {
            var data = new XRRaycastHitData();
            data.trackableId = plane.identifier;
            data.pose = new Pose(hit.point, Quaternion.LookRotation(hit.normal));
            data.distance = hit.distance;
            data.hitType = TrackableType.PlaneWithinBounds;

            return new NativeArray<XRRaycastHit>(new [] { PRSUtils.CopyData<XRRaycastHit>(data) }, allocator);;
          }
        }

        return new NativeArray<XRRaycastHit>(new XRRaycastHit[] {}, allocator);
      }
    }
  }
}