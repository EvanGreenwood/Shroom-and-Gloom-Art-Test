//  ___             _  _           _          _               _                                       
// / __| _ __  _ _ (_)| |_  ___   /_\   _ _  (_) _ __   __ _ | |_  ___  _ _                           
// \__ \| '_ \| '_|| ||  _|/ -_) / _ \ | ' \ | || '  \ / _` ||  _|/ _ \| '_|                          
// |___/| .__/|_|  |_| \__|\___|/_/ \_\|_||_||_||_|_|_|\__,_| \__|\___/|_|                            
//      |_|                                                                                           
//----------------------------------------------------------------------------------------------------

#region
using System;
using NaughtyAttributes;
using UnityEngine;
using UnityEngine.UI;
#endregion

namespace MathBad
{
public interface ISpriteListener
{
    public void OnFrameEnd(Sprite sprite);
}
public class SpriteAnimator : MonoBehaviour
{
    public const float FRAMES_PER_SECOND = 20;

    [SerializeField] SpriteBank _bank;
    [SerializeField] LoopType _loopType;
    [SerializeField] float _speed = 1;
    [SerializeField] bool _colorOverTime;
    [SerializeField] Gradient _colorOverTimeGradient;

    [SerializeField] bool _unscaledTime = false;
    [SerializeField] bool _randomStartTime = false;

    [SerializeField] bool _startPaused = false;
    [SerializeField] bool _destroyOnComplete = false;

    SpriteRenderer _sr;
    Image _image;
    ISpriteListener[] _listeners;

    int _currentFrameIndex;
    Timer2 _timer;

    // state
    // -----
    [SerializeField, ReadOnly] float _frameDelta, _totalTime;

    bool _isPaused = false, _isComplete = false;
    bool _hasInit = false;

    // public properties
    public float frameTime => _currentFrameIndex * _frameDelta;
    public float totalTime => _totalTime;

    public SpriteBank bank => _bank;
    public SpriteRenderer sr => _sr;

    public delegate void SpriteAnimHandler();
    public event SpriteAnimHandler onComplete;
    protected virtual void OnComplete() => onComplete?.Invoke();

    // init
    //----------------------------------------------------------------------------------------------------//
    public void Init()
    {
        ValidateComponents();
        Init(_bank, _sr != null ? _sr.color : _image != null ? _image.color : RGB.white);
    }
    public void Init(SpriteBank bank) {Init(bank, RGB.white);}
    public void Init(SpriteBank bank, Color color)
    {
        _listeners = GetComponents<ISpriteListener>();

        ValidateComponents();
        SetColor(color);
        SetBank(bank);

        if(_startPaused)
            _isPaused = true;

        _hasInit = true;
    }

    void ComputeFrameDelta()
    {
        _frameDelta = 1f / FRAMES_PER_SECOND / _speed;
        _totalTime = _bank.bank.Length * _frameDelta;
    }

    public void ValidateComponents()
    {
        _sr = GetComponent<SpriteRenderer>();
        if(_sr == null)
        {
            _image = GetComponent<Image>();
            if(_image == null)
            {
                _sr = gameObject.AddComponent<SpriteRenderer>();
            }
        }
    }

    public void SetColor(Color color)
    {
        if(_sr != null) _sr.color = color;
        else if(_image != null) _image.color = color;
    }

    // change bank
    //----------------------------------------------------------------------------------------------------//
    public void SetBank(SpriteBank bank)
    {
        _bank = bank;

        _currentFrameIndex = _randomStartTime ? RNG.Int(0, _bank.bank.Length) : 0;

        for(int i = 0; i < _bank.bank.Length; i++)
        {
            _bank.bank[i] = _bank.bank[i];
        }

        ComputeFrameDelta();

        if(_timer == null) _timer = new Timer2(_bank.bank.Length * _frameDelta);
        else _timer.Reset(_bank.bank.Length * _frameDelta);

        _timer.Step(frameTime);
    }

    // monobehaviour
    //----------------------------------------------------------------------------------------------------//
    void OnValidate()
    {
        if(!_bank)
        {
            _frameDelta = _totalTime = 0f;
        }
        else ComputeFrameDelta();
    }
    void Start()
    {
        if(!_hasInit) Init();
    }
    void Update()
    {
        if(!_hasInit || _isComplete || _isPaused)
            return;

        Step(_unscaledTime ? Time.unscaledDeltaTime : Time.deltaTime);
    }

    // step
    //--------------------------------------------------------------------------------------------------//
    void Step(float deltaTime)
    {
        _timer.Step(deltaTime * _speed);

        _isComplete = _currentFrameIndex == _bank.bank.Length;
        if(_isComplete)
        {
            switch(_loopType)
            {
                case LoopType.None:
                    OnComplete();
                    if(_destroyOnComplete) Destroy(gameObject);
                    return;
                case LoopType.Loop:
                    Restart();
                    break;
                // case LoopType.Reverse:
                //   _bank.bank.Reverse();
                //   Restart();
                //   break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        if(_timer.value >= frameTime)
        {
            SetSprite();
            _listeners.Foreach(listener => listener.OnFrameEnd(_bank.bank[_currentFrameIndex]));
            if(_currentFrameIndex < _bank.bank.Length)
                _currentFrameIndex++;
        }
    }

    void SetSprite()
    {
        if(_sr)
        {
            _sr.sprite = _bank.bank[_currentFrameIndex];
            if(_colorOverTime) _sr.color = _colorOverTimeGradient.Evaluate(_timer.percent);
        }
        else if(_image)
        {
            _image.sprite = _bank.bank[_currentFrameIndex];
            if(_colorOverTime) _image.color = _colorOverTimeGradient.Evaluate(_timer.percent);
        }
    }

    public void FlipX(bool x, bool y = false)
    {
        if(_sr)
        {
            _sr.flipX = x;
            _sr.flipY = y;
        }
    }

    public void Pause() {_isPaused = true;}
    public void Play() {_isPaused = false;}
    public void Stop()
    {
        _isPaused = true;
        _currentFrameIndex = 0;
        _timer.Reset();
        SetSprite();
    }
    public void Restart()
    {
        _currentFrameIndex = 0;
        _timer.Reset();
        _isComplete = false;
        _isPaused = false;
    }

    public void Reset()
    {
        _hasInit = _isPaused = _isComplete = false;
        _frameDelta = _totalTime = 0f;
        _currentFrameIndex = 0;
        if(_timer != null)
            _timer.Reset();
    }
}
}
