#region
using Framework;
using MathBad;
using UnityEngine;
#endregion

namespace MathBad
{
public class FreeCamera : MonoBehaviour
{
    [SerializeField] Camera _camera;
    [SerializeField] float _sensitivity = 5f;
    [Header("Move")]
    [SerializeField] float _moveSpeed = 100f;
    [SerializeField] float _moveDrag = 10f;
    [Header("Zoom")]
    [SerializeField] bool _canFov;
    [SerializeField] float _fovSpeed = 400f;
    [SerializeField] float _fovDrag = 10f;
    [SerializeField] FloatRange _fovLimits = new FloatRange(5, 120);

    Vector3 _velocity;
    float _pitch, _yaw;
    float _fovVelocity;
    float _startFov;

    // MonoBehaviour
    //----------------------------------------------------------------------------------------------------
    void Awake()
    {
        _pitch = MATH.Normalize_360(transform.eulerAngles.x);
        _yaw = MATH.Normalize_360(transform.eulerAngles.y);
        _startFov = _camera.fieldOfView;
        INPUT.mouseCaptured = true;
    }
    void Update() {Step(Time.deltaTime);}

    // Step
    //----------------------------------------------------------------------------------------------------
    void Step(float dt)
    {
        if(INPUT.tab.down) INPUT.mouseCaptured = !INPUT.mouseCaptured;
        Move(dt);
        Look(dt);
        FOV(dt);
    }

    void Move(float dt)
    {
        Vector3 input = INPUT.moveInput2.ToVector3XZ();
        if(INPUT.rightMouse.pressed) input.y = 1f;
        if(INPUT.leftShift.pressed) input.y = -1f;

        Vector3 moveForce = input.normalized * _moveSpeed;
        Vector3 dragForce = PHYSICS.GetLinearDragForce(_velocity, _moveDrag);

        _velocity += dt * (moveForce + dragForce);

        transform.position += dt * transform.TransformVector(_velocity);
    }

    void Look(float dt)
    {
        if(!INPUT.mouseCaptured)
            return;
        Vector2 look = INPUT.mouseDelta * (_sensitivity * dt);
        _pitch += -look.y;
        _yaw += look.x;

        transform.rotation = Quaternion.Euler(_pitch, _yaw, 0f);
    }

    void FOV(float dt)
    {
        if(!_canFov)
            return;
        if(INPUT.t.down)
        {
            _fovVelocity = 0f;
            _camera.fieldOfView = _startFov;
        }
        float fovForce = _fovSpeed * -INPUT.mouseScroll;
        float dragForce = PHYSICS.GetLinearDragForce(_fovVelocity, _fovDrag);
        _fovVelocity += dt * (fovForce + dragForce);
        float nextFov = _camera.fieldOfView + dt * _fovVelocity;
        _camera.fieldOfView = _fovLimits.Clamp(nextFov);
    }
}
}
