#region Usings
using UnityEngine;
#endregion

namespace MathBad
{
public class DEBUG
{
    public static void Ray(Ray ray, float distance, Color color, float t)
    {
        Debug.DrawRay(ray.origin, ray.direction * distance, color, t);
    }

    public static void RaycastHit(RaycastHit hit, Vector3 origin, Color color, float t, float scale = 0.5f)
    {
        ArrowLine(origin, hit.point, color, t);
        ArrowLine(hit.point, hit.normal * scale, RGB.white, t);
    }
    public static void Arrow(Vector2 pos, Vector2 direction, Color color, float t)
    {
        float headLength = 0.25f;
        float headAngle = 20.0f;

        Vector3 right = Quaternion.LookRotation(direction, -Vector3.forward) * Quaternion.Euler(0, 180 + headAngle, 0) * new Vector3(0, 0, 1);
        Vector3 left = Quaternion.LookRotation(direction, -Vector3.forward) * Quaternion.Euler(0, 180 - headAngle, 0) * new Vector3(0, 0, 1);

        Debug.DrawRay(pos, direction, color, t);
        Debug.DrawRay(pos + direction, left * headLength, color, t);
        Debug.DrawRay(pos + direction, right * headLength, color, t);
    }
    public static void Arrow(Vector3 pos, Vector3 direction, Color color, float t)
    {
        float headLength = 0.25f;
        float headAngle = 20.0f;

        Vector3 right = Quaternion.LookRotation(direction) * Quaternion.Euler(0, 90 + headAngle, 0) * new Vector3(0, 0, 1);
        Vector3 left = Quaternion.LookRotation(direction) * Quaternion.Euler(0, 90 - headAngle, 0) * new Vector3(0, 0, 1);

        Debug.DrawRay(pos, direction, color, t);
        Debug.DrawRay(pos + direction, left * headLength, color, t);
        Debug.DrawRay(pos + direction, right * headLength, color, t);
    }

    public static void ArrowLine(Vector3 p0, Vector3 p1, Color color, float t)
    {
        Vector3 heading = p1 - p0;
        Arrow(p0, heading, color, t);
    }

    public static void Arrow2D(Vector2 pos, Vector2 direction, Color color, float t, float arrowHeadLength = 0.25f,
                               float arrowHeadAngle = 20.0f)
    {
        Debug.DrawRay(pos, direction, color, t);

        Vector2 right = direction.Rotate(arrowHeadAngle / 2f).normalized;
        Vector2 left = direction.Rotate(-arrowHeadAngle / 2f).normalized;
        Debug.DrawRay(pos + direction, -right * arrowHeadLength, color, t);
        Debug.DrawRay(pos + direction, -left * arrowHeadLength, color, t);
    }

    public static void Axis(Transform transform, float size, float t)
    {
        Axis(transform.position, transform.rotation, size, t);
    }
    public static void Axis(Vector3 pos, Quaternion rot, float size, float t)
    {
        Matrix4x4 m = new Matrix4x4();
        m.SetTRS(pos, rot, Vector3.one * size);

        Vector3 center = m.MultiplyPoint(new Vector3(0f, 0f, 0f));
        ArrowLine(center, m.MultiplyPoint(new Vector3(1f, 0f, 0f)), RGB.red, t);   // x
        ArrowLine(center, m.MultiplyPoint(new Vector3(0f, 1f, 0f)), RGB.green, t); // y
        ArrowLine(center, m.MultiplyPoint(new Vector3(0f, 0f, 1f)), RGB.blue, t);  // z
    }

    public static void Line(Vector3[] vertices, Color color, float t)
    {
        for(int i = 0; i < vertices.Length - 1; i++)
            Debug.DrawLine(vertices[i], vertices[i + 1], color, t);
    }

    public static void Vertices(Vector3[] vertices, float scale, Color color, float t)
    {
        for(int i = 0; i < vertices.Length; i++)
            Box(vertices[i], Quaternion.identity, Vector2.one * scale, color, t);
    }

    public static void LineLoop(Vector3[] vertices, Color color, float t)
    {
        for(int i = 0; i < vertices.Length; i++)
            Debug.DrawLine(vertices[i], vertices[(i + 1) % vertices.Length], color, t);
    }

    public static void LineArrow(Vector3[] vertices, Color color, float t)
    {
        for(int i = 0; i < vertices.Length - 1; i++)
            ArrowLine(vertices[i], vertices[i + 1], color, t);
    }

    public static void Sphere(Vector3 pos, float radius, Color color) => Sphere(pos, radius, 12, color, Time.deltaTime);
    public static void Sphere(Vector3 pos, float radius, Color color, float t) => Sphere(pos, radius, 12, color, t);

    public static void Sphere(Vector3 pos, float radius, int resolution, Color color, float t)
    {
        Circle(pos, Vector3.forward, radius, resolution, color, t);
        Circle(pos, Vector3.right, radius, resolution, color, t);
        Circle(pos, Vector3.up, radius, resolution, color, t);
    }

    public static void Circle(Vector3 pos, Vector3 normal, float radius, int resolution, Color color, float t)
    {
        for(int i = 0; i < resolution; i++)
        {
            Vector3 pos0 = MATH.GetPointOnCircle(pos, radius, normal, ((float)i / resolution) * 360f);
            Vector3 pos1 = MATH.GetPointOnCircle(pos, radius, normal, ((float)(i + 1) / resolution) * 360f);
            Debug.DrawLine(pos0, pos1, color, t);
        }
    }

    public static void Box2D(Vector2 pos, Vector2 scale, Color color, float t)
    {
        Matrix4x4 m = new Matrix4x4();
        m.SetTRS(pos, Quaternion.identity, scale._Vec3XZ());

        Vector3 point1 = m.MultiplyPoint(new Vector3(0f, 1f));
        Vector3 point2 = m.MultiplyPoint(new Vector3(1f, 1f));
        Vector3 point3 = m.MultiplyPoint(new Vector3(1f, 0f));
        Vector3 point4 = m.MultiplyPoint(new Vector3(0f, 0f));

        Debug.DrawLine(point1, point2, color, t);
        Debug.DrawLine(point2, point3, color, t);
        Debug.DrawLine(point3, point4, color, t);
        Debug.DrawLine(point4, point1, color, t);
    }

    public static void Box(Vector3 pos, Quaternion rot, Vector2 scale, Color color, float t)
    {
        Matrix4x4 m = new Matrix4x4();
        m.SetTRS(pos, rot, scale._Vec3XZ());

        Vector3 point1 = m.MultiplyPoint(new Vector3(-0.5f, 0.0f, 0.5f));
        Vector3 point2 = m.MultiplyPoint(new Vector3(0.5f, 0.0f, 0.5f));
        Vector3 point3 = m.MultiplyPoint(new Vector3(0.5f, 0.0f, -0.5f));
        Vector3 point4 = m.MultiplyPoint(new Vector3(-0.5f, 0.0f, -0.5f));

        Debug.DrawLine(point1, point2, color, t);
        Debug.DrawLine(point2, point3, color, t);
        Debug.DrawLine(point3, point4, color, t);
        Debug.DrawLine(point4, point1, color, t);
    }

    public static void CubeFromTo(Vector3 from, Vector3 to, Vector2 size, Color color, float t)
    {
        Vector3 vector = (to - from);
        float length = vector.magnitude;
        Vector3 direction = vector.normalized;
        Vector3 center = from + direction * length.Half();
        Cube(center, Quaternion.LookRotation(direction), new Vector3(size.x, size.y, length), color, t);
    }
    public static void Cube(Vector3 pos, Vector3 scale, Color color, float t) {Cube(pos, Quaternion.identity, scale, color, t);}
    public static void Cube(Vector3 pos, Quaternion rot, Vector3 scale, Color color, float t)
    {
        Matrix4x4 m = new Matrix4x4();
        m.SetTRS(pos, rot, scale);

        Vector3 point1 = m.MultiplyPoint(new Vector3(-0.5f, -0.5f, 0.5f));
        Vector3 point2 = m.MultiplyPoint(new Vector3(0.5f, -0.5f, 0.5f));
        Vector3 point3 = m.MultiplyPoint(new Vector3(0.5f, -0.5f, -0.5f));
        Vector3 point4 = m.MultiplyPoint(new Vector3(-0.5f, -0.5f, -0.5f));

        Vector3 point5 = m.MultiplyPoint(new Vector3(-0.5f, 0.5f, 0.5f));
        Vector3 point6 = m.MultiplyPoint(new Vector3(0.5f, 0.5f, 0.5f));
        Vector3 point7 = m.MultiplyPoint(new Vector3(0.5f, 0.5f, -0.5f));
        Vector3 point8 = m.MultiplyPoint(new Vector3(-0.5f, 0.5f, -0.5f));

        Debug.DrawLine(point1, point2, color, t);
        Debug.DrawLine(point2, point3, color, t);
        Debug.DrawLine(point3, point4, color, t);
        Debug.DrawLine(point4, point1, color, t);

        Debug.DrawLine(point5, point6, color, t);
        Debug.DrawLine(point6, point7, color, t);
        Debug.DrawLine(point7, point8, color, t);
        Debug.DrawLine(point8, point5, color, t);

        Debug.DrawLine(point1, point5, color, t);
        Debug.DrawLine(point2, point6, color, t);
        Debug.DrawLine(point3, point7, color, t);
        Debug.DrawLine(point4, point8, color, t);
    }

    public static void Bounds(Bounds bounds, Color color, float t)
    {
        CubeMinMax(bounds.min, bounds.max, color, t);
    }

    public static void CubeMinMax(Vector3 min, Vector3 max, Color color, float t)
    {
        Vector3 point1 = new Vector3(min.x, min.y, max.z);
        Vector3 point2 = new Vector3(max.x, min.y, max.z);
        Vector3 point3 = new Vector3(max.x, min.y, min.z);
        Vector3 point4 = new Vector3(min.x, min.y, min.z);

        Vector3 point5 = new Vector3(min.x, max.y, max.z);
        Vector3 point6 = new Vector3(max.x, max.y, max.z);
        Vector3 point7 = new Vector3(max.x, max.y, min.z);
        Vector3 point8 = new Vector3(min.x, max.y, min.z);

        Debug.DrawLine(point1, point2, color, t);
        Debug.DrawLine(point2, point3, color, t);
        Debug.DrawLine(point3, point4, color, t);
        Debug.DrawLine(point4, point1, color, t);

        Debug.DrawLine(point5, point6, color, t);
        Debug.DrawLine(point6, point7, color, t);
        Debug.DrawLine(point7, point8, color, t);
        Debug.DrawLine(point8, point5, color, t);

        Debug.DrawLine(point1, point5, color, t);
        Debug.DrawLine(point2, point6, color, t);
        Debug.DrawLine(point3, point7, color, t);
        Debug.DrawLine(point4, point8, color, t);
    }

    public static void TransformAxis(Transform transform, float size, float t)
    {
        ArrowLine(transform.transform.position, transform.transform.position + transform.transform.forward * size, Color.blue, t);
        ArrowLine(transform.transform.position, transform.transform.position + transform.transform.right * size, Color.red, t);
        ArrowLine(transform.transform.position, transform.transform.position + transform.transform.up * size, Color.green, t);
    }
}
}
