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
      
      public override NativeArray<XRRaycastHit> Raycast(
        XRRaycastHit defaultRaycastHit,
        Vector2 screenPoint,
        TrackableType trackableTypeMask,
        Allocator allocator)
      {
        if (trackableTypeMask == TrackableType.PlaneWithinPolygon)
        {
          return SimulationAPI.TryRaycastPlanesWithinPolygon(defaultRaycastHit, screenPoint, allocator);
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
      
      // ReSharper disable once UnusedParameter.Local
      internal static NativeArray<XRRaycastHit> TryRaycastPlanesWithinPolygon(XRRaycastHit defaultRaycastHit, Vector2 screenPoint,
        Allocator allocator)
      { 
        Ray ray = PRSSimulation.instance.device.Camera.ViewportPointToRay(screenPoint);

        // ReSharper disable once Unity.PreferNonAllocApi
        var hits = Physics.RaycastAll(ray, Mathf.Infinity)
          .Where(h => h.collider.gameObject.GetComponent<PRSSimulatedPlane>() != null)
          .Where(h => h.collider.gameObject.GetComponent<PRSSimulatedPlane>().isDetected)
          .OrderBy(h => h.distance)
          .ToList();

        var data = new XRRaycastHit[hits.Count];
        for (int i = 0; i < hits.Count; i++)
        {
          var hit = hits[i];
          var plane = hit.collider.gameObject.GetComponent<PRSSimulatedPlane>();
          
          var record = new XRRaycastHitData();
          record.trackableId = plane.identifier;
          record.pose = new Pose(hit.point, Quaternion.LookRotation(hit.normal));
          record.distance = hit.distance;
          record.hitType = TrackableType.PlaneWithinBounds;

          data[i] = PRSUtils.CopyData<XRRaycastHit>(record);
        }

        return new NativeArray<XRRaycastHit>(data, allocator);
      }
    }
  }
}
