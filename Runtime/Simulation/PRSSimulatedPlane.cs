using System.Collections.Generic;
using System.Linq;
using System.Runtime.Remoting.Messaging;
using UnityEngine;
using UnityEngine.XR.ARFoundation;
using UnityEngine.XR.ARSubsystems;

namespace PleaseRemainSeated.Simulation
{
  /// <summary>
  /// Represents a simulated AR plane.
  /// </summary>
  public class PRSSimulatedPlane : MonoBehaviour, PRSSimulatedTrackable
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
    /// Plane alignment.
    /// </summary>
    public PlaneAlignment alignment
    {
      get => _alignment;
    }
    
    /// <summary>
    /// Center point in world space.
    /// </summary>
    public Vector3 center
    {
      get => transform.position;
    }
    
    /// <summary>
    /// Pose in world space.
    /// </summary>
    public Pose pose
    {
      get => new Pose(transform.position, transform.rotation);
    }

    /// <summary>
    /// Plane boundary in local space.
    /// </summary>
    public List<Vector2> localBoundary
    {
      get => _localBoundary;
    }

    /// <summary>
    /// Whether this plane has been detected by the plane subsystem.
    /// </summary>
    [HideInInspector]
    public bool isDetected;
    
    private TrackableId _identifier;
    private PRSSimulatedTrackableStateChange _stateChange;
    
    private PlaneAlignment _alignment;
    private List<Vector2> _localBoundary;
    public Vector2 size => CalculateSize(localBoundary);

    /// <summary>
    /// Initializes the simulated plane.
    /// </summary>
    /// <param name="identifier">Trackable ID.</param>
    /// <param name="alignment">Alignment type.</param>
    /// <param name="normal">Plane normal.</param>
    /// <param name="boundary">Plane boundary in world space.</param>
    public void Create(TrackableId identifier, PlaneAlignment alignment, Vector3 normal, List<Vector3> boundary)
    {
      _identifier = identifier;
      _alignment = alignment;
      
      transform.position = CalculateCenter(boundary);
      transform.rotation = Quaternion.FromToRotation(Vector3.up, normal);

      var projectedBoundary = new List<Vector3>(boundary);
      pose.InverseTransformPositions(projectedBoundary);
      _localBoundary = projectedBoundary
        .Select(p => new Vector2(p.x, p.z))
        .ToList();

      CreateRaycastCollider(boundary, normal);
    }

    private void CreateRaycastCollider(List<Vector3> vertices, Vector3 normal)
    {
      var mesh = new Mesh();
      mesh.vertices = vertices.Select(v => transform.InverseTransformPoint(v)).ToArray();
      mesh.normals = Enumerable.Repeat(normal, vertices.Count).ToArray();
      mesh.triangles = PRSUtils.TriangulateConvexPolygon(mesh.vertices);

      var collider = gameObject.AddComponent<MeshCollider>();
      collider.sharedMesh = mesh;
    }
    
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