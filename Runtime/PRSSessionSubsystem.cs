using UnityEngine;
using UnityEngine.Scripting;
using UnityEngine.XR.ARSubsystems;

namespace PleaseRemainSeated
{
  /// <summary>
  /// Please Remain Seated implementation of the <c>XRSessionSubsystem</c>. Do not create this directly. Use the <c>SubsystemManager</c> instead.
  /// </summary>
  [Preserve]
  public sealed class PRSSessionSubsystem : XRSessionSubsystem
  {
    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.SubsystemRegistration)]
    private static void RegisterDescriptor()
    {
#if UNITY_EDITOR || UNITY_STANDALONE
      XRSessionSubsystemDescriptor.RegisterDescriptor(new XRSessionSubsystemDescriptor.Cinfo
      {
        id = "PRS-Session",
        providerType = typeof(PRSProvider),
        subsystemTypeOverride = typeof(PRSSessionSubsystem),
        supportsMatchFrameRate = true,
        supportsInstall = false
      });
#endif
    }

    // ReSharper disable once UnusedAutoPropertyAccessor.Global
    public static bool IsRunning { get; private set; }

    protected override void OnCreate()
    {
    }

    private class PRSProvider : Provider
    {
      private bool hasStarted;

      public override void Start()
      {
        base.Start();

        IsRunning = true;

        if (!Application.isPlaying || hasStarted) return;
        hasStarted = true;
      }

      public override void Reset()
      {
        base.Reset();

        if (running)
        {
          Start();
        }
      }

      public override void Stop()
      {
        base.Stop();
        IsRunning = false;
      }

      public override void Update(XRSessionUpdateParams updateParams)
      {
      }

      public override void Destroy()
      {
        base.Destroy();
        IsRunning = false;
      }

      public override void OnApplicationPause()
      {
      }

      public override void OnApplicationResume()
      {
      }

      public override Promise<SessionAvailability> GetAvailabilityAsync()
      {
        return Promise<SessionAvailability>.CreateResolvedPromise(SessionAvailability.Installed | SessionAvailability.Supported);
      }

      public override Promise<SessionInstallationStatus> InstallAsync()
      {
        return Promise<SessionInstallationStatus>.CreateResolvedPromise(SessionInstallationStatus.Success);
      }

      public override TrackingState trackingState => TrackingState.Tracking;

      public override NotTrackingReason notTrackingReason => NotTrackingReason.None;

      public override int frameRate => 60;

      private bool _matchFrameRate;
      public override bool matchFrameRateEnabled => _matchFrameRate;

      public override bool matchFrameRateRequested
      {
        get => matchFrameRateEnabled;
        set => _matchFrameRate = value;
      }
    }
  }
}