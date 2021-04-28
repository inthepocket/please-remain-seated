using UnityEditor;
using UnityEngine;
using UnityEngine.XR.Management;
using System.IO;
using System.Linq;

namespace PleaseRemainSeated.Editor
{
  /// <summary>
  /// Holds settings that are used to configure the PRS XR Plugin.
  /// </summary>
  [System.Serializable]
  [XRConfigurationData("Please Remain Seated", "PleaseRemainSeated.Editor.PRSSettings")]
  public class PRSSettings : ScriptableObject
  {
    /// <summary>
    /// Enum which defines whether PRS is optional or required.
    /// </summary>
    public enum Requirement
    {
      /// <summary>
      /// The app cannot be installed on devices that do not support Please Remain Seated.
      /// </summary>
      Required,

      /// <summary>
      /// Tthe app can be installed on devices that do not support Please Remain Seated.
      /// </summary>
      Optional
    }

    [SerializeField, Tooltip("Toggles whether Please Remain Seated is required for this app. Will make app only downloadable by devices with PRS support if set to 'Required'.")]
    Requirement m_Requirement;

    /// <summary>
    /// Determines whether Please Remain Seated is required for this app: will make app only downloadable by devices with PRS support if set to <see cref="Requirement.Required"/>.
    /// </summary>
    public Requirement requirement
    {
      get { return m_Requirement; }
      set { m_Requirement = value; }
    }

    /// <summary>
    /// Gets the currently selected settings, or create a default one if no <see cref="PRSSettings"/> has been set in Player Settings.
    /// </summary>
    /// <returns>The PRS settings to use for the current Player build.</returns>
    public static PRSSettings GetOrCreateSettings()
    {
      var settings = currentSettings;
      if (settings != null)
        return settings;

      return CreateInstance<PRSSettings>();
    }

    /// <summary>
    /// Get or set the <see cref="PRSSettings"/> that will be used for the player build.
    /// </summary>
    public static PRSSettings currentSettings
    {
      get => EditorBuildSettings.TryGetConfigObject(k_SettingsKey, out PRSSettings settings) ? settings : null;

      set
      {
        if (value == null)
        {
          EditorBuildSettings.RemoveConfigObject(k_SettingsKey);
        }
        else
        {
          EditorBuildSettings.AddConfigObject(k_SettingsKey, value, true);
        }
      }
    }

    internal static bool TrySelect()
    {
      var settings = currentSettings;
      if (settings == null)
        return false;

      Selection.activeObject = settings;
      return true;
    }

    internal static SerializedObject GetSerializedSettings()
    {
      return new SerializedObject(GetOrCreateSettings());
    }

    const string k_SettingsKey = "PleaseRemainSeated.Editor.PRSSettings";
    const string k_OldConfigObjectName = "com.inthepocket.pleaseremainseated.PlayerSettings";

  }
}
