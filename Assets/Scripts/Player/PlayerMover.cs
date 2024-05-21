using Framework;
using MathBad;
using UnityEngine;

public class PlayerMover : MonoBehaviour
{
    [SerializeField] Transform _view;

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

    // Input
    //----------------------------------------------------------------------------------------------------
    public void SetInput(float fwdInput, bool runInput)
    {
        _fwdInput = fwdInput;
        _runInput = runInput;
    }
    
    public void SetTunnel(TunnelGenerator tunnel) {_tunnel = tunnel;}

    void Start()
    {
        INPUT.SetCursorPos(SCREEN.center);
        _view.localPosition = new Vector3(0f, _headHeight, 0f);
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

    void Move()
    {
        float t = _tunnel.GetClosestPositionAndDirection(transform.position, out Vector3 currentPosition, out Vector3 currentDirection, out Vector3 currentUp);

        if(float.IsNaN(currentDirection.x) || float.IsNaN(currentDirection.y) || float.IsNaN(currentDirection.z))
        {
            currentDirection = transform.forward;
        }

        Vector3 lookaheadPoint = _tunnel.GetLookaheadPoint(t, _directionLookaheadDistance);
        Vector3 towardsLookahead = (lookaheadPoint - currentPosition).normalized;
        transform.forward = Vector3.RotateTowards(transform.forward, towardsLookahead, Time.deltaTime * 0.25f, 0);
        currentDirection = _fwdInput * currentDirection;

        Debug.DrawLine(currentPosition, currentPosition + currentDirection);
        transform.position = Vector3.MoveTowards(transform.position, currentPosition + currentDirection, Time.deltaTime * _currentSpeed);
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
}
