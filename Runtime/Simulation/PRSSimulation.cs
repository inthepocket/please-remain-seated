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
            throw new System.Exception("AR Simulation object not found.");
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
    public List<SimulatedPlane> planes = new List<SimulatedPlane>();

    private List<SimulatedPlane> addedPlanes = new List<SimulatedPlane>();
    
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
      addedPlanes = planes.ToList();
    }
    
    /// <summary>
    /// Gets and clears plane updates (planes added, updated, deleted) since the last invocation.
    /// </summary>
    /// <param name="added">List of added planes.</param>
    /// <param name="updated">List of updated planes.</param>
    /// <param name="removed">List of removed plane ID's.</param>
    public void ConsumePlaneUpdates(out List<SimulatedPlane> added, out List<SimulatedPlane> updated, out List<TrackableId> removed)
    {
      added = addedPlanes.ToList();
      addedPlanes.Clear();
      
      updated = new List<SimulatedPlane>();
      removed = new List<TrackableId>();
    }

    #endregion

  }
}