#region Usings
using UnityEngine;
using UnityEditor;
using MathBad;
using static MathBad_Editor.EDITOR_HELP;
#endregion

[CustomEditor(typeof(TunnelStyle))]
public class TunnelStyleEditor : Editor
{
    TunnelStyle _target;

    void OnEnable() {_target = (TunnelStyle)target;}
    void OnDisable() { }

    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();
        // Button("Add Color Key", () => { });
    }
}