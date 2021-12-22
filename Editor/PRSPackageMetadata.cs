using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEditor.XR.Management.Metadata;

namespace PleaseRemainSeated.Editor
{
  class XRPackage : IXRPackage
  {
    class PRSLoaderMetadata : IXRLoaderMetadata
    {
      public string loaderName { get; set; }
      public string loaderType { get; set; }
      public List<BuildTargetGroup> supportedBuildTargets { get; set; }
    }

    class PRSPackageMetadata : IXRPackageMetadata
    {
      public string packageName { get; set; }
      public string packageId { get; set; }
      public string settingsType { get; set; }
      public List<IXRLoaderMetadata> loaderMetadata { get; set; }
    }

    static readonly IXRPackageMetadata s_Metadata = new PRSPackageMetadata()
    {
      packageName = "Please Remain Seated XR Plugin",
      packageId = "com.inthepocket.pleaseremainseated",
      settingsType = typeof(PRSSettings).FullName,
      loaderMetadata = new List<IXRLoaderMetadata>()
            {
                new PRSLoaderMetadata()
                {
                    loaderName = "Please Remain Seated",
                    loaderType = typeof(PRSLoader).FullName,
                    supportedBuildTargets = new List<BuildTargetGroup>()
                    {
                        BuildTargetGroup.Standalone
                    }
                },
            }
    };

    public IXRPackageMetadata metadata => s_Metadata;

    public bool PopulateNewSettingsInstance(ScriptableObject obj)
    {
      if (obj is PRSSettings settings)
      {
        PRSSettings.currentSettings = settings;
        settings.requirement = PRSSettings.Requirement.Required;
        return true;
      }

      return false;
    }
  }
}
