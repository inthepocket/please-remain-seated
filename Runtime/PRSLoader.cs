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
    
    /// <summary>
    /// The `XRSessionSubsystem` whose lifecycle is managed by this loader.
    /// </summary>
    public XRSessionSubsystem sessionSubsystem => GetLoadedSubsystem<XRSessionSubsystem>();

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

      if (sessionSubsystem == null)
      {
        Debug.LogError("Failed to load session subsystem.");
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
      StartSubsystem<XRSessionSubsystem>();
      StartSubsystem<XRCameraSubsystem>();
      StartSubsystem<XRPlaneSubsystem>();

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
      StopSubsystem<XRSessionSubsystem>();
      StopSubsystem<XRCameraSubsystem>();
      StopSubsystem<XRPlaneSubsystem>();

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
#endif
      return true;
    }
  }
}