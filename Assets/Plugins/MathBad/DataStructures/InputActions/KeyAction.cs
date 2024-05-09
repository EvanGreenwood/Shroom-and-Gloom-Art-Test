#region
using UnityEngine;
using UnityEngine.InputSystem.Controls;
#endregion

namespace MathBad
{
public class KeyAction
{
  private KeyControl _key;
  private float _lastPressDuration;
  private float _lastUpTime, _lastDownTime;

  public KeyAction(KeyControl key)
  {
    _key = key;
  }

  public float lastUpTime => _lastUpTime;
  public float lastDownTime => _lastDownTime;
  public float lastPressDuration => _lastPressDuration;

  public bool pressed => _key.isPressed;

  public bool up
  {
    get
    {
      bool flag = _key.wasReleasedThisFrame;
      if(flag)
      {
        _lastUpTime = Time.realtimeSinceStartup;
        _lastPressDuration = _lastUpTime - _lastDownTime;
      }
      return flag;
    }
  }

  public bool down
  {
    get
    {
      bool flag = _key.wasPressedThisFrame;
      if(flag) { _lastDownTime = Time.realtimeSinceStartup; }
      return flag;
    }
  }
}
}
