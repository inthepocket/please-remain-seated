using UnityEngine;
using UnityEngine.XR.ARSubsystems;

namespace PleaseRemainSeated.Simulation
{
  /// <summary>
  /// Represents a simulated AR anchor.
  /// </summary>
  public class PRSSimulatedAnchor : MonoBehaviour, PRSSimulatedTrackable
  {
    public TrackableId identifier => _identifier;

    public PRSSimulatedTrackableStateChange stateChange 
    { 
      get => _stateChange;
      set => _stateChange = value;
    }

    /// <summary>
    /// Pose in world space.
    /// </summary>
    public Pose pose => new Pose(transform.position, transform.rotation);

    private TrackableId _identifier;
    private PRSSimulatedTrackableStateChange _stateChange;

    /// <summary>
    /// Initializes the simulated anchor.
    /// </summary>
    /// <param name="id">Trackable ID.</param>
    /// <param name="position">Position in world space.</param>
    /// <param name="rotation">Rotation in world space.</param>
    public void Create(TrackableId id, Vector3 position, Quaternion rotation)
    {
      _identifier = id;
      transform.SetPositionAndRotation(position, rotation);
    }
  }
}