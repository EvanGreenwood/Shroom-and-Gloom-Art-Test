#region
using System.Runtime.CompilerServices;
using Unity.Mathematics;
#endregion

namespace MathBad
{
public static class SpatialCastingExt
{
  [MethodImpl(MethodImplOptions.AggressiveInlining)] public static AABB2f _AABB2F(this UnityEngine.Bounds bounds) => AABB2f.FromMinMax(bounds.min._float2(), bounds.max._float2());
}
public static class SpatialExt
{
  public const float RAYCAST_EPSILON = 0.00001f;
#region AABB2F
  /// <summary>
  /// Calculates the surface area of the aabb.
  /// </summary>
  /// <returns>The surface area of the aabb.</returns>
  public static float SurfaceArea(this AABB2f aabb) => 2f * (aabb.size.x * aabb.size.y);

  /// <summary>
  /// Calculates the volume of the aabb.
  /// </summary>
  /// <returns>The volume of the aabb.</returns>
  public static float Volume(this AABB2f aabb) => aabb.size.x * aabb.size.y;

  // contains position
  //----------------------------------------------------------------------------------------------------
  public static bool Contains(this AABB2f aabb, float x, float y) => aabb.Contains(new float2(x, y));
  public static bool Contains(this AABB2f aabb, float2 position) =>
    position.x >= aabb.min.x && position.x <= aabb.max.x &&
    position.y >= aabb.min.y && position.y <= aabb.max.y;

  public static AABB2f Encapsulate(this AABB2f aabb, float2 pos)
  {
    if(pos.x < aabb.min.x) aabb.min = new float2(pos.x, aabb.min.y);
    if(pos.x > aabb.max.x) aabb.max = new float2(pos.x, aabb.max.y);
    if(pos.y < aabb.min.y) aabb.min = new float2(pos.y, aabb.min.y);
    if(pos.y > aabb.max.y) aabb.max = new float2(pos.y, aabb.max.y);
    return aabb;
  }

  /// <summary>
  /// Calculates the closest point on an aabb to a given point.
  /// </summary>
  /// <param name="aabb">The aabb from which to find the closest point.</param>
  /// <param name="point">The point to which the closest point is calculated.</param>
  /// <returns>The closest point on the AABB to the given point.</returns>
  public static float2 ClosestPoint(this AABB2f aabb, float2 point)
  {
    float2 result = point;

    result.x = result.x < aabb.min.x ? aabb.min.x : result.x;
    result.y = result.y < aabb.min.x ? aabb.min.y : result.y;
    result.x = result.x > aabb.max.x ? aabb.max.x : result.x;
    result.y = result.y > aabb.max.x ? aabb.max.y : result.y;

    return result;
  }

  /// <summary>
  /// Checks if an aabb intersects with another aabb.
  /// </summary>
  /// <param name="aabb">The first aabb to check for intersection.</param>
  /// <param name="other">The second aabb to check for intersection.</param>
  /// <returns>True if the aabbs intersect, otherwise false.</returns>
  public static bool IntersectsAABB(this AABB2f aabb, AABB2f other) => math.abs(aabb.position.x - other.position.x) <= aabb.extents.x + other.extents.x &&
                                                                       math.abs(aabb.position.y - other.position.y) <= aabb.extents.y + other.extents.y;

  /// <summary>
  /// Checks if an aabb intersects with a sphere.
  /// </summary>
  /// <param name="aabb">The aabb to check for intersection.</param>
  /// <param name="circle">The sphere to check for intersection.</param>
  /// <returns>True if the aabb intersects with the sphere, otherwise false.</returns>
  public static bool Intersects(this AABB2f aabb, Circle2F circle) => aabb.DistSqrTo(circle.position) <= circle.radius * circle.radius;

  /// <summary>
  /// Calculates the squared distance from the aabb to a given position.
  /// </summary>
  /// <param name="position">The position to calculate the squared distance to.</param>
  /// <returns>The squared distance from the aabb to the position.</returns>
  public static float DistSqrTo(this AABB2f aabb, float2 position) => math.lengthsq(aabb.ClosestPoint(position) - aabb.position);

  /// <summary>
  /// Calculates the minimal enclosing sphere of an aabb.
  /// </summary>
  /// <param name="aabb">The aabb for which to calculate the minimal enclosing sphere.</param>
  /// <returns>The minimal enclosing sphere of the aabb.</returns>
  public static Circle2F MinimalEnclosingSphere(this AABB2f aabb) => new Circle2F(aabb.position, math.length(aabb.extents));

  /// <summary>
  /// Clips the line inside the aabb using the Liang-Barsky algorithm.
  /// </summary>
  /// <param name="p0">The starting point of the line segment. Will be modified to the new clipped starting point if there is an intersection.</param>
  /// <param name="p1">The ending point of the line segment. Will be modified to the new clipped ending point if there is an intersection.</param>
  /// <param name="min">The minimum corner.</param>
  /// <param name="max">The maximum corner.</param>
  /// <returns>True if the line segment intersects the AABB, false otherwise. If true, p0 and p1 are modified to the clipped segment.</returns>
  public static bool ClipRay(this AABB2f aabb, ref Line2F line3F)
  {
    float t0 = 0f, t1 = 1f;

    for(int axis = 0; axis < 3; axis++)
    {
      float p_delta = axis switch
                      {
                        0 => line3F.delta.x,
                        _ => line3F.delta.y
                      };
      float p_min = axis switch
                    {
                      0 => aabb.min.x,
                      _ => aabb.min.y
                    };
      float p_max = axis switch
                    {
                      0 => aabb.max.x,
                      _ => aabb.max.y,
                    };
      float p_pos = axis switch
                    {
                      0 => line3F.origin.x,
                      _ => line3F.origin.y,
                    };

      for(int j = 0; j < 2; j++)
      {
        float pi = j == 0 ? -p_delta : p_delta;
        float qi = j == 0 ? p_pos - p_min : p_max - p_pos;

        if(pi == 0f && qi < 0f)
          return false;

        float r = qi / pi;

        if(pi < 0f) t0 = math.max(t0, r);
        else t1 = math.min(t1, r);

        if(t0 > t1)
          return false;
      }
    }

    line3F.origin += t0 * line3F.delta;
    line3F.end += t1 * line3F.delta;

    return true;
  }
#endregion
#region AABB3F
  /// <summary>
  /// Calculates the surface area of the aabb.
  /// </summary>
  /// <returns>The surface area of the aabb.</returns>
  public static float SurfaceArea(this AABB3F aabb3F) => 2f * (aabb3F.size.x * aabb3F.size.y + aabb3F.size.x * aabb3F.size.z + aabb3F.size.y * aabb3F.size.z);

  /// <summary>
  /// Calculates the volume of the aabb.
  /// </summary>
  /// <returns>The volume of the aabb.</returns>
  public static float Volume(this AABB3F aabb3F) => aabb3F.size.x * aabb3F.size.y * aabb3F.size.z;

  // contains position
  //----------------------------------------------------------------------------------------------------
  public static bool Contains(this AABB3F aabb3F, float x, float y) => aabb3F.Contains(new float2(x, y));
  public static bool Contains(this AABB3F aabb3F, float2 position) =>
    position.x >= aabb3F.min.x && position.x <= aabb3F.max.x &&
    position.y >= aabb3F.min.y && position.y <= aabb3F.max.y;

  public static bool Contains(this AABB3F aabb3F, float x, float y, float z) => aabb3F.Contains(new float3(x, y, z));
  public static bool Contains(this AABB3F aabb3F, float3 position) =>
    position.x >= aabb3F.min.x && position.x <= aabb3F.max.x &&
    position.y >= aabb3F.min.y && position.y <= aabb3F.max.y &&
    position.z >= aabb3F.min.z && position.z <= aabb3F.max.z;

  /// <summary>
  /// Calculates the closest point on an aabb to a given point.
  /// </summary>
  /// <param name="aabb3F">The aabb from which to find the closest point.</param>
  /// <param name="point">The point to which the closest point is calculated.</param>
  /// <returns>The closest point on the AABB to the given point.</returns>
  public static float3 ClosestPoint(this AABB3F aabb3F, float3 point)
  {
    float3 result = point;

    result.x = result.x < aabb3F.min.x ? aabb3F.min.x : result.x;
    result.y = result.y < aabb3F.min.x ? aabb3F.min.y : result.y;
    result.z = result.z < aabb3F.min.x ? aabb3F.min.z : result.z;
    result.x = result.x > aabb3F.max.x ? aabb3F.max.x : result.x;
    result.y = result.y > aabb3F.max.x ? aabb3F.max.y : result.y;
    result.z = result.z > aabb3F.max.x ? aabb3F.max.z : result.z;

    return result;
  }

  /// <summary>
  /// Checks if an aabb intersects with another aabb.
  /// </summary>
  /// <param name="aabb3F">The first aabb to check for intersection.</param>
  /// <param name="other">The second aabb to check for intersection.</param>
  /// <returns>True if the aabbs intersect, otherwise false.</returns>
  public static bool IntersectsAABB(this AABB3F aabb3F, AABB3F other) => math.abs(aabb3F.position.x - other.position.x) <= aabb3F.extents.x + other.extents.x &&
                                                                         math.abs(aabb3F.position.y - other.position.y) <= aabb3F.extents.y + other.extents.y &&
                                                                         math.abs(aabb3F.position.z - other.position.z) <= aabb3F.extents.z + other.extents.z;

  /// <summary>
  /// Checks if an aabb intersects with a sphere.
  /// </summary>
  /// <param name="aabb3F">The aabb to check for intersection.</param>
  /// <param name="sphere">The sphere to check for intersection.</param>
  /// <returns>True if the aabb intersects with the sphere, otherwise false.</returns>
  public static bool Intersects(this AABB3F aabb3F, Sphere sphere) => aabb3F.DistSqrTo(sphere.position) <= sphere.radius * sphere.radius;

  /// <summary>
  /// Calculates the squared distance from the aabb to a given position.
  /// </summary>
  /// <param name="position">The position to calculate the squared distance to.</param>
  /// <returns>The squared distance from the aabb to the position.</returns>
  public static float DistSqrTo(this AABB3F aabb3F, float3 position) => math.lengthsq(aabb3F.ClosestPoint(position) - aabb3F.position);

  /// <summary>
  /// Calculates the minimal enclosing sphere of an aabb.
  /// </summary>
  /// <param name="aabb3F">The aabb for which to calculate the minimal enclosing sphere.</param>
  /// <returns>The minimal enclosing sphere of the aabb.</returns>
  public static Sphere MinimalEnclosingSphere(this AABB3F aabb3F) => new Sphere(aabb3F.position, math.length(aabb3F.extents));

  /// <summary>
  /// Clips the line inside the aabb using the Liang-Barsky algorithm.
  /// </summary>
  /// <param name="p0">The starting point of the line segment. Will be modified to the new clipped starting point if there is an intersection.</param>
  /// <param name="p1">The ending point of the line segment. Will be modified to the new clipped ending point if there is an intersection.</param>
  /// <param name="min">The minimum corner.</param>
  /// <param name="max">The maximum corner.</param>
  /// <returns>True if the line segment intersects the AABB, false otherwise. If true, p0 and p1 are modified to the clipped segment.</returns>
  public static bool ClipRay(this AABB3F aabb3F, ref Line3F line3F)
  {
    float t0 = 0f, t1 = 1f;

    for(int axis = 0; axis < 3; axis++)
    {
      float p_delta = axis switch
                      {
                        0 => line3F.delta.x,
                        1 => line3F.delta.y,
                        _ => line3F.delta.z
                      };
      float p_min = axis switch
                    {
                      0 => aabb3F.min.x,
                      1 => aabb3F.min.y,
                      _ => aabb3F.min.z
                    };
      float p_max = axis switch
                    {
                      0 => aabb3F.max.x,
                      1 => aabb3F.max.y,
                      _ => aabb3F.max.z
                    };
      float p_pos = axis switch
                    {
                      0 => line3F.origin.x,
                      1 => line3F.origin.y,
                      _ => line3F.origin.z
                    };

      for(int j = 0; j < 2; j++)
      {
        float pi = j == 0 ? -p_delta : p_delta;
        float qi = j == 0 ? p_pos - p_min : p_max - p_pos;

        if(pi == 0f && qi < 0f)
          return false;

        float r = qi / pi;

        if(pi < 0f) t0 = math.max(t0, r);
        else t1 = math.min(t1, r);

        if(t0 > t1)
          return false;
      }
    }

    line3F.origin += t0 * line3F.delta;
    line3F.end += t1 * line3F.delta;

    return true;
  }
#endregion
#region Line3F
  //  _     _                                                                                           
  // | |   (_) _ _   ___                                                                                
  // | |__ | || ' \ / -_)                                                                               
  // |____||_||_||_|\___|                                                                               
  //                                                                                                    
  //----------------------------------------------------------------------------------------------------
  // Ray alternative                                                                                    
  //----------------------------------------------------------------------------------------------------
  /// <summary>
  /// Determines if a line intersects with an aabb (Branchless Slab).
  /// </summary>
  /// <param name="line3F">The line to test for intersection. It should have an origin point and a direction.</param>
  /// <param name="aabb3F">The axis-aligned bounding box to test for intersection. It should have minimum and maximum points defining the box.</param>
  /// <returns>Returns true if the line intersects the AABB, false otherwise.</returns>
  public static bool IntersectsAABB(this Line3F line3F, AABB3F aabb3F)
  {
    float3 invDir = new float3(line3F.direction.x != 0.0f ? 1.0f / line3F.direction.x : float.MaxValue,
                               line3F.direction.y != 0.0f ? 1.0f / line3F.direction.y : float.MaxValue,
                               line3F.direction.z != 0.0f ? 1.0f / line3F.direction.z : float.MaxValue);

    float t1 = (aabb3F.min.x - line3F.origin.x) * invDir.x;
    float t2 = (aabb3F.max.x - line3F.origin.x) * invDir.x;
    float t_min = math.min(t1, t2);
    float t_max = math.max(t1, t2);

    for(int i = 1; i < 3; ++i)
    {
      t1 = (aabb3F.min[i] - line3F.origin[i]) * invDir[i];
      t2 = (aabb3F.max[i] - line3F.origin[i]) * invDir[i];
      t_min = math.max(t_min, math.min(t1, t2));
      t_max = math.min(t_max, math.max(t1, t2));
    }

    return t_max > 0 && t_max >= t_min;
  }

  /// <summary>
  /// Calculates the closest point on an AABB to a given line.
  /// If the line origin is inside the AABB, the method returns the line's origin.
  /// </summary>
  /// <param name="line3F">The line from which to find the closest point.</param>
  /// <param name="aabb3F">The AABB to calculate the closest point on.</param>
  /// <returns>The closest point on the AABB to the line.</returns>
  public static float3 ClosestPoint(this Line3F line3F, AABB3F aabb3F)
  {
    float3 p0 = float3.zero; // in
    bool isInside = true;

    for(int i = 0; i < 3; i++)
    {
      float tMin = (aabb3F.min[i] - line3F.origin[i]) / line3F.delta[i];
      float tMax = (aabb3F.max[i] - line3F.origin[i]) / line3F.delta[i];
      if(tMax < tMin) tMax = tMin;
      p0[i] = line3F.delta[i] * tMin;

      // Is outside the aabb
      if(line3F.origin[i] < aabb3F.min[i] || line3F.origin[i] > aabb3F.max[i])
        isInside = false;
    }

    // If the line origin is inside the aabb, return the line's origin
    return isInside ? line3F.origin : p0;
  }

  /// <summary>
  /// Tests whether a line intersects an aabb and returns the hit result.
  /// </summary>
  /// <param name="line3F">The line to perform the raycast with.</param>
  /// <param name="aabb3F">The aabb to perform the raycast against.</param>
  /// <param name="result">The hit result containing information about the intersection.</param>
  /// <returns>True if the line intersects with the aabb, otherwise false.</returns>
  public static bool GetAABBIntersection(this Line3F line3F, AABB3F aabb3F, out Hit3F result)
  {
    result = new Hit3F();

    float t0 = (aabb3F.min.x - line3F.origin.x) / (line3F.direction.x.Approx(0.0f) ? RAYCAST_EPSILON : line3F.direction.x);
    float t1 = (aabb3F.max.x - line3F.origin.x) / (line3F.direction.x.Approx(0.0f) ? RAYCAST_EPSILON : line3F.direction.x);
    float t2 = (aabb3F.min.y - line3F.origin.y) / (line3F.direction.y.Approx(0.0f) ? RAYCAST_EPSILON : line3F.direction.y);
    float t3 = (aabb3F.max.y - line3F.origin.y) / (line3F.direction.y.Approx(0.0f) ? RAYCAST_EPSILON : line3F.direction.y);
    float t4 = (aabb3F.min.z - line3F.origin.z) / (line3F.direction.z.Approx(0.0f) ? RAYCAST_EPSILON : line3F.direction.z);
    float t5 = (aabb3F.max.z - line3F.origin.z) / (line3F.direction.z.Approx(0.0f) ? RAYCAST_EPSILON : line3F.direction.z);

    float tMin = math.max(math.max(math.min(t0, t1), math.min(t2, t3)), math.min(t4, t5));
    float tMax = math.min(math.min(math.max(t0, t1), math.max(t2, t3)), math.max(t4, t5));

    // Raycast Failed
    if(tMax < 0) return false;
    if(tMin > tMax) return false;

    // // Raycast Success
    float distance = tMin;

    if(tMin < 0.0f)
      distance = tMax;

    float dist = distance;
    float3 pos = line3F.origin + line3F.direction * distance;
    float3 normal = distance switch
                    {
                      _ when distance.Approx(t0) => new float3(-1f, 0f, 0f),
                      _ when distance.Approx(t1) => new float3(1f, 0f, 0f),
                      _ when distance.Approx(t2) => new float3(0f, -1f, 0f),
                      _ when distance.Approx(t3) => new float3(0f, 1f, 0f),
                      _ when distance.Approx(t4) => new float3(0f, 0f, -1f),
                      _ when distance.Approx(t5) => new float3(0f, 0f, 1f),
                      _                          => new float3(0f, 0f, 1f)
                    };
    float3 delta = pos - line3F.origin;
    result = new Hit3F(pos, normal, delta, dist);
    return true;
  }

  public static bool Intersects2D(this Line3F l0, Line3F l1)
  {
    float2 h0 = l0.direction.xy, h1 = l1.direction.xy;
    float cross = h0.x * h1.y - h0.y * h1.x;
    return math.abs(cross) > math.EPSILON;
  }

  public static bool GetIntersection2D(this Line3F l0, Line3F l1, out float2 hit)
  {
    float a1 = l0.end.y - l0.origin.y;
    float b1 = l0.origin.x - l0.end.x;
    float c1 = a1 * l0.origin.x + b1 * l0.origin.y;

    float a2 = l1.end.y - l1.origin.y;
    float b2 = l1.origin.x - l1.end.x;
    float c2 = a2 * l1.origin.x + b2 * l1.origin.y;

    float delta = a1 * b2 - a2 * b1;

    if(math.abs(delta) < math.EPSILON)
    {
      hit = new float2();
      return false;
    }

    float x = (b2 * c1 - b1 * c2) / delta;
    float y = (a1 * c2 - a2 * c1) / delta;

    if(x < math.min(l0.origin.x, l0.end.x) || x > math.max(l0.origin.x, l0.end.x) ||
       x < math.min(l1.origin.x, l1.end.x) || x > math.max(l1.origin.x, l1.end.x) ||
       y < math.min(l0.origin.y, l0.end.y) || y > math.max(l0.origin.y, l0.end.y) ||
       y < math.min(l1.origin.y, l1.end.y) || y > math.max(l1.origin.y, l1.end.y))
    {
      hit = new float2();
      return false;
    }

    hit = new float2(x, y);
    return true;
  }
#endregion
#region OOB3F
  //   ___   ___  ___                                                                                   
  //  / _ \ | _ )| _ )                                                                                  
  // | (_) || _ \| _ \                                                                                  
  //  \___/ |___/|___/                                                                                  
  //                                                                                                    
  //----------------------------------------------------------------------------------------------------
  public static float3 TransformPoint(this OBB3F obb, float3 point) => math.mul(obb.rotation, point - obb.position);
  public static float3 InverseTransformPoint(this OBB3F obb, float3 point) => math.mul(math.inverse(obb.rotation), point - obb.position);

  /// <summary>
  /// Checks if an obb contains a given position.
  /// </summary>
  /// <param name="aabb">The obb to check for containment.</param>
  /// <param name="position">The position to check for containment.</param>
  /// <returns>True if the position is contained within the obb, otherwise false.</returns>
  public static bool Contains(this OBB3F obb, float3 point)
  {
    float3 localPoint = obb.InverseTransformPoint(point);
    return math.abs(localPoint.x) <= obb.size.x.Half() &&
           math.abs(localPoint.y) <= obb.size.y.Half() &&
           math.abs(localPoint.z) <= obb.size.z.Half();
  }

  /// <summary>
  /// Returns the closest point on the OBB (Oriented Bounding Box) to the specified point.
  /// </summary>
  /// <param name="obb">The OBB to find the closest point on.</param>
  /// <param name="point">The point to find the closest point to.</param>
  /// <returns>The closest point on the OBB to the specified point.</returns>
  public static float3 ClosestPoint(this OBB3F obb, float3 point)
  {
    float3 localPoint = obb.InverseTransformPoint(point);
    float3 localResult = math.clamp(localPoint, -obb.size / 2f, obb.size / 2f);
    return obb.TransformPoint(localResult);
  }

  /// <summary>
  /// Gets the local axes of the OBB (Oriented Bounding Box).
  /// </summary>
  /// <param name="obb">The OBB to get the local axes from.</param>
  /// <returns>An array of the local axes of the OBB.</returns>
  public static float3[] GetLocalAxes(this OBB3F obb)
  {
    return new[]
           {
             obb.rotation.right(), obb.rotation.up(),
             obb.rotation.forward(), obb.rotation.left(),
             obb.rotation.down(), obb.rotation.back()
           };
  }

  /// <summary>
  /// Checks if an obb intersects with another using SAT (Separating Axis Theorem)
  /// </summary>
  /// <param name="obb">The first OBB to check for intersection.</param>
  /// <param name="other">The second OBB to check for intersection.</param>
  /// <returns>True if the OBBs intersect, otherwise false.</returns>
  public static bool IntersectsOBB(this OBB3F obb, OBB3F other)
  {
    // All separating axes to check.
    float3[] combinedAxes = ARRAY.Combine(obb.GetLocalAxes(), other.GetLocalAxes());
    foreach(float3 axis in combinedAxes)
      if(IsSeparateAlongAxis(obb.vertices, other.vertices, axis))
        return false; // Found separation, exit early.
    return true;
    // No separation found, therefore they must be intersecting.
  }

  /// <summary>
  /// Checks if an obb axis is separated.
  /// </summary>
  /// <param name="vertsA">The vertices of the first OBB.</param>
  /// <param name="vertsB">The vertices of the second OBB.</param>
  /// <param name="axis">The axis.</param>
  /// <returns>True if the OBBs are separated, otherwise false.</returns>
  static bool IsSeparateAlongAxis(float3[] vertsA, float3[] vertsB, float3 axis)
  {
    // cross product edge case
    if(axis.Approx(0.0f))
      return false;

    float aMin = float.MaxValue, aMax = float.MinValue;
    float bMin = float.MaxValue, bMax = float.MinValue;

    float[] projectionsA = new float[8]; // Array to cache the dot product calculations.
    float[] projectionsB = new float[8];

    // First iteration:
    // calculate and cache the dot product calculations.
    for(int i = 0; i < 8; i++)
    {
      projectionsA[i] = math.dot(vertsA[i], axis);
      projectionsB[i] = math.dot(vertsB[i], axis);
    }

    // Second iteration:
    // retrieve the cached values and update min / max.
    for(int i = 0; i < 8; i++)
    {
      aMin = math.min(aMin, projectionsA[i]);
      aMax = math.max(aMax, projectionsA[i]);
      bMin = math.min(bMin, projectionsB[i]);
      bMax = math.max(bMax, projectionsB[i]);
    }

    // One - dimensional intersection test between a and b
    float longSpan = math.max(aMax, bMax) - math.min(aMin, bMin);
    float sumSpan = aMax - aMin + bMax - bMin;
    return longSpan >= sumSpan; // >= to treat touching as intersection
  }

  /// <summary>
  /// Tests if a line intersects with an obb.
  /// </summary>
  /// <param name="obb">The OBB to test for intersection.</param>
  /// <param name="line3F">The line to test for intersection.</param>
  /// <param name="result">Contains the intersection details if an intersection occurred.</param>
  /// <returns>True if the line intersects the obb, otherwise false.</returns>
  public static bool GetRayIntersection(this OBB3F obb, Line3F line3F, out Hit3F result)
  {
    float tMin = 0.0f;
    float tMax = line3F.distance;

    float3 delta = obb.position - line3F.origin;
    result = new Hit3F();

    if(!ClipAxis(obb.rotation.right(), obb.extents.x, -obb.extents.x,
                 ref tMin, ref tMax, delta, line3F.direction, RAYCAST_EPSILON)) return false;
    if(!ClipAxis(obb.rotation.up(), obb.extents.y, -obb.extents.y,
                 ref tMin, ref tMax, delta, line3F.direction, RAYCAST_EPSILON)) return false;
    if(!ClipAxis(obb.rotation.forward(), obb.extents.z, -obb.extents.z,
                 ref tMin, ref tMax, delta, line3F.direction, RAYCAST_EPSILON)) return false;

    result = new Hit3F(line3F.origin + line3F.direction * tMin,
                       CalculateNormal(result.point, obb),
                       delta,
                       tMin);
    return true;
  }

  /// <summary>
  /// Tests a line against a single axis of an oriented bounding box (OBB).
  /// </summary>
  /// <param name="axis">The axis to test.</param>
  /// <param name="min">The minimum extent along the axis.</param>
  /// <param name="max">The maximum extent along the axis.</param>
  /// <param name="tMin">The minimum parametric value along the line where an intersection can occur.</param>
  /// <param name="tMax">The maximum parametric value along the line where an intersection can occur.</param>
  /// <param name="delta">The vector from the line origin to the OBB center.</param>
  /// <param name="direction">The direction of the line.</param>
  /// <param name="threshold">The precision threshold.</param>
  /// <returns>True if the line intersects the axis, otherwise false.</returns>
  static bool ClipAxis(float3 axis, float min, float max, ref float tMin, ref float tMax,
                       float3 delta, float3 direction, float threshold)
  {
    float e = math.dot(axis, delta), f = math.dot(direction, axis);

    if(math.abs(f) > threshold)
    {
      float t1 = (e + min) / f;
      float t2 = (e + max) / f;

      if(t1 > t2)
        Swap(ref t1, ref t2);

      if(t2 < tMax) tMax = t2;
      if(t1 > tMin) tMin = t1;
      if(tMin > tMax) return false;
    }
    else
    {
      if(-e + min > 0.0 || -e + max < 0.0)
        return false;
    }

    return true;
    void Swap<T>(ref T t0, ref T t1)
    {
      T temp = t1;
      t1 = t0;
      t0 = temp;
    }
  }

  /// <summary>
  /// Calculates the normal at a point on an oriented bounding box (OBB).
  /// </summary>
  /// <param name="position">The point on the OBB.</param>
  /// <param name="obb">The OBB.</param>
  /// <returns>The normal vector at the point on the OBB.</returns>
  static float3 CalculateNormal(float3 position, OBB3F obb)
  {
    float3 localPosition = math.mul(math.inverse(obb.rotation), position - obb.position);
    float3 localNormal;

    if(math.abs(localPosition.x) > math.abs(localPosition.y))
    {
      if(math.abs(localPosition.x) > math.abs(localPosition.z))
        localNormal = math.sign(localPosition.x) * math.right();
      else localNormal = math.sign(localPosition.z) * math.forward();
    }
    else
    {
      if(math.abs(localPosition.y) > math.abs(localPosition.z))
        localNormal = math.sign(localPosition.y) * math.up();
      else localNormal = math.sign(localPosition.z) * math.forward();
    }

    return math.normalize(math.mul(obb.rotation, localNormal));
  }
#endregion
#region Sphere3F
  //  ___        _                                                                                      
  // / __| _ __ | |_   ___  _ _  ___                                                                    
  // \__ \| '_ \| ' \ / -_)| '_|/ -_)                                                                   
  // |___/| .__/|_||_|\___||_|  \___|                                                                   
  //      |_|                                                                                           
  //----------------------------------------------------------------------------------------------------

  /// <summary>
  /// Calculates the distance between the edges of two spheres.
  /// </summary>
  /// <returns>The distance between the edges of the spheres. Returns a negative value if the spheres overlap, indicating the amount of overlap.</returns>
  public static float Distance(this Sphere s0, Sphere s1)
  {
    float centerDistance = math.length(s1.position - s0.position);
    return math.max(0, centerDistance - s0.radius - s1.radius);
  }

  /// <summary>
  /// Calculates the surface area of the sphere.
  /// </summary>
  /// <returns>The surface area of the sphere.</returns>
  public static float SurfaceArea(this Sphere sphere) => 4.0f * math.PI * math.pow(sphere.radius, 2);

  /// <summary>
  /// Calculates the volume of the sphere.
  /// </summary>
  /// <returns>The volume of the sphere.</returns>
  public static float Volume(this Sphere sphere) => 4.0f / 3.0f * math.PI * math.pow(sphere.radius, 3);

  /// <summary>
  /// Grows the radius of the sphere s0, to fit the sphere s1.
  /// </summary>
  /// <returns>The volume of the sphere.</returns>
  public static Sphere Encapsulate(this Sphere s0, Sphere s1)
  {
    float dist = (s1.position - s0.position).length();
    s0.radius = dist + s1.radius;
    return s0;
  }

  /// <summary>
  /// Calculates the distance from the sphere to a given position.
  /// </summary>
  /// <param name="position">The position to calculate the distance to.</param>
  /// <returns>The distance from the sphere to the position.</returns>
  public static float DistanceTo(this Sphere sphere, float3 position) => math.length(position - sphere.position) - sphere.radius;

  /// <summary>
  /// Checks if two spheres intersect each other.
  /// </summary>
  /// <param name="s1">The other sphere to check for intersection.</param>
  /// <returns>True if the spheres intersect, otherwise false.</returns>
  public static bool Intersects(this Sphere s0, Sphere s1) => math.lengthsq(s1.position - s0.position) <= (s0.radius + s1.radius).Squared();

  /// <summary>
  /// Calculates the closest point on a sphere to a given position.
  /// </summary>
  /// <param name="sphere">The sphere from which to find the closest point.</param>
  /// <param name="position">The position to which the closest point is calculated.</param>
  /// <returns>The closest point on the sphere to the given position.</returns>
  public static float3 ClosestPoint(this Sphere sphere, float3 position) => math.normalize(position - sphere.position) * sphere.radius + sphere.position;

  /// <summary>
  /// Checks if a sphere contains a given position.
  /// </summary>
  /// <param name="sphere">The sphere to check for containment.</param>
  /// <param name="position">The position to check for containment.</param>
  /// <returns>True if the position is contained within the sphere, otherwise false.</returns>
  public static bool Contains(this Sphere sphere, float3 position) => math.lengthsq(position - sphere.position) <= sphere.radius.Squared();

  /// <summary>
  /// Checks if a sphere intersects with an AABB.
  /// </summary>
  /// <param name="sphere">The sphere to check for intersection.</param>
  /// <param name="aabb3F">The AABB to check for intersection.</param>
  /// <returns>True if the sphere intersects with the AABB, otherwise false.</returns>
  public static bool IntersectsAABB(this Sphere sphere, AABB3F aabb3F) => aabb3F.DistSqrTo(sphere.position) <= sphere.radius.Squared();

  /// <summary>
  /// Checks if a sphere intersects with an AABB and finds the intersection point and normal if there is an intersection.
  /// </summary>
  /// <param name="sphere">The sphere to test for intersection.</param>
  /// <param name="aabb3F">The AABB to test for intersection.</param>
  /// <param name="hit3F">Output parameter that contains the hit information if an intersection is found.</param>
  /// <returns>True if there is an intersection, otherwise false.</returns>
  public static bool GetAABBIntersection(this Sphere sphere, AABB3F aabb3F, out Hit3F hit3F)
  {
    float3 closest = aabb3F.ClosestPoint(sphere.position);
    float3 delta = closest - sphere.position;
    hit3F = new Hit3F();

    if(math.lengthsq(delta) < sphere.radius * sphere.radius)
    {
      float distanceToIntersection = sphere.radius - math.length(delta);
      float3 position = closest + math.normalize(delta) * distanceToIntersection;
      hit3F = new Hit3F(position,
                        math.normalize(sphere.position - position),
                        delta, distanceToIntersection);
      return true;
    }
    return false;
  }
#endregion
}
}
