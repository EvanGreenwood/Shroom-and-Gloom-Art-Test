using Framework;
using System.Collections;
using System.Collections.Generic;
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
    [SerializeField] private TunnelGenerator _tunnel;
    private Vector3 _tunnelFacingEulers = Vector3.zero;
    //
    
    void Start()
    {
        transform.position = transform.position.WithY(_headHeight);
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
              
                transform.Translate(_tunnel.TunnelForward * Time.deltaTime * _speed, Space.World);
                transform.position = Vector3.Lerp(transform.position, _tunnel.GetClosestPoint(transform.position), Time.deltaTime * 3).WithY(transform.position.y);
            }
            _bobCounter += Time.deltaTime * _speed;
            // 
            transform.position = transform.position.WithY(Mathf.Lerp(transform.position.y, _headHeight + Mathf.Sin(_bobCounter / _bobRate * Mathf.PI  ) * _bobHeight, Time.deltaTime * 12));
        }
        else if (Input.GetKey(KeyCode.S))
        {
            if (_tunnel == null)
            {
                transform.Translate(transform.forward * Time.deltaTime * -_speed);
            }
            else
            {
                transform.Translate(_tunnel.TunnelForward * Time.deltaTime * -_speed, Space.World);
                transform.position = Vector3.Lerp(transform.position, _tunnel.GetClosestPoint(transform.position), Time.deltaTime * 3).WithY(transform.position.y);
            }
            _bobCounter -= Time.deltaTime * _speed;
            transform.position = transform.position.WithY(Mathf.Lerp(transform.position.y, _headHeight + Mathf.Sin(_bobCounter / _bobRate * Mathf.PI  ) * _bobHeight, Time.deltaTime * 12));
        }
        else
        {
            _bobCounter = Mathf.Lerp(_bobCounter, Mathf.Round(_bobCounter * _bobRate) / _bobRate, Time.deltaTime * 7);
            //
            if (_tunnel != null)
            {
                transform.position = Vector3.Lerp(transform.position, _tunnel.GetClosestPoint(transform.position), Time.deltaTime * 3).WithY(transform.position.y);
            }
            transform.position = transform.position.WithY(Mathf.Lerp(transform.position.y, _headHeight + Mathf.Sign(_bobCounter / _bobRate * Mathf.PI  ) * _bobHeight, Time.deltaTime * 8));
        }
        //
        Vector3 forwardEulers = (_tunnel != null ? Quaternion.LookRotation(_tunnel.TunnelForward, Vector3.up) : Quaternion.LookRotation(Vector3.forward)).eulerAngles;
        _tunnelFacingEulers =  new Vector3(Mathf.LerpAngle(_tunnelFacingEulers.x, forwardEulers.x, Time.deltaTime * 5), Mathf.LerpAngle(_tunnelFacingEulers.y, forwardEulers.y, Time.deltaTime * 4), Mathf.LerpAngle(_tunnelFacingEulers.z, forwardEulers.z, Time.deltaTime * 5) );
        transform.localEulerAngles = new Vector3(((Input.mousePosition.y - Screen.height / 2f) / Screen.height / -2f) * maxXAngle + baseXAngle, ((Input.mousePosition.x - Screen.width / 2f) / Screen.width / 2f) * maxYAngle + _tunnelFacingEulers.y, 0);

    }
}
