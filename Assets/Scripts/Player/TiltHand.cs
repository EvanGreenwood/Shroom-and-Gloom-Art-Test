using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TiltHand : MonoBehaviour
{
    [SerializeField] private float _xTiltBase = 15;
    [SerializeField] private float _xTiltM = -1;
    [SerializeField] private float _yShiftM = 1;
    private Vector3 _startingLocalPosition;
    void Start()
    {
        _startingLocalPosition = transform.localPosition;
    }

    //  
    void Update()
    {
        float mouseYM = (( Input.mousePosition.y - Screen.height/2f) * 2 / Screen.height);
        transform.localEulerAngles = new Vector3(_xTiltBase + mouseYM * _xTiltM, 0, 0);
        transform.localPosition = _startingLocalPosition + Vector3.up * (_yShiftM * mouseYM);
    }
}
