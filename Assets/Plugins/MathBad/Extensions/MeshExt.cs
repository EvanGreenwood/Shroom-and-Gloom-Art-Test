using UnityEngine;
namespace MathBad
{
public static class MeshExt
{
    public static bool Raycast(this Mesh mesh, Ray ray, out Vector3 hitPos, out Vector3 hitNormal)
    {
        hitPos = Vector3.zero;
        hitNormal = Vector3.zero;

        float minDst = Mathf.Infinity;

        for(int i = 0; i < mesh.triangles.Length; i += 3)
        {
            Vector3 v0 = mesh.vertices[mesh.triangles[i]];
            Vector3 v1 = mesh.vertices[mesh.triangles[i + 1]];
            Vector3 v2 = mesh.vertices[mesh.triangles[i + 2]];

            if(RayIntersectsTriangle(ray, v0, v1, v2, out Vector3 hit, out float dst))
            {
                if(dst < minDst)
                {
                    minDst = dst;
                    hitPos = hit;
                    hitNormal = Vector3.Cross(v1 - v0, v2 - v0).normalized;
                    if(Vector3.Dot(ray.direction, hitNormal) > 0)
                        hitNormal = -hitNormal; // Ensure the normal is facing against the ray
                }
            }
        }

        return minDst != Mathf.Infinity;
        //--------------------------------------------------
        bool RayIntersectsTriangle(Ray r, Vector3 p0, Vector3 p1, Vector3 p2, out Vector3 hitPos, out float dst)
        {
            const float EPSILON = 1e-6f;
            hitPos = Vector3.zero;
            dst = 0;

            Vector3 edge1 = p1 - p0;
            Vector3 edge2 = p2 - p0;
            Vector3 h = Vector3.Cross(r.direction, edge2);
            float a = Vector3.Dot(edge1, h);

            if(a > -EPSILON && a < EPSILON)
                return false; // This ray is parallel to this triangle.

            float f = 1.0f / a;
            Vector3 s = r.origin - p0;
            float u = f * Vector3.Dot(s, h);

            if(u < 0.0 || u > 1.0)
                return false;

            Vector3 q = Vector3.Cross(s, edge1);
            float v = f * Vector3.Dot(r.direction, q);

            if(v < 0.0 || u + v > 1.0)
                return false;

            float t = f * Vector3.Dot(edge2, q);
            if(t > EPSILON) // ray intersection
            {
                hitPos = r.origin + r.direction * t;
                dst = t;
                return true;
            }

            return false;
        }
    }
}
}
