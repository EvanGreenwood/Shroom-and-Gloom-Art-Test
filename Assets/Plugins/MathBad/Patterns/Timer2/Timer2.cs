#region
using System;
using UnityEngine;
#endregion

namespace MathBad
{
[Serializable]
public class Timer2
{
    // Serialized Fields
    //------------------
    [SerializeField, HideInInspector] float _value = 0.0f;
    [SerializeField] float _baseTarget = 1.0f;
    [SerializeField] float _targetVariance = 0.0f;

    float _currentTarget;
    float _startTime, _finishTime;
    float _percent;
    bool _needsTarget;

    // State
    //------
    bool _wasStarted, _wasFinished;
    bool _wasStartedThisFrame, _wasFinishedThisFrame;
    bool _hasStarted, _hasFinished;

    public Timer2() => Init(1.0f, 0.0f);
    public Timer2(float target) => Init(target, 0.0f);
    public Timer2(float target, float variance) => Init(target, variance);

    public delegate void TimerStepHandler(Timer2 timer2, float dt);
    public event TimerStepHandler onStep;
    void OnStep(float dt) => onStep?.Invoke(this, dt);

    public delegate void TimerHandler(Timer2 timer2);
    public event TimerHandler onComplete, onReset;
    void OnComplete() => onComplete?.Invoke(this);
    void OnReset() => onReset?.Invoke(this);

    // Init
    //----------------------------------------------------------------------------------------------------
    void Init(float target, float variance)
    {
        _baseTarget = target.Max(float.Epsilon);
        _targetVariance = variance;
        _value = 0.0f;

        _percent = 0f;
        _startTime = _finishTime = 0f;
        _needsTarget = true;

        SetStarted(false);
        SetFinished(false);
    }

    // Public Properties
    //------------------
    public float value => _value;
    public float baseTarget => _baseTarget;
    public float currentTarget => _currentTarget;
    public float percent
    {
        get
        {
            if(_currentTarget == 0f)
                return 0f;
            return (_value / _currentTarget).Clamp01();
        }
    }
    public float remaining => _currentTarget - _value;
    public float lastStartTime => _startTime;
    public float lastFinishTime => _finishTime;

    // State
    //------
    public bool wasStartedThisFrame => _wasStartedThisFrame;
    public bool hasStarted => _hasStarted;
    public bool wasFinishedThisFrame => _wasFinishedThisFrame;
    public bool hasFinished => _hasFinished || _wasFinishedThisFrame;

    void SetStarted(bool flag) {_hasStarted = _wasStarted = _wasStartedThisFrame = flag;}
    void SetFinished(bool flag) {_hasFinished = _wasFinished = _wasFinishedThisFrame = flag;}

    // Private Methods
    //----------------------------------------------------------------------------------------------------
    // Get the next target
    float GetNextTarget()
    {
        if(_targetVariance == 0f)
        {
            return _baseTarget;
        }
        else
        {
            float target = _baseTarget + RNG.FloatSign() * _targetVariance.Half();
            return target.Max(0f);
        }
    }

    // Step State
    void StepState(float delta)
    {
        _wasStarted = _hasStarted;
        _wasFinished = _hasFinished;

        _hasStarted = _value > 0f;
        _hasFinished = _value >= _currentTarget;

        _wasStartedThisFrame = !_wasStarted && _hasStarted;
        _wasFinishedThisFrame = !_wasFinished && _hasFinished;

        if(_wasStartedThisFrame) _startTime = Time.time;
        if(_wasFinishedThisFrame) _finishTime = Time.time;
        if(_wasFinishedThisFrame) { OnComplete(); }
    }

    // Public
    //----------------------------------------------------------------------------------------------------
    /// <summary>
    /// Increments <c>_value</c> by the given delta.
    /// </summary>
    /// <param name="delta">The amount to translate _value by.</param>
    public void Step(float delta)
    {
        if(!hasFinished)
        {
            if(_needsTarget)
            {
                _currentTarget = GetNextTarget();
                _needsTarget = false;
            }
            _percent += delta / _currentTarget;
            _value += delta;
            _value = _value.Clamp(0.0f, _currentTarget);

            OnStep(delta);
        }
        StepState(delta);
    }

    /// <summary>
    /// Sets state to finished.
    /// </summary>
    public void ForceComplete()
    {
        _value = _currentTarget;
        SetStarted(true);
        SetFinished(true);

        OnComplete();
    }

    public float Unlerp01(float min, float max) => MATH.Unlerp01(min, max, _value);
    public float Evaluate(EaseType type = EaseType.Linear) => EASE.Evaluate(_percent.Clamp01(), type);

    public Vector2 Lerp(Vector2 a, Vector2 b, EaseType type = EaseType.Linear) => MATH.Lerp(a, b, EASE.Evaluate(_percent.Clamp01(), type));
    public Vector3 Lerp(Vector3 a, Vector3 b, EaseType type = EaseType.Linear) => MATH.Lerp(a, b, EASE.Evaluate(_percent.Clamp01(), type));
    public Color Lerp(Color a, Color b, EaseType type = EaseType.Linear) => Color.Lerp(a, b, EASE.Evaluate(_percent.Clamp01(), type));

    /// <summary>
    /// Resets to initial state.
    /// </summary>
    public void Reset() {Reset(_baseTarget);}
    public void Reset(float target)
    {
        Init(target, _targetVariance);
        OnReset();
    }

    // Formatting
    public override string ToString() => $"{_value:F1} / {_currentTarget}";
}
}
