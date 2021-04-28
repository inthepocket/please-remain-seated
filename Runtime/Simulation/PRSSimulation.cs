using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.XR.ARSubsystems;
using UnityEngine.Assertions;
using PleaseRemainSeated;

using Debug = UnityEngine.Debug;
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

    #region MonoBehaviour

    public void Start()
    {
    }

    public void Update()
    {
    }

    public void OnDestroy()
    {
    }

    #endregion

  }
}