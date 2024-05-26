using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SlaveCamera : MonoBehaviour
{
     [SerializeField] private Camera _masterCamera;
    private Camera _thisCamera;
     IEnumerator Start()
    {
        _thisCamera = GetComponent<Camera>();
        yield return new WaitForEndOfFrame();
        _thisCamera.enabled = true;
    }

    //  
    void LateUpdate()
    {
        _thisCamera.fieldOfView = _masterCamera.fieldOfView;
        _thisCamera.nearClipPlane = _masterCamera.nearClipPlane;
        _thisCamera.farClipPlane = _masterCamera.farClipPlane;
        _thisCamera.transform.position = _masterCamera.transform.position;
        _thisCamera.transform.rotation = _masterCamera.transform.rotation;
    }
}
