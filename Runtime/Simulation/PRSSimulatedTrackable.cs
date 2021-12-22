using UnityEngine.XR.ARSubsystems;

namespace PleaseRemainSeated.Simulation
{
  /// <summary>
  /// Interface for simulated trackables.
  /// </summary>
  public interface PRSSimulatedTrackable
  {
    public TrackableId identifier
    {
      get;
    }
    
    public PRSSimulatedTrackableStateChange stateChange
    {
      get;
    }
  }
  
  public enum PRSSimulatedTrackableStateChange
  {
    /// <summary>
    /// Minding its own business.
    /// </summary>
    Unchanged,
      
    /// <summary>
    /// Created and ready to be added to trackables.
    /// </summary>
    Added,
      
    /// <summary>
    /// Updated.
    /// </summary>
    Updated,
      
    /// <summary>
    /// Ready to be removed from trackables.
    /// </summary>
    Removed
  }
}