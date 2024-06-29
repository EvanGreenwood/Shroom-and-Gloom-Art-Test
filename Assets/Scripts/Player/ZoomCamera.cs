using Framework;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ZoomCamera : SingletonBehaviour<ZoomCamera>
{
    private bool _zooming = false;
    private Transform _zoomTransform;
    private Vector3 _localPositionStart;
    [SerializeField] private Camera[] _cameras;
    private float _initialFOV;
    void Start()
    {
        _localPositionStart = transform.localPosition;
        _initialFOV = _cameras[0].fieldOfView;
        enabled = false;
    }
    //
    public void ZoomIn(Transform zoomTransform)
    {
        _initialFOV = _cameras[0].fieldOfView;
        _zoomTransform = zoomTransform;
        _zooming = true;
        _localPositionStart = transform.localPosition;
        enabled = true;
        GetComponent<PlayerView>().enabled = false;
    }
    public void Release()
    {
        _zooming = false;
    }
    //  
    void Update()
    {
        if (_zooming)
        {
            transform.position = Vector3.MoveTowards(transform.position, _zoomTransform.position, Time.deltaTime * 0.5f);
            transform.position = Vector3.Lerp(transform.position, _zoomTransform.position, Time.deltaTime * 2);
            transform.rotation = Quaternion.Lerp(transform.rotation, _zoomTransform.rotation, Time.deltaTime * 6);
            foreach (Camera cam in _cameras)
            {
                cam.fieldOfView = Mathf.Lerp(cam.fieldOfView, _initialFOV * 0.7f, Time.deltaTime   * 0.3f);
                cam.fieldOfView = Mathf.MoveTowards(cam.fieldOfView, _initialFOV * 0.6f, Time.deltaTime * _initialFOV * 0.4f);
            }
        }
        else
        {
            transform.localPosition = Vector3.MoveTowards(transform.localPosition, _localPositionStart, Time.deltaTime * 0.5f);
            transform.localPosition = Vector3.Lerp(transform.localPosition, _localPositionStart, Time.deltaTime * 2);
            transform.localRotation = Quaternion.Lerp(transform.localRotation, Quaternion.identity, Time.deltaTime * 4);
            //
            foreach (Camera cam in _cameras)
            {
                cam.fieldOfView = Mathf.Lerp(cam.fieldOfView, _initialFOV , Time.deltaTime   * 0.6f);
                cam.fieldOfView = Mathf.MoveTowards(cam.fieldOfView, _initialFOV  , Time.deltaTime * _initialFOV * 0.6f);
            }
            //
            if ((transform.localPosition - _localPositionStart).magnitude < 0.004f)
            {
                transform.localPosition = _localPositionStart;
                transform.localRotation =   Quaternion.identity ;
                GetComponent<PlayerView>().enabled = true;
                enabled = false;
                foreach (Camera cam in _cameras)
                {
                    cam.fieldOfView =   _initialFOV ;
                }
            }
        }
    }
}
