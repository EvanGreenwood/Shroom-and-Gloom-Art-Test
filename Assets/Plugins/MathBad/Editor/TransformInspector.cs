#region Usings
using System;
using System.Reflection;
using MathBad;
using UnityEngine;
using UnityEditor;
using static MathBad_Editor.EDITOR_HELP;
#endregion

namespace MathBad_Editor
{
[CustomEditor(typeof(Transform), true), CanEditMultipleObjects]
public class TransformInspector : ExtendedEditor<Transform>
{
    Editor _defaultEditor;

    void OnEnable()
    {
        _defaultEditor = CreateEditor(((Editor)this).targets, Type.GetType("UnityEditor.TransformInspector, UnityEditor"));
    }

    void OnDisable()
    {
        MethodInfo disableMethod = _defaultEditor.GetType().GetMethod("OnDisable", BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public);
        if(disableMethod != null)
            disableMethod.Invoke(_defaultEditor, null);
        DestroyImmediate(_defaultEditor);
    }

    public override void OnInspectorGUI()
    {
        _defaultEditor.OnInspectorGUI();

        float width = 18, height = 20;

        Rect posRect = new Rect(0, 2, width, height);
        Rect rotRect = new Rect(0, 22, width, height);
        Rect scaleRect = new Rect(0, 42, width, height);

        Button(posRect, "0", ResetPosition);
        Button(rotRect, "0", ResetRotation);
        Button(scaleRect, "1", ResetScale);
    }

    void ResetPosition()
    {
        if(targets.IsPopulated())
            targets.Foreach(t => { t.localPosition = Vector3.zero; });
        else target.localPosition = Vector3.zero;
    }
    void ResetRotation()
    {
        if(targets.IsPopulated())
            targets.Foreach(transform => { TransformUtils.SetInspectorRotation(transform, Vector3.zero); });
        else TransformUtils.SetInspectorRotation(target, Vector3.zero);
    }
    void ResetScale()
    {
        if(targets.IsPopulated())
            targets.Foreach(transform => { transform.localScale = Vector3.one; });
        else target.localScale = Vector3.one;
    }
}
}
