using Framework;
using System.Collections;
using System.Collections.Generic;
using MathBad;
using UnityEngine;

public class PropCoward : MonoBehaviour
{
    [SerializeField] private float _distance = 1;
    [SerializeField] private float _speed = 1.5f;
    [SerializeField] private float _triggerThreshold = 0.9f;

    private bool _leftWall = true;
    private bool _retreating = false;
    private Vector3 _startPosition = Vector3.zero;
    private float _retreatTime = 0;
    private SpriteRenderer _spriteRenderer;
    private Color _originalColor = Color.white;

    void Start()
    {
        _startPosition = transform.position;
        _spriteRenderer = GetComponent<SpriteRenderer>();
        if(_spriteRenderer != null) _originalColor = _spriteRenderer.color;
    }

    public void Setup(bool isLeftWall)
    {
        _leftWall = isLeftWall;
    }
    void Update()
    {
        if(_retreating)
        {
            _retreatTime += Time.deltaTime;
            if(_retreatTime < _distance / _speed + 0.02f)
            {
                transform.position = Vector3.MoveTowards(transform.position, _startPosition + (_leftWall ? transform.right * -_distance : transform.right * _distance), Time.deltaTime * _speed);
                transform.position = transform.position.WithY(Mathf.Lerp(transform.position.y, _startPosition.y + Mathf.Sin(Time.time * 40) * 0.06f + +Mathf.Sin(_retreatTime * 6) * 0.06f, Time.deltaTime * 35));
            }
        }
        else if(CAMERA.main != null)
        {
            Vector3 dirToCamera = (CAMERA.main.transform.position - transform.position).WithY(0).normalized;
            Vector3 right = transform.right.WithY(0).normalized;

            if(_leftWall)
            {
                float dot = Vector3.Dot(dirToCamera, right);

                if(dot > _triggerThreshold)
                {
                    _retreating = true;
                }
            }
            else
            {
                float dot = Vector3.Dot(dirToCamera, -right);
                if(dot > _triggerThreshold)
                {
                    _retreating = true;
                }
            }
        }
    }
}
