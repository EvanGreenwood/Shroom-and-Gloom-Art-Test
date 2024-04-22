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
    void Start()
    {
        transform.position = transform.position.WithY(_headHeight);
    }


    void Update()
    {
        if (Input.GetKey(KeyCode.W))
        {
            transform.Translate(transform.forward * Time.deltaTime * _speed);
            _bobCounter += Time.deltaTime * _speed;
            // 
            transform.position = transform.position.WithY(Mathf.Lerp(transform.position.y, _headHeight + Mathf.Sin(_bobCounter / _bobRate * Mathf.PI  ) * _bobHeight, Time.deltaTime * 12));
        }
        else if (Input.GetKey(KeyCode.S))
        {
            transform.Translate(transform.forward * Time.deltaTime * -_speed);
            _bobCounter -= Time.deltaTime * _speed;
            transform.position = transform.position.WithY(Mathf.Lerp(transform.position.y, _headHeight + Mathf.Sin(_bobCounter / _bobRate * Mathf.PI  ) * _bobHeight, Time.deltaTime * 12));
        }
        else
        {
            _bobCounter = Mathf.Lerp(_bobCounter, Mathf.Round(_bobCounter * _bobRate) / _bobRate, Time.deltaTime * 7);
            transform.position = transform.position.WithY(Mathf.Lerp(transform.position.y, _headHeight + Mathf.Sign(_bobCounter / _bobRate * Mathf.PI  ) * _bobHeight, Time.deltaTime * 8));
        }
    }
}
