//   ___  ___  ____ __  __   ___   ___                                                                
//  / __||_ _||_  /|  \/  | / _ \ / __|                                                               
// | (_ | | |  / / | |\/| || (_) |\__ \                                                               
//  \___||___|/___||_|  |_| \___/ |___/                                                               
//                                                                                                    
//----------------------------------------------------------------------------------------------------

#region
using System;
using Mainframe;
using UnityEngine;
#endregion

public static class GIZMOS
{
    public static void GizmoColor(Color color, Action content)
    {
        Color last = Gizmos.color;
        Gizmos.color = color;
        content();
        Gizmos.color = last;
    }
    public static void GizmoMatrix(Matrix4x4 matrix, Action content)
    {
        Matrix4x4 last = Gizmos.matrix;
        Gizmos.matrix = matrix;
        content();
        Gizmos.matrix = last;
    }

    public static void Line(Vector2 p0, Vector2 p1, Color color) {GizmoColor(color, () => Gizmos.DrawLine(p0, p1));}
    public static void LineThick(Vector3 p0, Vector3 p1, float thickness, Color color)
    {
        Vector3 disp = p1 - p0;
        float dst = disp.magnitude;
        Vector3 normal = disp.normalized;

        Matrix4x4 matrix = Matrix4x4.TRS(p0 + disp.Half(), Quaternion.LookRotation(normal), new Vector3(thickness, thickness, dst));
        GizmoColor(color, () =>
                       GizmoMatrix(matrix, () =>
                       {
                           Gizmos.DrawCube(Vector3.zero, Vector3.one);
                       }));
    }
    public static void Ray(Vector3 p0, Vector2 vector, Color color) {GizmoColor(color, () => Gizmos.DrawRay(p0, vector));}
    public static void Sphere(Vector3 p0, float radius, Color color) {GizmoColor(color, () => Gizmos.DrawSphere(p0, radius));}

    public static void Circle(Vector2 p0, float radius, Color color) {GizmoColor(color, () => GizmoMatrix(Matrix4x4.Scale(Vector2.one), () => Gizmos.DrawSphere(p0, radius)));}
    public static void WireCircle(Vector2 p0, float radius, Color color) {GizmoColor(color, () => GizmoMatrix(Matrix4x4.Scale(Vector2.one), () => Gizmos.DrawWireSphere(p0, radius)));}
    public static void WireAABB(Bounds bounds, Color color) {GizmoColor(color, () => Gizmos.DrawWireCube(bounds.center, bounds.size));}
    public static void WireBox(Vector2 pos, Vector2 size, Color color) {GizmoColor(color, () => Gizmos.DrawWireCube(pos._Vec3(), size._Vec3()));}
    public static void WireBoxMinMax(Vector2 min, Vector2 max, Color color)
    {
        Vector2 size = max - min;
        Vector2 pos = min + size.Half();
        GizmoColor(color, () => Gizmos.DrawWireCube(pos, size));
    }
}
