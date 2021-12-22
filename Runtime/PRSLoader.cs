using UnityEngine;
using System.Collections.Generic;
using UnityEngine.XR.ARSubsystems;
using UnityEngine.XR.Management;

namespace PleaseRemainSeated
{
  /// <summary>
  /// Manages the lifecycle of PleaseRemainSeated subsystems.
  /// </summary>
  public class PRSLoader : XRLoaderHelper
  {
    static List<XRSessionSubsystemDescriptor> s_SessionSubsystemDescriptors = new List<XRSessionSubsystemDescriptor>();
    static List<XRCameraSubsystemDescriptor> s_CameraSubsystemDescriptors = new List<XRCameraSubsystemDescriptor>();
    static List<XRPlaneSubsystemDescriptor> s_PlaneSubsystemDescriptors = new List<XRPlaneSubsystemDescriptor>();
    static List<XRRaycastSubsystemDescriptor> s_RaycastSubsystemDescriptors = new List<XRRaycastSubsystemDescriptor>();
    static List<XRAnchorSubsystemDescriptor> s_AnchorSubsystemDescriptors = new List<XRAnchorSubsystemDescriptor>();

    /// <summary>
    /// The `XRSessionSubsystem` whose lifecycle is managed by this loader.
    /// </summary>
    public XRSessionSubsystem sessionSubsystem => GetLoadedSubsystem<XRSessionSubsystem>();

    /// <summary>
    /// The `XRCameraSubsystem` whose lifecycle is managed by this loader.
    /// </summary>
    public XRCameraSubsystem cameraSubsystem => GetLoadedSubsystem<XRCameraSubsystem>();

    /// <summary>
    /// The `XRPlaneSubsystem` whose lifecycle is managed by this loader.
    /// </summary>
    public XRPlaneSubsystem planeSubsystem => GetLoadedSubsystem<XRPlaneSubsystem>();

    /// <summary>
    /// The `XRRaycastSubsystem` whose lifecycle is managed by this loader.
    /// </summary>
    public XRRaycastSubsystem raycastSubsystem => GetLoadedSubsystem<XRRaycastSubsystem>();

    /// <summary>
    /// The `XRAnchorSubsystem` whose lifecycle is managed by this loader.
    /// </summary>
    public XRAnchorSubsystem anchorSubsystem => GetLoadedSubsystem<XRAnchorSubsystem>();

    /// <summary>
    /// Initializes the loader.
    /// </summary>
    /// <returns>`True` if the session subsystem was successfully created, otherwise `false`.</returns>
    public override bool Initialize()
    {
#if UNITY_EDITOR || UNITY_STANDALONE
      CreateSubsystem<XRSessionSubsystemDescriptor, XRSessionSubsystem>(s_SessionSubsystemDescriptors, "PRS-Session");
      CreateSubsystem<XRCameraSubsystemDescriptor, XRCameraSubsystem>(s_CameraSubsystemDescriptors, "PRS-Camera");
      CreateSubsystem<XRPlaneSubsystemDescriptor, XRPlaneSubsystem>(s_PlaneSubsystemDescriptors, "PRS-Plane");
      CreateSubsystem<XRRaycastSubsystemDescriptor, XRRaycastSubsystem>(s_RaycastSubsystemDescriptors, "PRS-Raycast");
      CreateSubsystem<XRAnchorSubsystemDescriptor, XRAnchorSubsystem>(s_AnchorSubsystemDescriptors, "PRS-Anchor");

      if (sessionSubsystem == null)
      {
        Debug.LogError("[Please Remain Seated] Failed to load session subsystem.");
      }

      return sessionSubsystem != null;
#else
      return false;
#endif
    }

    /// <summary>
    /// This method does nothing. Subsystems must be started individually.
    /// </summary>
    /// <returns>Returns `true` in editor. Returns `false` otherwise.</returns>
    public override bool Start()
    {
#if UNITY_EDITOR || UNITY_STANDALONE
      return true;
#else
      return false;
#endif
    }

    /// <summary>
    /// This method does nothing. Subsystems must be stopped individually.
    /// </summary>
    /// <returns>Returns `true` in editor. Returns `false` otherwise.</returns>
    public override bool Stop()
    {
#if UNITY_EDITOR || UNITY_STANDALONE
      return true;
#else
      return false;
#endif
    }

    /// <summary>
    /// Destroys each subsystem.
    /// </summary>
    /// <returns>Always returns `true`.</returns>
    public override bool Deinitialize()
    {
#if UNITY_EDITOR || UNITY_STANDALONE
      DestroySubsystem<XRSessionSubsystem>();
      DestroySubsystem<XRCameraSubsystem>();
      DestroySubsystem<XRPlaneSubsystem>();
      DestroySubsystem<XRRaycastSubsystem>();
      DestroySubsystem<XRAnchorSubsystem>();
#endif
      return true;
    }
  }
}