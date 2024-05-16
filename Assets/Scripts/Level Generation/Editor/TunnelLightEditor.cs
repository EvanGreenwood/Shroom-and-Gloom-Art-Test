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
        target.splinePercent = EditorGUILayout.Slider("Spline Percent", target.splinePercent, 0f, 1f);
        target.distance = EditorGUILayout.Slider("Distance", target.distance, 0f, 10f);
        target.angle = EditorGUILayout.Slider("Angle", target.angle, -180f, 180f);
        if(EditorGUI.EndChangeCheck())
        {
            Undo.RecordObject(target, "TunnelLight Transform changed");
            Step();

            EditorUtility.SetDirty(target);
        }

        return;
        void Step()
        {
            if(target.rig == null)
                return;
            target.rig.spline.Evaluate(target.splinePercent, out float3 pos, out float3 tangent, out float3 up);
            float3 dir = Quaternion.AngleAxis(MATH.Normalize_360(target.angle), tangent) * up;
            float3 nextPos = pos + dir * target.distance;
            target.transform.rotation = Quaternion.LookRotation(tangent, up);
            target.transform.position = nextPos;
        }
    }

    void OnSceneGUI()
    {
        Tools.current = Tool.None;
        target.rig.spline.Evaluate(target.splinePercent, out float3 curPos, out float3 curTangent, out float3 curUp);

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
            toSpline = up.Rotate(target.angle, curTangent);
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
        Quaternion wishRot = Handles.Disc(Quaternion.AngleAxis(target.angle, curTangent), curPos, curTangent, target.distance, false, 0f);
        if(EditorGUI.EndChangeCheck())
        {
            Undo.RecordObject(target, "TunnelLight angle changed");
            Rotate(wishRot);
            EditorUtility.SetDirty(target);
        }

        return;
        void MoveForward(float3 nextPos)
        {
            SplineUtility.GetNearestPoint(target.rig.spline, new float3(0f, 0f, nextPos.z), out float3 nearest, out float t, 20, 3);
            target.rig.spline.Evaluate(t, out float3 pos, out float3 tangent, out float3 up);
            target.splinePercent = t;

            float3 dir = Quaternion.AngleAxis(target.angle, tangent) * up;

            target.transform.rotation = Quaternion.LookRotation(tangent, up);
            target.transform.position = pos + dir * target.distance;
        }
        void MoveDistance(Vector3 nextDst)
        {
            float3 curSplinePos = target.rig.spline.EvaluatePosition(target.splinePercent);
            float dst = (nextDst - curSplinePos._Vec3()).magnitude;
            target.distance = dst;
            float3 dir = Quaternion.AngleAxis(target.angle, curTangent) * curUp;
            float3 nextPos = curPos + dir * dst;

            target.transform.position = nextPos;
        }
        void Rotate(Quaternion nextRot)
        {
            nextRot.ToAngleAxis(out float angle, out Vector3 axis);
            target.angle = angle;
            Vector3 dir = Quaternion.AngleAxis(MATH.Normalize_360(angle), curTangent) * curUp;
            target.transform.position = curPos._Vec3() + dir * target.distance;
            target.transform.rotation = nextRot;
        }
    }
}
