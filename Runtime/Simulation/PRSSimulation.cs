using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.XR.ARSubsystems;
using Object = UnityEngine.Object;

namespace PleaseRemainSeated.Simulation
{
  /// <summary>
  /// Virtual AR environment.
  /// </summary>
  public class PRSSimulation : MonoBehaviour
  {
    #region Singleton implementation
    
    public static PRSSimulation instance
    {
      get
      {
        if (_instance == null)
        {
          var instances = Object.FindObjectsOfType<PRSSimulation>();

          if (instances.Length == 0)
          {
            return null;
          }
          
          if (instances.Length > 1)
          {
            throw new System.Exception("More than one AR Simulation object present.");
          }
          
          _instance = instances[0];
        }

        return _instance;
      }
    }

    private static PRSSimulation _instance;
    
    #endregion
    
    [Tooltip("Layer containing simulation objects.")]
    public LayerMask simulationLayer;

    /// <summary>
    /// Simulated AR device.
    /// </summary>
    public PRSSimulatedDevice device;

    /// <summary>
    /// Simulated AR planes.
    /// </summary>
    public List<PRSSimulatedPlane> planes = new List<PRSSimulatedPlane>();
    
    /// <summary>
    /// Simulated AR anchors.
    /// </summary>
    public List<PRSSimulatedAnchor> anchors = new List<PRSSimulatedAnchor>();
    
    private GameObject trackablesRoot;
    
    #region MonoBehaviour

    public void Start()
    {
      trackablesRoot = new GameObject("[Trackables]");
      trackablesRoot.transform.SetParent(transform);
      trackablesRoot.hideFlags = HideFlags.NotEditable | HideFlags.DontSave;
      trackablesRoot.layer = (int)Mathf.Log(simulationLayer.value, 2);

      // Generate simulated planes.
      var planeGenerator = new PRSPlaneGenerator(trackablesRoot);
      planes = planeGenerator.Generate(simulationLayer);
    }

    #endregion

    #region Planes
    
    /// <summary>
    /// Triggers detection of undetected simulated planes that match the given plane detection mode.
    /// </summary>
    /// <param name="mode">Plane detection mode.</param>
    public void DetectPlanes(PlaneDetectionMode mode)
    { 
      var detectedPlanes = planes
        .Where(p => p.isDetected == false)
        .Where(p => PRSUtils.PlaneAlignmentMatchesDetectionMode(p.alignment, mode))
        .ToList();

      foreach (var plane in detectedPlanes)
      {
        plane.stateChange = PRSSimulatedTrackableStateChange.Added;
        plane.isDetected = true;
      }
    }
    
    /// <summary>
    /// Gets and clears plane updates (planes added, updated, deleted) since the last invocation.
    /// </summary>
    /// <param name="added">List of added planes.</param>
    /// <param name="updated">List of updated planes.</param>
    /// <param name="removed">List of removed plane ID's.</param>
    public void ConsumePlaneUpdates(out List<PRSSimulatedPlane> added, out List<PRSSimulatedPlane> updated, out List<TrackableId> removed)
    {
      // Output scheduled state changes.
      added = planes
        .Where(p => p.stateChange == PRSSimulatedTrackableStateChange.Added)
        .ToList();
      
      updated = planes
        .Where(p => p.stateChange == PRSSimulatedTrackableStateChange.Updated)
        .ToList();

      var removedPlanes = planes
        .Where(p => p.stateChange == PRSSimulatedTrackableStateChange.Removed)
        .ToList();
      
      removed = removedPlanes
        .Select(p => p.identifier)
        .ToList();
      
      // Clear state changes.
      added.ForEach(p => p.stateChange = PRSSimulatedTrackableStateChange.Unchanged);
      updated.ForEach(p => p.stateChange = PRSSimulatedTrackableStateChange.Unchanged);
      removedPlanes.ForEach(p => p.stateChange = PRSSimulatedTrackableStateChange.Unchanged);

      // Delete simulated planes scheduled for removal.
      removedPlanes.ForEach(p =>
      {
        if (p != null) Destroy(p.gameObject);
      });

    }

    #endregion
    
    #region Raycasting

    // TODO: Physics.Raycast call should happen here instead of the subsystem
    
    #endregion
    
    #region Anchors

    /// <summary>
    /// Adds an anchor.
    /// </summary>
    /// <param name="position">Anchor position.</param>
    /// <param name="rotation">Anchor rotation.</param>
    /// <param name="parentTrackable">Optional parent trackable to attach the anchor to.
    /// </param>
    /// <returns></returns>
    public PRSSimulatedAnchor AddAnchor(Vector3 position, Quaternion rotation, TrackableId? parentTrackable = null)
    { 
      var identifier = PRSUtils.GenerateTrackableId();
      
      var obj = new GameObject($"Anchor {identifier.ToString()}")
      {
        hideFlags = trackablesRoot.gameObject.hideFlags,
        layer = trackablesRoot.gameObject.layer
      };

      if (parentTrackable.HasValue)
      {
        var parent = trackablesRoot
          .GetComponentsInChildren<PRSSimulatedPlane>()
          .First(t => t.identifier == parentTrackable.Value);
        
        obj.transform.SetParent(parent.transform);
      }
      else
      {
        obj.transform.SetParent(trackablesRoot.transform);
      }
      
      var anchor = obj.AddComponent<PRSSimulatedAnchor>();
      anchor.Create(identifier, position, rotation);
      anchor.stateChange = PRSSimulatedTrackableStateChange.Added;
      anchors.Add(anchor);
      return anchor;
    }

    public bool RemoveAnchor(TrackableId identifier)
    {
      var anchor = trackablesRoot
        .GetComponentsInChildren<PRSSimulatedAnchor>()
        .FirstOrDefault(t => t.identifier == identifier);

      if (anchor != null)
      {
        anchor.stateChange = PRSSimulatedTrackableStateChange.Removed;
        return true;
      }
      else
      {
        return false;
      }
    }

    /// <summary>
    /// Gets and clears anchor updates (anchors added, updated, deleted) since the last invocation.
    /// </summary>
    /// <param name="added">List of added anchors.</param>
    /// <param name="updated">List of updated anchors.</param>
    /// <param name="removed">List of removed anchor ID's.</param>
    public void ConsumeAnchorUpdates(out List<PRSSimulatedAnchor> added, out List<PRSSimulatedAnchor> updated, out List<TrackableId> removed)
    {
      // Output scheduled state changes.
      added = anchors
        .Where(p => p.stateChange == PRSSimulatedTrackableStateChange.Added)
        .ToList();
      
      updated = anchors
        .Where(p => p.stateChange == PRSSimulatedTrackableStateChange.Updated)
        .ToList();

      var removedAnchors = anchors
        .Where(p => p.stateChange == PRSSimulatedTrackableStateChange.Removed)
        .ToList();
      
      removed = removedAnchors
        .Select(p => p.identifier)
        .ToList();
      
      // Clear state changes.
      added.ForEach(p => p.stateChange = PRSSimulatedTrackableStateChange.Unchanged);
      updated.ForEach(p => p.stateChange = PRSSimulatedTrackableStateChange.Unchanged);
      removedAnchors.ForEach(p => p.stateChange = PRSSimulatedTrackableStateChange.Unchanged);

      // Delete simulated anchors scheduled for removal.
      removedAnchors.ForEach(p =>
      {
        if (p != null) Destroy(p.gameObject);
      });
    }

    #endregion

  }
}