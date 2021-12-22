using UnityEngine;
using UnityEngine.XR.ARSubsystems;

namespace PleaseRemainSeated.Simulation
{
  /// <summary>
  /// Represents a simulated AR anchor.
  /// </summary>
  public class PRSSimulatedAnchor : MonoBehaviour, PRSSimulatedTrackable
  {
    public TrackableId identifier
    {
      get => _identifier;
    }

    public PRSSimulatedTrackableStateChange stateChange 
    { 
      get => _stateChange;
      set => _stateChange = value;
    }

    /// <summary>
    /// Pose in world space.
    /// </summary>
    public Pose pose
    {
      get => new Pose(transform.position, transform.rotation);
    }
    
    private TrackableId _identifier;
    private PRSSimulatedTrackableStateChange _stateChange;

    /// <summary>
    /// Initializes the simulated anchor.
    /// </summary>
    /// <param name="identifier">Trackable ID.</param>
    /// <param name="position">Position in world space.</param>
    /// <param name="rotation">Rotation in world space.</param>
    public void Create(TrackableId identifier, Vector3 position, Quaternion rotation)
    {
      _identifier = identifier;
      _stateChange = PRSSimulatedTrackableStateChange.Added;
      
      transform.SetPositionAndRotation(position, rotation);
    }
  }
}