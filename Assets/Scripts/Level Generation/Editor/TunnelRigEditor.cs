#region Usings
using UnityEngine;
using UnityEditor;
using MathBad;
using MathBad_Editor;
using static MathBad_Editor.EDITOR_HELP;
#endregion

[CustomEditor(typeof(TunnelRig))]
public class TunnelRigEditor : ExtendedEditor<TunnelRig>
{
    void OnEnable() { }
    void OnDisable() { }

    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();
        Button("Add Light", () => { target.AddLight(); });
        Button("Add ColorNode", () => { target.AddColorNode(); });
    }
}
