//  ___  _  _ __   __ ___  ___   ___  ___                                                             
// | _ \| || |\ \ / // __||_ _| / __|/ __|                                                            
// |  _/| __ | \ V / \__ \ | | | (__ \__ \                                                            
// |_|  |_||_|  |_|  |___/|___| \___||___/                                                            
//                                                                                                    
//----------------------------------------------------------------------------------------------------

#region
using UnityEngine;
using static Unity.Mathematics.math;
#endregion
namespace MathBad
{
public static class RigidbodyExt
{
  public static void RotateTowardsDirection(this Rigidbody2D rb, Vector2 dir, float deltaTime)
  {
    if(dir == Vector2.zero)
      return;

    rb.rotation = Mathf.LerpAngle(rb.rotation, dir.ToAngle(), deltaTime);
  }
}

public static class PHYSICS
{
  static PHYSICS()
  {
    raycastHits2D = new RaycastHit2D[1];
    colliderHits2D = new Collider2D[100];
  }

  // Raycast
  //----------------------------------------------------------------------------------------------------
  public static RaycastHit2D[] raycastHits2D;
  public static Collider2D[] colliderHits2D;

  public static int Raycast2D(Vector2 origin, Vector2 direction, float maxDistance, int hitMask) => Physics2D.RaycastNonAlloc(origin, direction, raycastHits2D, maxDistance, hitMask);
  public static int OverlapCircle(Vector2 origin, float radius, int hitMask) => Physics2D.OverlapCircleNonAlloc(origin, radius, colliderHits2D, hitMask);

  public static Vector2 GetVelocityAtPoint(Vector2 pos, Vector2 v, float av, Vector2 com)
  {
    Vector2 r = pos - com;
    float x = -av * Mathf.Deg2Rad * r.y, y = av * Mathf.Deg2Rad * r.x;
    Vector2 perpVel = new Vector2(x, y);
    return v + perpVel;
  }

  // Drag
  //----------------------------------------------------------------------------------------------------
  public static float GetLinearDragForce(float x, float drag) => -drag * x;
  public static Vector2 GetLinearDragForce(Vector2 v, float drag) => -drag * v;
  public static Vector3 GetLinearDragForce(Vector3 v, float drag) => -drag * v;

  public static float GetQuadraticDragForce(float x, float drag) => -drag * x * x;
  public static Vector2 GetQuadraticDragForce(Vector2 v, float drag) => -drag * v.magnitude * v;
  public static Vector3 GetQuadraticDragForce(Vector3 v, float drag) => -drag * v.magnitude * v;

  public static float ApplyLinearDragForce(float x, float drag, float deltaTime) => x + GetLinearDragForce(x, drag) * deltaTime;
  public static Vector2 ApplyLinearDragForce(Vector2 v, float drag, float deltaTime) => v + GetLinearDragForce(v, drag) * deltaTime;
  public static Vector3 ApplyLinearDragForce(Vector3 v, float drag, float deltaTime) => v + GetLinearDragForce(v, drag) * deltaTime;

  public static float ApplyQuadraticDragForce(float x, float drag, float deltaTime) => x + GetQuadraticDragForce(x, drag) * deltaTime;
  public static Vector2 ApplyQuadraticDragForce(Vector2 v, float drag, float deltaTime) => v + GetQuadraticDragForce(v, drag) * deltaTime;
  public static Vector3 ApplyQuadraticDragForce(Vector3 v, float drag, float deltaTime) => v + GetQuadraticDragForce(v, drag) * deltaTime;

  // Spring
  //----------------------------------------------------------------------------------------------------
  public static float LinearSpringForce(float p0, float p1, float restLength, float stiffness) => Mathf.Sign(p0 - p1) * (-stiffness * max(0, (p0 - p1) - restLength));
  public static Vector2 LinearSpringForce(Vector2 p0, Vector2 p1, float restLength, float stiffness) => (p0 - p1).normalized * (-stiffness * max(0, (p0 - p1).magnitude - restLength));
  public static Vector3 LinearSpringForce(Vector3 p0, Vector3 p1, float restLength, float stiffness) => (p0 - p1).normalized * (-stiffness * max(0, (p0 - p1).magnitude - restLength));
}
}
