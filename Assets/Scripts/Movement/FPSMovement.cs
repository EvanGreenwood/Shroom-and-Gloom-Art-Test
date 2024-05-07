using Framework;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class FPSMovement : MonoBehaviour
{
    [SerializeField] private float _speed = 4;
    [SerializeField] private float _bobHeight = 0.1f;
    [SerializeField] private float _bobRate = 0.5f;
    private float _bobCounter = 0;
    [SerializeField] private float _headHeight = 1.6f;
    [SerializeField] private float _directionLookaheadDistance = 1;
    //
    [SerializeField] private Vector2 angleOffset;
    [SerializeField] private float maxXAngle = 35;
    [SerializeField] private float maxYAngle = 35;
    
    //
    [SerializeField] private Camera _headCamera;
    //
    [SerializeField] private TunnelGenerator _tunnel;
    private Vector3 _tunnelFacingEulers = Vector3.zero;
    private float _currentSpeed = 5;
    //
    
    void Start()
    {
        transform.position = transform.position.WithY(_headHeight);
        //
        if (_tunnel == null)
        {
            FindNewTunnel();
        }
        //
        _currentSpeed = _speed;
    }

    private void FindNewTunnel()
    {
        //Slow and bad
        TunnelGenerator[] generators = FindObjectsByType<TunnelGenerator>(FindObjectsInactive.Exclude, FindObjectsSortMode.None);

        //find closest tunnel
        float closestDistance = float.MaxValue;
        TunnelGenerator closestGenerator = null;
        foreach (TunnelGenerator generator in generators)
        {
            float distance = Vector3.Distance(generator.transform.position, transform.position);

            if (distance < closestDistance)
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

    void Update()
    {
        if (Input.GetKey(KeyCode.LeftShift))
        {
            _currentSpeed = Mathf.MoveTowards(_currentSpeed, _speed * 1.8f, Time.deltaTime * _speed * 0.6f);
        }
        else
        {
            _currentSpeed = Mathf.MoveTowards(_currentSpeed, _speed  , Time.deltaTime * _speed);
        }

        float moveDir = Input.GetKey(KeyCode.W) ? 1f : Input.GetKey(KeyCode.S) ? -1f: 0;
        if (Mathf.Abs(moveDir) > 0)
        {
            if (_tunnel == null)
            {
                transform.Translate(transform.forward * Time.deltaTime * _currentSpeed * moveDir);
            }
            else
            {
                if (moveDir > 0)
                {
                    if (_tunnel.GetNormDistanceFromPoint(transform.position) > 0.99f)
                    {
                        FindNewTunnel();
                    }
                }
                else if (moveDir < 0)
                {
                    //TODO: RN, can not go back to a previous tunnel. Does not work, closest tunnel is still current.
                    /*if (_tunnel.GetNormDistanceFromPoint(transform.position) < 0.1f)
                    {
                        FindNewTunnel();
                    }*/
                }
           
                float t =_tunnel.GetClosestPositionAndDirection(transform.position, out Vector3 currentPosition, out Vector3 currentDirection, out Vector3 currentUp);
                Vector3 lookaheadPoint = _tunnel.GetLookaheadPoint(t, _directionLookaheadDistance);
                Vector3 towardsLookahead = (lookaheadPoint - currentPosition).normalized;
                transform.forward = Vector3.RotateTowards(transform.forward, towardsLookahead, Time.deltaTime*0.25f, 0);
                currentDirection = moveDir * currentDirection;
                Debug.DrawLine(currentPosition, currentPosition + currentDirection);
                transform.position = Vector3.MoveTowards(transform.position, currentPosition + currentDirection, Time.deltaTime * _currentSpeed);
            }
            
            //Moving bob
            _bobCounter += Time.deltaTime * _speed * moveDir;
            _headCamera.transform.localPosition = _headCamera.transform.localPosition.WithY(Mathf.Lerp(_headCamera.transform.localPosition.y, _headHeight + Mathf.Sin(_bobCounter / _bobRate * Mathf.PI  ) * _bobHeight, Time.deltaTime * 12));
        }
        else
        {
            //Reduce bob
            _bobCounter = Mathf.Lerp(_bobCounter, Mathf.Round(_bobCounter * _bobRate) / _bobRate, Time.deltaTime * 7);
            _headCamera.transform.localPosition = _headCamera.transform.localPosition.WithY(Mathf.Lerp(_headCamera.transform.localPosition.y, _headHeight + Mathf.Sin(_bobCounter / _bobRate * Mathf.PI  ) * _bobHeight, Time.deltaTime * 8)) ;
        }
        
        //Camera movement
        _headCamera.transform.localEulerAngles = new Vector3(((Input.mousePosition.y - Screen.height / 2f) / Screen.height / -2f) * maxXAngle, ((Input.mousePosition.x - Screen.width / 2f) / Screen.width / 2f) * maxYAngle, 0);
    }
}
