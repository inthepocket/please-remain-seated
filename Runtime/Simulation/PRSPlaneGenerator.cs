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

    /// <summary>
    /// Root transform for simulated plane objects.
    /// </summary>
    public Transform rootObject;

    public PRSPlaneGenerator(GameObject rootObject)
    {
      this.rootObject = rootObject.transform;
    }
    
    /// <summary>
    /// Generates simulated plane data from scene colliders.
    /// </summary>
    /// <param name="simulationLayer">AR simulation layer.</param>
    public List<PRSSimulatedPlane> Generate(LayerMask simulationLayer)
    { 
      var planes = Object.FindObjectsOfType<BoxCollider>()
        .SelectMany(CreatePlanesFromCollider)
        .ToList();

      var horizontalCount = planes.Count(p => p.alignment == PlaneAlignment.HorizontalUp || p.alignment == PlaneAlignment.HorizontalDown);
      var verticalCount = planes.Count(p => p.alignment == PlaneAlignment.Vertical);
      Debug.Log($"Created {planes.Count} simulated planes from BoxColliders ({horizontalCount} horizontal, {verticalCount} vertical)");
      
      return planes;
    }

    private List<PRSSimulatedPlane> CreatePlanesFromCollider(BoxCollider box)
    {
      var result = new List<PRSSimulatedPlane>();
      
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
          result.Add(CreatePlane(PlaneAlignment.HorizontalUp, face.normal, box));
        }

        // HorizontalDown
        if (Vector3.Angle(worldSpaceNormal, Vector3.down) <= MaximumFaceAngle)
        {
          result.Add(CreatePlane(PlaneAlignment.HorizontalDown, face.normal, box));
        }

        // Vertical
        if (Mathf.Abs(Vector3.Dot(Vector3.up, worldSpaceNormal)) < Mathf.Epsilon)
        {
          result.Add(CreatePlane(PlaneAlignment.Vertical, face.normal, box));
        }
      }
      
      return result;
    }

    private PRSSimulatedPlane CreatePlane(PlaneAlignment alignment, Vector3 normal, BoxCollider box)
    {
      var identifier = PRSUtils.GenerateTrackableId();
      
      var obj = new GameObject($"Plane {identifier.ToString()}");
      obj.transform.SetParent(rootObject);
      obj.hideFlags = rootObject.gameObject.hideFlags;
      obj.layer = rootObject.gameObject.layer;
      
      var plane = obj.AddComponent<PRSSimulatedPlane>();
      plane.Create(identifier, alignment, normal, GetPointsFromBoxColliderFace(box, normal));

      return plane;
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
      
      // ReSharper disable once AssignNullToNotNullAttribute
      return localPts
        .Select(p => box.gameObject.transform.TransformPoint(box.center + p))
        .ToList();
    }
  }
}