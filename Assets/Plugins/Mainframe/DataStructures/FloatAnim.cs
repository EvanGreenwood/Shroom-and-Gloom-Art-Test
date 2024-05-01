#region
using System;
using UnityEngine;
#endregion

namespace Mainframe
{
public enum LoopType
{
    None = 0,
    Loop,
    Reverse,
    PingPong,
}

[Serializable]
public struct FloatAnim
{
    [SerializeField]
    Easing _easing;
    LoopType _loopType;
    bool _reverse;
    float _value;
    float _duration;
    float _percent;

    bool _wasFinishedThisFrame, _hasFinished, _wasFinished;

    public FloatAnim(Easing easing, LoopType loopType, float duration, bool reverse)
    {
        _easing = easing;
        _loopType = loopType;
        _duration = Mathf.Max(0.0001f, duration);
        _reverse = reverse;
        _percent = _value = _reverse ? 1.0f : 0.0f;
        _wasFinishedThisFrame = _hasFinished = _wasFinished = false;

        onPingPong = null;
        onLoop = null;
    }

    public FloatAnim(Easing easing, LoopType loopType, float duration) : this(easing, loopType, duration, false) { }
    public FloatAnim(Easing easing, float duration, bool reverse) : this(easing, LoopType.None, duration, reverse) { }
    public FloatAnim(Easing easing, float duration) : this(easing, LoopType.None, duration, false) { }

    public float duration => _duration;
    public bool reverse => _reverse;
    public float percent => _percent;
    public float lerp => _reverse ? 1f - _percent : _percent;
    public Easing easing => _easing;
    public float value => _value;
    public bool wasFinishedThisFrame => _wasFinishedThisFrame;
    public bool hasFinished => _hasFinished;
    float start => _reverse ? 1.0f : 0.0f;
    float target => _reverse ? 0.0f : 1.0f;
    bool targetReached => _reverse ? _percent <= 0.0f : _percent >= 1.0f;

    public delegate void FanimHandler();
    public event FanimHandler onPingPong, onLoop;
    public void OnPingPong() {onPingPong?.Invoke();}

    public void OnLoop() {onLoop?.Invoke();}

    public void Step(float deltaTime)
    {
        float step = deltaTime / _duration;
        _percent += _reverse ? -step : +step;

        _wasFinished = _hasFinished;

        if(targetReached)
        {
            switch(_loopType)
            {
                case LoopType.None:
                    _percent = target;
                    _hasFinished = true;
                    _wasFinishedThisFrame = !_wasFinished && _hasFinished;
                    break;
                case LoopType.Loop:
                    _percent = start;
                    OnLoop();
                    break;
                case LoopType.PingPong:
                    _reverse = !_reverse;
                    _percent = start;
                    OnPingPong();
                    break;
            }
        }

        _value = EASE.Evaluate(Mathf.Clamp01(_percent), _easing);
    }

    public void SetDuration(float duration) {_duration = Mathf.Max(0.0001f, duration);}
    public void Reset(float duration)
    {
        _duration = Mathf.Max(0.0001f, duration);
        Reset();
    }
    public void Reset()
    {
        _percent = _value = 0f;
        _wasFinishedThisFrame = false;
        _wasFinished = _hasFinished = false;
    }
}
}
