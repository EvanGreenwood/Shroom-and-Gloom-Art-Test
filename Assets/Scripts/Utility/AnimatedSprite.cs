using Framework;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimatedSprite : MonoBehaviour
{
    [SerializeField] private Sprite[] _sprites;
    private SpriteRenderer _spriteRenderer;
    //
    private int _frame = 0;
    private float _counter = 0;
    [SerializeField] private float _frameRate = 0.016667f;
    [SerializeField] private bool _loop = false;
    [SerializeField] private bool _destroyAtEnd = true;
    [SerializeField] private bool _rotateEachFrame = false;
    void Awake()
    {
        _spriteRenderer = GetComponent<SpriteRenderer>();

        //
        if (Time.deltaTime >= _frameRate /2f)
        {
            _frame = Random.Range(0, 3);
            _spriteRenderer.sprite = _sprites[_frame % _sprites.Length];
        }
        else
        {
            _spriteRenderer.sprite = _sprites[0];
        }
    }

     
    void Update()
    {
        _counter += Time.deltaTime;
        while (_counter > _frameRate)
        {
            _counter -= _frameRate;
            _frame++;
            //
            if (_loop && _frame >= _sprites.Length) _frame -= _sprites.Length;
            //
            if (_frame < _sprites.Length)
            {
                _spriteRenderer.sprite = _sprites[_frame];
                //
                if (_rotateEachFrame)
                {
                    transform.localEulerAngles = transform.localEulerAngles.PlusZ(30 + Random.value * 15);
                }
            }
            else
            {
                if (_destroyAtEnd)
                {
                    Destroy(gameObject);
                }
                else
                {
                    enabled = false;
                }
                break;
            }
        }
    }
}
