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
    //
    [SerializeField] private float maxXAngle = 35;
    [SerializeField] private float baseXAngle = -5;
    //
    [SerializeField] private float maxYAngle = 35;
    //
    [SerializeField] private Camera _headCamera;
    //
    [SerializeField] private TunnelGenerator _tunnel;
    private Vector3 _tunnelFacingEulers = Vector3.zero;
    //
    
    void Start()
    {
        transform.position = transform.position.WithY(_headHeight);
        //
        if (_tunnel == null) _tunnel = FindAnyObjectByType<TunnelGenerator>();
    }
    public void SwitchTunnel(TunnelGenerator newTunnel)
    {
        _tunnel = newTunnel;
    }

    void Update()
    {
        if (Input.GetKey(KeyCode.W))
        {
            if (_tunnel == null)
            {
                transform.Translate(transform.forward * Time.deltaTime * _speed);
            }
            else
            {
                _tunnel.GetTunnelPosittionAndDirection(_tunnel.GetTunnelProportion(transform.position), out Vector3 currentPosition, out Vector3 currentDirection);
                //
                transform.Translate(currentDirection * Time.deltaTime * _speed, Space.World);
                // 
                //
                transform.position = Vector3.Lerp(transform.position, currentPosition + currentDirection * Time.deltaTime * _speed, Time.deltaTime * 3) ;
            }
            _bobCounter += Time.deltaTime * _speed;
            // 
            _headCamera.transform.localPosition = _headCamera.transform.localPosition.WithY(Mathf.Lerp(_headCamera.transform.localPosition.y, _headHeight + Mathf.Sin(_bobCounter / _bobRate * Mathf.PI  ) * _bobHeight, Time.deltaTime * 12));
        }
        else if (Input.GetKey(KeyCode.S))
        {
            if (_tunnel == null)
            {
                transform.Translate(transform.forward * Time.deltaTime * -_speed);
            }
            else
            {
                //
                _tunnel.GetTunnelPosittionAndDirection(_tunnel.GetTunnelProportion(transform.position), out Vector3 currentPosition, out Vector3 currentDirection);
                //
                transform.Translate(currentDirection * Time.deltaTime * -_speed, Space.World); 
                //
                transform.position = Vector3.Lerp(transform.position, currentPosition - currentDirection * Time.deltaTime * _speed, Time.deltaTime * 3) ;
            }
            _bobCounter -= Time.deltaTime * _speed;
            _headCamera.transform.localPosition = _headCamera.transform.localPosition.WithY(Mathf.Lerp(_headCamera.transform.localPosition.y, _headHeight + Mathf.Sin(_bobCounter / _bobRate * Mathf.PI  ) * _bobHeight, Time.deltaTime * 12));
        }
        else
        {
            _bobCounter = Mathf.Lerp(_bobCounter, Mathf.Round(_bobCounter * _bobRate) / _bobRate, Time.deltaTime * 7);
            //
            if (_tunnel != null)
            {
              //  transform.position = Vector3.Lerp(transform.position, _tunnel.GetClosestPoint(transform.position), Time.deltaTime * 3).WithY(_tunnel.GetTunnelYHeight(transform.position)) ;
            }
            _headCamera.transform.localPosition = _headCamera.transform.localPosition.WithY(Mathf.Lerp(_headCamera.transform.localPosition.y, _headHeight + Mathf.Sin(_bobCounter / _bobRate * Mathf.PI  ) * _bobHeight, Time.deltaTime * 8)) ;
        }
        //
        if (_tunnel != null)
        {
            _tunnel.GetTunnelPosittionAndDirection(_tunnel.GetTunnelProportion(transform.position), out Vector3 currentPosition, out Vector3 currentDirection);
            Vector3 forwardEulers =  Quaternion.LookRotation(currentDirection, Vector3.up) .eulerAngles; 
            _tunnelFacingEulers = new Vector3(Mathf.LerpAngle(_tunnelFacingEulers.x, forwardEulers.x, Time.deltaTime * 5), Mathf.LerpAngle(_tunnelFacingEulers.y, forwardEulers.y, Time.deltaTime * 4), Mathf.LerpAngle(_tunnelFacingEulers.z, forwardEulers.z, Time.deltaTime * 5));
        }
        else
        {
            Vector3 forwardEulers =  Quaternion.LookRotation(Vector3.forward).eulerAngles;
            _tunnelFacingEulers = new Vector3(Mathf.LerpAngle(_tunnelFacingEulers.x, forwardEulers.x, Time.deltaTime * 5), Mathf.LerpAngle(_tunnelFacingEulers.y, forwardEulers.y, Time.deltaTime * 4), Mathf.LerpAngle(_tunnelFacingEulers.z, forwardEulers.z, Time.deltaTime * 5));
        }
        _headCamera.transform.localEulerAngles = new Vector3(_tunnelFacingEulers.x + ((Input.mousePosition.y - Screen.height / 2f) / Screen.height / -2f) * maxXAngle + baseXAngle, ((Input.mousePosition.x - Screen.width / 2f) / Screen.width / 2f) * maxYAngle + _tunnelFacingEulers.y, 0);

    }
}
