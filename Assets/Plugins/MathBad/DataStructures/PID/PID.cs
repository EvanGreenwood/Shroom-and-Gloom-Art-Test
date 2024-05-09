#region
using System;
using UnityEngine;
#endregion

namespace MathBad
{
// PID
//----------------------------------------------------------------------------------------------------
/// <summary> PID Controller (Proportional Integral Derivative)
/// Represents a Proportional-Integral-Derivative (PID) controller.
/// A PID controller is a control loop feedback mechanism widely used in industrial control systems.
/// It calculates an "error" value as the difference between a desired set-point and a measured process variable.
/// This class provides the means to compute an output based on that error and its history.
/// <summary>;
[Serializable]
public class PID
{
  [SerializeField] float _kp, _ki, _kd;
  float _ip, _ii, _id;
  float _p, _i, _d;

  float _prevError;

  bool _hasInit = false;

  /// <summary>
  /// Creates a new PID Controller.
  /// </summary>
  /// <param name="p">proportional</param>
  /// <param name="i">integral</param>
  /// <param name="d">derivative</param>
  public PID(float p = 1.0f, float i = 0.0f, float d = 0.1f)
  {
    _kp = _ip = p;
    _ki = _ii = i;
    _kd = _id = d;
  }

  /// <summary>
  /// Proportional gain coefficient.
  /// This value determines the reaction based on the current error.
  /// A high proportional gain results in a large change in the output for a given change in the error.
  /// </summary>
  public float kp
  {
    get => _kp;
    set => _kp = value;
  }

  /// <summary>
  /// Integral gain coefficient.
  /// Represents the accumulation of past errors. If error has been present for an extended period of time, it will accumulate.
  /// The integral term seeks to eliminate the residual error by increasing the control action in relation to the accumulated error.
  /// </summary>
  public float ki
  {
    get => _ki;
    set => _ki = value;
  }

  /// <summary>
  /// Derivative gain coefficient.
  /// Represents a prediction of future error, based on its rate of change.
  /// It provides a control action to counteract the rate of error change, aiming to bring the error to zero more swiftly.
  /// </summary>
  public float kd
  {
    get => _kd;
    set => _kd = value;
  }

  /// <summary>
  /// Reset the controller to initialized values.
  /// </summary>
  public void Reset()
  {
    _kp = _ip;
    _ki = _ii;
    _kd = _id;
  }

  /// <summary>
  /// Calculates the PID controller output based on the provided error and change in time.
  /// </summary>
  /// <param name="currentError">The current difference between the desired set-point and the measured process variable.</param>
  /// <param name="deltaTime">The change in time since the last error measurement.</param>
  /// <returns>The computed output of the PID controller.</returns>
  public float GetOutput(float currentError, float deltaTime)
  {
    if(!_hasInit) { Init(); }

    _p = currentError;
    _i += _p * deltaTime;
    _d = (_p - _prevError) / deltaTime;
    _prevError = currentError;

    return _p * kp + _i * ki + _d * kd;
    //----------
    void Init()
    {
      _ip = _kp;
      _ii = _ki;
      _id = _kd;
      _hasInit = true;
    }
  }
}
}
