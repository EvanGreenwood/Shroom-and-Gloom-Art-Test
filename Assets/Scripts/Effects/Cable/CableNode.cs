#region
using System;
using Framework;
using MathBad;
using UnityEngine;
#endregion

[Serializable]
public class CableNode
{
  public CableNode parent, child;
  public bool isRoot, isLeaf, isSheep;

  public float parentDistance => isRoot ? 0f : Vector3.Distance(position, parent.position);
  public float childDistance => isLeaf ? 0f : Vector3.Distance(position, child.position);

  public Vector3 position, lastPosition;
  public Vector3 velocity;
  public Vector3 normal, tangent;

  public float mass;
  public float restLength, stiffness;

  public bool isGrounded = false;
  readonly float _gravity;

  public CableNode(Vector3 position, float mass,float gravity, float restLength, float stiffness)
  {
    this.position = lastPosition = position;
    this.restLength = restLength;
    this.stiffness = stiffness;
    this.mass = mass;
    _gravity = gravity;
    velocity = Vector3.zero;
  }

  // Step
  public void Step(float drag, float dt)
  {
    lastPosition = position;
    if(isSheep)
    { 
      // Velocity
      velocity += new Vector3(0f, -_gravity, 0f) * dt;
      Vector3 upSpring = PHYSICS.LinearSpringForce(position, parent.position, restLength, stiffness);
      Vector3 downSpring = PHYSICS.LinearSpringForce(position, child.position, restLength, stiffness);
      velocity += Vector3.ClampMagnitude(upSpring + downSpring, 250f) / mass * dt;
      if(isGrounded && velocity.y < 0f) velocity.y = 0f;

      // Step
      position += velocity * dt;
      
      // float groundHeight = 0f;
      // if(position.y <= groundHeight + 0.1f)
      // {
      //   isGrounded = true;
      //   position.y = groundHeight + 0.1f;
      //   if(velocity.y < 0f) velocity.y = 0f;
      // }
      tangent = (child.position - position).normalized;

      // Drag
      velocity = PHYSICS.ApplyLinearDragForce(velocity, drag, dt);
    }
  }
}
