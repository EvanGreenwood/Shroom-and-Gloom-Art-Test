using Framework;
using System.Collections;
using System.Collections.Generic;
using MathBad;
using Unity.VisualScripting;
using UnityEngine;

public class FPSMovement : MonoBehaviour
{
    [SerializeField] float _speed = 4;
    [SerializeField] float _bobHeight = 0.1f;
    [SerializeField] float _bobRate = 0.5f;

    [SerializeField] float _headHeight = 1.6f;
    [SerializeField] float _directionLookaheadDistance = 1;
    //
    [SerializeField] Vector2 angleOffset;
    [SerializeField] float maxXAngle = 35;
    [SerializeField] float maxYAngle = 35;

    //
    [SerializeField] Transform _view;
    //
    [SerializeField] TunnelGenerator _tunnel;

    float _bobCounter = 0;
    float _currentSpeed = 5;
    bool _isActivated;

    // Activate
    //----------------------------------------------------------------------------------------------------
    public void Activate() {_isActivated = true;}
    void Start()
    {
        transform.position = transform.position.WithY(_headHeight);
        //
        if(_tunnel == null)
        {
            FindNewTunnel();
        }
        //
        _currentSpeed = _speed;
    }

    void Update()
    {
        if(!_isActivated)
            return;
        if(INPUT.leftShift.pressed)
        {
            _currentSpeed = Mathf.MoveTowards(_currentSpeed, _speed * 1.8f, Time.deltaTime * _speed * 0.6f);
        }
        else
        {
            _currentSpeed = Mathf.MoveTowards(_currentSpeed, _speed, Time.deltaTime * _speed);
        }

        float moveDir = INPUT.moveInput2.y;

        if(Mathf.Abs(moveDir) > 0)
        {
            if(_tunnel == null)
            {
                transform.Translate(transform.forward * (Time.deltaTime * _currentSpeed * moveDir));
            }
            else
            {
                if(moveDir > 0)
                {
                    if(_tunnel.GetNormDistanceFromPoint(transform.position) > 0.99f)
                    {
                        FindNewTunnel();
                    }
                }
                else if(moveDir < 0)
                {
                    //TODO: RN, can not go back to a previous tunnel. Does not work, closest tunnel is still current.
                    /*if (_tunnel.GetNormDistanceFromPoint(transform.position) < 0.1f)
                    {
                        FindNewTunnel();
                    }*/
                }

                float t = _tunnel.GetClosestPositionAndDirection(transform.position, out Vector3 currentPosition, out Vector3 currentDirection, out Vector3 currentUp);
                Vector3 lookaheadPoint = _tunnel.GetLookaheadPoint(t, _directionLookaheadDistance);
                Vector3 towardsLookahead = (lookaheadPoint - currentPosition).normalized;
                transform.forward = Vector3.RotateTowards(transform.forward, towardsLookahead, Time.deltaTime * 0.25f, 0);
                currentDirection = moveDir * currentDirection;
                Debug.DrawLine(currentPosition, currentPosition + currentDirection);
                transform.position = Vector3.MoveTowards(transform.position, currentPosition + currentDirection, Time.deltaTime * _currentSpeed);
            }

            //Moving bob
            _bobCounter += Time.deltaTime * _speed * moveDir;
            _view.transform.localPosition = _view.transform.localPosition.WithY(Mathf.Lerp(_view.transform.localPosition.y, _headHeight + Mathf.Sin(_bobCounter / _bobRate * Mathf.PI) * _bobHeight, Time.deltaTime * 12));
        }
        else
        {
            //Reduce bob
            _bobCounter = Mathf.Lerp(_bobCounter, Mathf.Round(_bobCounter * _bobRate) / _bobRate, Time.deltaTime * 7);
            _view.transform.localPosition = _view.transform.localPosition.WithY(Mathf.Lerp(_view.transform.localPosition.y, _headHeight + Mathf.Sin(_bobCounter / _bobRate * Mathf.PI) * _bobHeight, Time.deltaTime * 8));
        }

        //Camera movement
        _view.transform.localEulerAngles = new Vector3(((Input.mousePosition.y - Screen.height / 2f) / Screen.height / -2f) * maxXAngle, ((Input.mousePosition.x - Screen.width / 2f) / Screen.width / 2f) * maxYAngle, 0);
    }

    // Get Tunnel
    //----------------------------------------------------------------------------------------------------
    void FindNewTunnel()
    {
        //Slow and bad
        TunnelGenerator[] generators = FindObjectsByType<TunnelGenerator>(FindObjectsInactive.Exclude, FindObjectsSortMode.None);

        //find closest tunnel
        float closestDistance = float.MaxValue;
        TunnelGenerator closestGenerator = null;
        foreach(TunnelGenerator generator in generators)
        {
            float distance = Vector3.Distance(generator.transform.position, transform.position);

            if(distance < closestDistance)
            {
                closestDistance = distance;
                closestGenerator = generator;
            }
        }
        _tunnel = closestGenerator;
    }
    public void SwitchTunnel(TunnelGenerator newTunnel)
    {
        _tunnel = newTunnel;
    }
}
