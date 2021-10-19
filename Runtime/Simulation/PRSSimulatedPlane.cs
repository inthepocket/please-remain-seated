using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.XR.ARFoundation;
using UnityEngine.XR.ARSubsystems;

namespace PleaseRemainSeated.Simulation
{
  /// <summary>
  /// Represents a simulated AR plane.
  /// </summary>
  public class SimulatedPlane
  {
    public SimulatedPlane(PlaneAlignment alignment, Vector3 normal, List<Vector3> boundary)
    {
      identifier = PRSUtils.GenerateTrackableId();

      this.alignment = alignment;
      this.boundary = boundary;

      center = CalculateCenter(this.boundary);
      pose = new Pose(center, Quaternion.FromToRotation(Vector3.up, normal));

      var projectedBoundary = new List<Vector3>(boundary);
      pose.InverseTransformPositions(projectedBoundary);
      this.localBoundary = projectedBoundary
        .Select(p => new Vector2(p.x, p.z))
        .ToList();
      
      size = CalculateSize(this.localBoundary);
    }
    
    /// <summary>
    /// Trackable ID.
    /// </summary>
    public TrackableId identifier { get; }

    /// <summary>
    /// Pose in world space.
    /// </summary>
    public Pose pose { get; }
    
    /// <summary>
    /// 3D boundary polygon in world space.
    /// </summary>
    public List<Vector3> boundary { get; }
    
    /// <summary>
    /// 2D boundary polygon in local space (relative to pose).
    /// </summary>
    public List<Vector2> localBoundary { get; }
    
    /// <summary>
    /// Center point in world space.
    /// </summary>
    public Vector3 center { get; }

    /// <summary>
    /// Local 2D size.
    /// </summary>
    public Vector2 size { get; }
    
    /// <summary>0
    /// Plane alignment.
    /// </summary>
    public PlaneAlignment alignment { get; }

    // Calculates the center (average) of the given set of points.
    private Vector3 CalculateCenter(List<Vector3> points)
    {
      var pos = Vector3.zero;
      foreach (var p in points)
        pos += p;

      return pos / points.Count;
    }
    
    // Calculates the 2D size (minimum/maximum) of the given set of points.
    private Vector2 CalculateSize(List<Vector2> points)
    {
      float minX = 0, minY = 0, maxX = 0, maxY = 0;
      
      foreach (var p in points)
      {
        if (p.x < minX) minX = p.x;
        if (p.x > maxX) maxX = p.x;
        if (p.y < minY) minY = p.y;
        if (p.y < minY) maxY = p.y;
      }
      
      return new Vector2(maxX - minX, maxY - minY);
    }
  }
}