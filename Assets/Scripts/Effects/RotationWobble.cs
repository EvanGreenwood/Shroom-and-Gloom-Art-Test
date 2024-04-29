using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEditor.Experimental.AssetDatabaseExperimental.AssetDatabaseCounters;

public class RotationWobble : MonoBehaviour
{
    [SerializeField] private float _speed = 2;
    [SerializeField] private float _amount = 4;
   private Quaternion _originalLocalRotation ;
    private float _counter = 0;
    void Start()
    {
        _counter = transform.position.z;
        _originalLocalRotation = transform.localRotation;
    }

    // 
    void Update()
    {
        _counter += Time.deltaTime * _speed;
        transform.localRotation = _originalLocalRotation * Quaternion.Euler(0, 0, Mathf.Sin(_counter) * _amount);
    }
}
