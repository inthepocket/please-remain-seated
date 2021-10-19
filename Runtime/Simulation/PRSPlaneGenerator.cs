using System.Linq;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.ARSubsystems;

namespace PleaseRemainSeated.Simulation
{
  /// <summary>
  /// Observes scene colliders and generates simulated AR planes from them. Currently limited to <c>BoxCollider</c>s.
  /// </summary>
  public class PRSPlaneGenerator
  {
    // Minimum length in each dimension for a collider surface to be counted as an AR plane.
    private static readonly double MinimumSurfaceDimension = 0.15;
   
    // Maximum angle between two planes to be considered equal.
    private static readonly double MaximumFaceAngle = 1;

    public List<SimulatedPlane> planes = new List<SimulatedPlane>();

    /// <summary>
    /// Generates simulated plane data from scene colliders.
    /// </summary>
    /// <param name="simulationLayer">AR simulation layer.</param>
    public List<SimulatedPlane> Generate(LayerMask simulationLayer)
    { 
      planes = Object.FindObjectsOfType<BoxCollider>()
        .SelectMany(GetPlanesFromCollider)
        .ToList();

      var horizontalCount = planes.Count(p => p.alignment == PlaneAlignment.HorizontalUp);
      var verticalCount = planes.Count(p => p.alignment == PlaneAlignment.Vertical);
      Debug.Log($"Found {horizontalCount} horizontal and {verticalCount} vertical planes in the scene.");

      return planes;
    }

    private List<SimulatedPlane> GetPlanesFromCollider(BoxCollider box)
    {
      var result = new List<SimulatedPlane>();
      
      // For each of the 6 directions, determine the corner points of the face in world space.
      var size = box.size;
      
      var sides = new List<(Vector3 normal, float sideA, float sideB)>
      {
        (Vector3.up, size.x, size.z),
        (Vector3.down, size.x, size.z),
        (Vector3.forward, size.x, size.y),
        (Vector3.back, size.x, size.y),
        (Vector3.left, size.y, size.z),
        (Vector3.right, size.y, size.z),
      };

      foreach (var face in sides)
      {
        // Filter out small faces.
        if (face.sideA < MinimumSurfaceDimension || face.sideB < MinimumSurfaceDimension)
          continue;

        var worldSpaceNormal = box.transform.TransformDirection(face.normal);

        // HorizontalUp
        if (Vector3.Angle(worldSpaceNormal, Vector3.up) <= MaximumFaceAngle)
        {
          var plane = new SimulatedPlane(
            PlaneAlignment.HorizontalUp,
            worldSpaceNormal,
            GetPointsFromBoxColliderFace(box, face.normal)
            );
          
          result.Add(plane);
        }
        
        // HorizontalDown
        if (Vector3.Angle(worldSpaceNormal, Vector3.down) <= MaximumFaceAngle)
        {
          var plane = new SimulatedPlane(
            PlaneAlignment.HorizontalDown,
            worldSpaceNormal,
            GetPointsFromBoxColliderFace(box, face.normal)
          );
          
          result.Add(plane);
        }

        // Vertical
        if (Mathf.Abs(Vector3.Dot(Vector3.up, worldSpaceNormal)) < Mathf.Epsilon)
        {
          var plane = new SimulatedPlane(
            PlaneAlignment.Vertical,
            worldSpaceNormal,
            GetPointsFromBoxColliderFace(box, face.normal)
          );
          
          result.Add(plane);
        }
      }

      return result;
    }

    private List<Vector3> GetPointsFromBoxColliderFace(BoxCollider box, Vector3 normal)
    {
      List<Vector3> localPts = null;
      var halfSize = box.size / 2f;

      if (normal == Vector3.up)
      {
        localPts = new List<Vector3>
        {
          new Vector3(-halfSize.x, halfSize.y, halfSize.z),
          new Vector3(halfSize.x, halfSize.y, halfSize.z),
          new Vector3(halfSize.x, halfSize.y, -halfSize.z), 
          new Vector3(-halfSize.x, halfSize.y, -halfSize.z)
        };
      }
      else if (normal == Vector3.down)
      {
        localPts = new List<Vector3>
        {
          new Vector3(-halfSize.x, -halfSize.y, -halfSize.z),
          new Vector3(halfSize.x, -halfSize.y, -halfSize.z), 
          new Vector3(halfSize.x, -halfSize.y, halfSize.z),
          new Vector3(-halfSize.x, -halfSize.y, halfSize.z)
        };
      }
      else if (normal == Vector3.forward)
      {
        localPts = new List<Vector3>
        {
          new Vector3(halfSize.x, -halfSize.y, halfSize.z),
          new Vector3(halfSize.x, halfSize.y, halfSize.z), 
          new Vector3(-halfSize.x, halfSize.y, halfSize.z),
          new Vector3(-halfSize.x, -halfSize.y, halfSize.z)
        };
      }
      else if (normal == Vector3.back)
      {
        localPts = new List<Vector3>
        {
          new Vector3(-halfSize.x, -halfSize.y, -halfSize.z),
          new Vector3(-halfSize.x, halfSize.y, -halfSize.z),
          new Vector3(halfSize.x, halfSize.y, -halfSize.z), 
          new Vector3(halfSize.x, -halfSize.y, -halfSize.z)
        };
      }
      else if (normal == Vector3.right)
      {
        localPts = new List<Vector3>
        {
          new Vector3(halfSize.x, -halfSize.y, -halfSize.z),
          new Vector3(halfSize.x, halfSize.y, -halfSize.z),
          new Vector3(halfSize.x, halfSize.y, halfSize.z), 
          new Vector3(halfSize.x, -halfSize.y, halfSize.z)
        };
      }
      else if (normal == Vector3.left)
      {
        localPts = new List<Vector3>
        {
          new Vector3(-halfSize.x, -halfSize.y, halfSize.z),
          new Vector3(-halfSize.x, halfSize.y, halfSize.z),
          new Vector3(-halfSize.x, halfSize.y, -halfSize.z), 
          new Vector3(-halfSize.x, -halfSize.y, -halfSize.z)
        };
      }
      
      return localPts
        .Select(p => box.gameObject.transform.TransformPoint(box.center + p))
        .ToList();
    }
  }
}