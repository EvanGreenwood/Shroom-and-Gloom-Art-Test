#region Usings
using UnityEngine;
using UnityEditor;
using MathBad;
using MathBad_Editor;
using Unity.Mathematics;
using UnityEngine.Splines;
using static MathBad_Editor.EDITOR_HELP;
#endregion

[CustomEditor(typeof(TunnelLight))]
public class TunnelLightEditor : ExtendedEditor<TunnelLight>
{
    public override void OnInspectorGUI()
    {
        // base.OnInspectorGUI();
        EditorGUI.BeginChangeCheck();
        target.SplinePercent = EditorGUILayout.Slider("Spline Percent", target.SplinePercent, 0f, 1f);
        target.Distance = EditorGUILayout.Slider("Distance", target.Distance, 0f, 10f);
        target.Angle = EditorGUILayout.Slider("Angle", target.Angle, -180f, 180f);
        if(EditorGUI.EndChangeCheck())
        {
            Undo.RecordObject(target, "TunnelLight Transform changed");
            Step();

            EditorUtility.SetDirty(target);
        }

        return;
        void Step()
        {
            if(target.Rig == null)
                return;
            target.Rig.spline.Evaluate(target.SplinePercent, out float3 pos, out float3 tangent, out float3 up);
            float3 dir = Quaternion.AngleAxis(MATH.Normalize_360(target.Angle), tangent) * up;
            float3 nextPos = pos + dir * target.Distance;
            target.transform.rotation = Quaternion.LookRotation(tangent, up);
            target.transform.position = nextPos;
        }
    }

    void OnSceneGUI()
    {
        Tools.current = Tool.None;
        target.Rig.spline.Evaluate(target.SplinePercent, out float3 curPos, out float3 curTangent, out float3 curUp);

        EditorGUI.BeginChangeCheck();
        Handles.color = Handles.zAxisColor;
        Vector3 wishPos = Handles.Slider(target.transform.position, curTangent);
        if(EditorGUI.EndChangeCheck())
        {
            Undo.RecordObject(target, "TunnelLight position changed");
            MoveForward(wishPos);
            EditorUtility.SetDirty(target);
        }

        EditorGUI.BeginChangeCheck();
        float3 toSpline = (target.transform.position._float3() - curPos).normalizesafe();
        if(toSpline.Magnitude().Approx(0f))
        {
            Vector3 up = curUp;
            toSpline = up.Rotate(target.Angle, curTangent);
        }
        Handles.color = Handles.xAxisColor;
        Vector3 wishDstPos = Handles.Slider(target.transform.position, toSpline);
        if(EditorGUI.EndChangeCheck())
        {
            Undo.RecordObject(target, "TunnelLight distance changed");
            MoveDistance(wishDstPos);
            EditorUtility.SetDirty(target);
        }

        EditorGUI.BeginChangeCheck();
        Handles.color = Handles.zAxisColor;
        Quaternion wishRot = Handles.Disc(Quaternion.AngleAxis(target.Angle, curTangent), curPos, curTangent, target.Distance, false, 0f);
        if(EditorGUI.EndChangeCheck())
        {
            Undo.RecordObject(target, "TunnelLight angle changed");
            Rotate(wishRot);
            EditorUtility.SetDirty(target);
        }

        return;
        void MoveForward(float3 nextPos)
        {
            SplineUtility.GetNearestPoint(target.Rig.spline, new float3(0f, 0f, nextPos.z), out float3 nearest, out float t, 20, 3);
            target.Rig.spline.Evaluate(t, out float3 pos, out float3 tangent, out float3 up);
            target.SplinePercent = t;

            float3 dir = Quaternion.AngleAxis(target.Angle, tangent) * up;

            target.transform.rotation = Quaternion.LookRotation(tangent, up);
            target.transform.position = pos + dir * target.Distance;
        }
        void MoveDistance(Vector3 nextDst)
        {
            float3 curSplinePos = target.Rig.spline.EvaluatePosition(target.SplinePercent);
            float dst = (nextDst - curSplinePos._Vec3()).magnitude;
            target.Distance = dst;
            float3 dir = Quaternion.AngleAxis(target.Angle, curTangent) * curUp;
            float3 nextPos = curPos + dir * dst;

            target.transform.position = nextPos;
        }
        void Rotate(Quaternion nextRot)
        {
            nextRot.ToAngleAxis(out float angle, out Vector3 axis);
            target.Angle = angle;
            Vector3 dir = Quaternion.AngleAxis(MATH.Normalize_360(angle), curTangent) * curUp;
            target.transform.position = curPos._Vec3() + dir * target.Distance;
            target.transform.rotation = nextRot;
        }
    }
}
