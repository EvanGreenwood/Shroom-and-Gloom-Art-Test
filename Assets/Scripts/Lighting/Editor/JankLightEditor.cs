#region Usings
using MathBad;
using MathBad_Editor;
using UnityEditor;
using UnityEngine;
#endregion

[CustomEditor(typeof(JankLight)), CanEditMultipleObjects]
public class JankLightEditor : ExtendedEditor<JankLight>
{
    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();
        if(GUILayout.Button("Get Light Color"))
        {
            target.GetLightColor();
            targets.Foreach(t => t.GetLightColor());
        }
        if(GUILayout.Button("Set Light & Flare Colors"))
        {
            target.SetColors();
            targets.Foreach(t => t.SetColors());
        }
    }
}
