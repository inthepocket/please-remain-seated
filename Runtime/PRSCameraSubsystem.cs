using System;
using System.Runtime.InteropServices;
using Unity.Collections;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Scripting;
using UnityEngine.XR.ARSubsystems;
using PleaseRemainSeated.Simulation;

namespace PleaseRemainSeated
{
  /// <summary>
  /// The camera subsystem implementation for Please Remain Seated.
  /// </summary>
  [Preserve]
  public sealed class PRSCameraSubsystem : XRCameraSubsystem
  {
    /// <summary>
    /// The name for the shader for rendering the camera texture.
    /// </summary>
    /// <value>
    /// The name for the shader for rendering the camera texture.
    /// </value>
    const string k_BackgroundShaderName = "Unlit/PRSBackground";

    /// <summary>
    /// Create and register the camera subsystem descriptor to advertise a77 providing implementation for camera
    /// functionality.
    /// </summary>
    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.SubsystemRegistration)]
    static void Register()
    {
#if UNITY_EDITOR || UNITY_STANDALONE
      XRCameraSubsystemCinfo cameraSubsystemCinfo = new XRCameraSubsystemCinfo
      {
        id = "PRS-Camera",
        providerType = typeof(PRSProvider),
        subsystemTypeOverride = typeof(PRSCameraSubsystem),
        supportsCameraImage = false,
        supportsProjectionMatrix = true,
        supportsAverageBrightness = false,
        supportsAverageColorTemperature = false,
        supportsColorCorrection = false,
        supportsDisplayMatrix = false,
        supportsTimestamp = false,
        supportsCameraConfigurations = false,
        supportsAverageIntensityInLumens = false,
        supportsFocusModes = false,
        supportsFaceTrackingAmbientIntensityLightEstimation = false,
        supportsFaceTrackingHDRLightEstimation = false,
        supportsWorldTrackingAmbientIntensityLightEstimation = false,
        supportsWorldTrackingHDRLightEstimation = false,
        supportsCameraGrain = false
      };

      if (!XRCameraSubsystem.Register(cameraSubsystemCinfo))
      {
        Debug.LogError("Cannot register the camera subsystem.");
      }
#endif
    }

    /// <summary>
    /// Provides the camera functionality for the Please Remain Seated implementation.
    /// </summary>
    class PRSProvider : Provider
    {
      /// <summary>
      /// Get the material used by <c>XRCameraSubsystem</c> to render the camera texture.
      /// </summary>
      /// <returns>
      /// The material to render the camera texture.
      /// </returns>
      public override Material cameraMaterial => m_CameraMaterial;

      Material m_CameraMaterial;

      private int _backgroundShaderTexturePropertyID;

      /// <summary>
      /// Whether camera permission has been granted.
      /// </summary>
      /// <value>
      /// <c>true</c> if camera permission has been granted for this app. Otherwise, <c>false</c>.
      /// </value>
      public override bool permissionGranted => SimulationAPI.IsCameraPermissionGranted();

      // ^ TODO: simulate permissions?

      /// <summary>
      /// Constructs the Please Remain Seated camera functionality provider.
      /// </summary>
      public PRSProvider()
      {
        m_CameraMaterial = CreateCameraMaterial(k_BackgroundShaderName);
      }

      /// <summary>
      /// Get the currently active camera or set the requested camera.
      /// </summary>
      public override Feature requestedCamera
      {
        get => SimulationAPI.GetCameraFeatures();
      }

      public override Feature currentCamera => requestedCamera;

      /// <summary>
      /// Start the camera functionality.
      /// </summary>
      public override void Start()
      {
        _backgroundShaderTexturePropertyID = Shader.PropertyToID("_MainTex");

        SimulationAPI.StartCamera();
      }

      /// <summary>
      /// Stop the camera functionality.
      /// </summary>
      public override void Stop()
      {
        SimulationAPI.StopCamera();
      }

      /// <summary>
      /// Get the current camera frame for the subsystem.
      /// </summary>
      /// <param name="cameraParams">The current Unity <c>Camera</c> parameters.</param>
      /// <param name="cameraFrame">The current camera frame returned by the method.</param>
      /// <returns>
      /// <c>true</c> if the method successfully got a frame. Otherwise, <c>false</c>.
      /// </returns>
      public override bool TryGetFrame(XRCameraParams cameraParams, out XRCameraFrame cameraFrame)
      {
        return SimulationAPI.TryGetFrame(cameraParams, out cameraFrame);
      }

      /// <summary>
      /// Get the camera intrinsics information.
      /// </summary>
      /// <param name="cameraIntrinsics">The camera intrinsics information returned from the method.</param>
      /// <returns>
      /// <c>true</c> if the method successfully gets the camera intrinsics information. Otherwise, <c>false</c>.
      /// </returns>
      public override bool TryGetIntrinsics(out XRCameraIntrinsics cameraIntrinsics)
      {
        return SimulationAPI.TryGetIntrinsics(out cameraIntrinsics);
      }

      /// <summary>
      /// Gets the texture descriptors associated with the current camera frame.
      /// </summary>
      /// <param name="defaultDescriptor">Default descriptor.</param>
      /// <param name="allocator">Allocator.</param>
      /// <returns>The texture descriptors.</returns>
      public override NativeArray<XRTextureDescriptor> GetTextureDescriptors(
        XRTextureDescriptor defaultDescriptor,
        Allocator allocator)
      {
        return SimulationAPI.GetTextureDescriptors(_backgroundShaderTexturePropertyID, allocator);
      }
    }

    /// <summary>
    /// Interface between the camera subsystem and the AR simulation.
    /// </summary>
    static class SimulationAPI
    {
      // Equivalent of <c>XRCameraFrame</c>.
      [StructLayout(LayoutKind.Sequential)]
      private struct XRCameraFrameData
      {
        public long timestampNs;
        public float averageBrightness;
        public float averageColorTemperature;
        public Color colorCorrection;
        public Matrix4x4 projectionMatrix;
        public Matrix4x4 displayMatrix;
        public TrackingState trackingState;
        public IntPtr nativePtr;
        public XRCameraFrameProperties properties;
        public float averageIntensityInLumens;
        public double exposureDuration;
        public float exposureOffset;
        public float mainLightIntensityLumens;
        public Color mainLightColor;
        public Vector3 mainLightDirection;
        public SphericalHarmonicsL2 ambientSphericalHarmonics;
        public XRTextureDescriptor cameraGrain;
        public float noiseIntensity;
      }

      // Equivalent of <c>XRTextureDescriptor</c>.
      [StructLayout(LayoutKind.Sequential)]
      private struct XRTextureDescriptorData
      {
        public IntPtr nativeTexturePtr;
        public int width;
        public int height;
        public int mipMapCount;
        public TextureFormat textureFormat;
        public int propertyNameID;
        public int depth;
        public TextureDimension dimension;
      }

      public static bool IsCameraPermissionGranted()
      {
        // TODO: simulate?
        return true;
      }

      public static Feature GetCameraFeatures()
      {
        return Feature.WorldFacingCamera;
      }

      public static void StartCamera()
      {
      }

      public static void StopCamera()
      {
      }

      public static bool TryGetFrame(XRCameraParams cameraParams, out XRCameraFrame cameraFrame)
      {
        var data = new XRCameraFrameData();
        data.projectionMatrix = PRSSimulation.instance.device.projectionMatrix;
        data.timestampNs = (long) (Time.realtimeSinceStartup * 1e+9);
        data.properties = XRCameraFrameProperties.ProjectionMatrix | XRCameraFrameProperties.Timestamp;
        cameraFrame = PRSUtils.CopyData<XRCameraFrame>(data);

        return true;
      }

      public static bool TryGetIntrinsics(out XRCameraIntrinsics cameraIntrinsics)
      {
        cameraIntrinsics = default(XRCameraIntrinsics);
        return false;
      }

      internal static NativeArray<XRTextureDescriptor> GetTextureDescriptors(int shaderPropertyID, Allocator allocator)
      {
        var texture = PRSSimulation.instance.device.targetTexture;

        var data = new XRTextureDescriptorData();
        data.nativeTexturePtr = texture.GetNativeTexturePtr();
        data.width = texture.width;
        data.height = texture.height;
        data.textureFormat = TextureFormat.RGB24;
        data.mipMapCount = 0;
        data.propertyNameID = shaderPropertyID;
        data.depth = 1;
        data.dimension = TextureDimension.Tex2D;

        var descriptor = PRSUtils.CopyData<XRTextureDescriptor>(data);
        var array = new[] {descriptor}; // TODO: cache?
        return new NativeArray<XRTextureDescriptor>(array, allocator);
      }
    }
  }
}