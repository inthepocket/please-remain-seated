using PleaseRemainSeated.Rendering;
using UnityEngine;

namespace UnityEditor.XR.ARFoundation
{
  [CustomEditor(typeof(PRSCameraBackground))]
  class PRSCameraBackgroundEditor : Editor
  {
    public override void OnInspectorGUI()
    {
      serializedObject.Update();

      EditorGUILayout.HelpBox("Enabling the PRS Background Renderer feature adds this component to your camera, and disables the AR Background component. This is necessary for correct display of the simulated camera image.\n\nTo revert this, remove or disable the renderer feature in your renderer configuration.", MessageType.Info);

      serializedObject.ApplyModifiedProperties();
    }
  }
}