using Framework;
using MathBad;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.Splines;

public class PlayerMover : MonoBehaviour
{
    public Vector3 CurrentUp => _currentUp;
    private Vector3 _currentUp;
    
    [SerializeField] PlayerView _view;

    [SerializeField] float _speed = 4;
    [SerializeField] float _runMul = 1.8f;
    [SerializeField] float _headHeight = 1.6f;
    [SerializeField] float _directionLookaheadDistance = 1;

    [Header("Bob")]
    [SerializeField] float _bobHeight = 0.1f;
    [SerializeField] float _bobRate = 0.5f;

    TunnelGenerator _tunnel;

    float _bobCounter = 0;
    float _currentSpeed = 5;

    bool _runInput;
    float _fwdInput;
    bool _isMoving;

    private SplineContainer _overrideSpline;

    // Input
    //----------------------------------------------------------------------------------------------------
    public void SetInput(float fwdInput, bool runInput)
    {
        _fwdInput = fwdInput;
        _runInput = runInput;
    }
    
    public void SetTunnel(TunnelGenerator tunnel) {_tunnel = tunnel;}

    public void SetOverrideSpline(SplineContainer spline)
    {
        _overrideSpline = spline;
    }
    void Start()
    {
        INPUT.SetCursorPos(SCREEN.center);
        _view.transform.localPosition = new Vector3(0f, _headHeight, 0f);
        _currentSpeed = _speed;
    }

    void Update()
    {
        float speed = _runInput ? _speed * _runMul : _speed;
        float timeStep = Time.deltaTime * (_runInput ? _speed * 0.666f : _speed);
        _currentSpeed = Mathf.MoveTowards(_currentSpeed, speed, timeStep);
        
        _isMoving = _fwdInput.Abs() > 0;

        if(_isMoving)
        {
            if(_tunnel == null)
                transform.Translate(transform.forward * (Time.deltaTime * _currentSpeed * _fwdInput));
            else Move();
        }

        BobHead();
    }
    
    //Wrapper copied from TunnelGenerator to replicate tunnel spline behaviour
    public float GetClosestPositionAndDirection(Vector3 closestPoint, out Vector3 position, out Vector3 direction, out Vector3 up)
    {
        if (_overrideSpline == null)
        {
            return _tunnel.GetClosestPositionAndDirection(closestPoint, out position, out direction, out up);
        }
        else
        {
            float t = GetOverrideNormDistanceFromPoint(closestPoint);
            GetOverridePositionAndDirection(t, out position, out direction, out up);
            return t;
        }
        
        float GetOverrideNormDistanceFromPoint(Vector3 worldPoint)
        {
            Vector3 localPoint = transform.InverseTransformPoint(worldPoint);
            SplineUtility.GetNearestPoint(_overrideSpline.Spline, localPoint, out float3 nearest, out float t, 6, 6);
            return t;
        }
        
        void GetOverridePositionAndDirection(float t, out Vector3 position, out Vector3 direction, out Vector3 up)
        {
            _overrideSpline.Spline.Evaluate(t, out float3 vPosition, out float3 vTangent, out float3 vUp);
            position = transform.TransformPoint(vPosition);
            direction = transform.TransformDirection(vTangent.normalize());
            up = transform.TransformDirection(vUp.normalize());
        }
    }

    //Wrapper
     public Vector3 GetLookaheadPoint(float normalizedPosition, float distanceAhead)
     {
         if (_overrideSpline == null)
         {
             return _tunnel.GetLookaheadPoint(normalizedPosition, distanceAhead);
         }
         else
         {
             _overrideSpline.Spline.GetPointAtLinearDistance(normalizedPosition, distanceAhead, out float newT);
             return _overrideSpline.EvaluatePosition(newT);
         }
    }
    
    void Move()
    {
        float t = GetClosestPositionAndDirection(transform.position, out Vector3 currentPosition, out Vector3 currentDirection, out Vector3 currentUp);
        _currentUp = currentUp;
        _view.SetUp(_currentUp);
        if(float.IsNaN(currentDirection.x) || float.IsNaN(currentDirection.y) || float.IsNaN(currentDirection.z))
        {
            currentDirection = transform.forward;
        }

        Vector3 lookaheadPoint = GetLookaheadPoint(t, _directionLookaheadDistance);
        Vector3 towardsLookahead = (lookaheadPoint - currentPosition).normalized;
        
        Vector3 newForward = Vector3.RotateTowards(transform.forward, towardsLookahead, Time.deltaTime * 0.25f, 0);
        transform.rotation = Quaternion.LookRotation(newForward, currentUp);
        currentDirection = _fwdInput * currentDirection;

        Debug.DrawLine(currentPosition + towardsLookahead * 2, currentPosition + towardsLookahead * 5, Color.red);
        Debug.DrawLine(currentPosition + currentDirection * 2, currentPosition + currentDirection * 5, Color.green);


        Vector3 towardsClosest = (currentPosition - transform.position).normalized;
        //float facingVsDirectToSplineAmount = Mathf.Clamp01(Vector3.Distance(transform.position,currentPosition)*4);
        //Vector3 facingContribution  = _overrideSpline == null ? currentDirection : Vector3.Lerp(currentDirection, towardsClosest, facingVsDirectToSplineAmount);

       /* if (_overrideSpline != null)
        {
            transform.position = Vector3.MoveTowards(transform.position, currentPosition + Vector3.Slerp(towardsClosest, currentDirection, 0.5f), Time.deltaTime * _currentSpeed);
        }*/
       // else
        {
            transform.position = Vector3.MoveTowards(transform.position, currentPosition + currentDirection, Time.deltaTime * _currentSpeed);
        }
    }

    void BobHead()
    {
        if(_isMoving)
        {
            // Moving bob
            _bobCounter += Time.deltaTime * _speed * _fwdInput;
            _view.transform.localPosition = _view.transform.localPosition.WithY(Mathf.Lerp(_view.transform.localPosition.y, _headHeight + Mathf.Sin(_bobCounter / _bobRate * Mathf.PI) * _bobHeight, Time.deltaTime * 12));
        }
        else
        {
            // Reduce bob
            _bobCounter = Mathf.Lerp(_bobCounter, Mathf.Round(_bobCounter * _bobRate) / _bobRate, Time.deltaTime * 7);
            _view.transform.localPosition = _view.transform.localPosition.WithY(Mathf.Lerp(_view.transform.localPosition.y, _headHeight + Mathf.Sin(_bobCounter / _bobRate * Mathf.PI) * _bobHeight, Time.deltaTime * 8));
        }
    }

    public bool HasOverrideSpline()
    {
        return _overrideSpline != null;
    }

    public bool AtEndOfOverrideSpline()
    {
        Vector3 localPoint = _overrideSpline.transform.InverseTransformPoint(transform.position);
        SplineUtility.GetNearestPoint(_overrideSpline.Spline, localPoint, out float3 nearest, out float t, 6, 6);
        Debug.Log(t, _overrideSpline.gameObject);
        return t > 0.8f;
    }
}
