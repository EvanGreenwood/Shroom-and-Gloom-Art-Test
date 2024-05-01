#region
using UnityEngine;
using UnityEngine.InputSystem.Controls;
#endregion

namespace Mainframe
{
public class ButtonAction
{
  private ButtonControl _button;
  private float _lastPressDuration;
  private float _lastUpTime, _lastDownTime;

  public ButtonAction(ButtonControl button)
  {
    _button = button;
  }

  public float lastUpTime => _lastUpTime;
  public float lastDownTime => _lastDownTime;
  public float lastPressDuration => _lastPressDuration;

  public bool pressed => _button.isPressed;

  public bool up
  {
    get
    {
      bool flag = _button.wasReleasedThisFrame;
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
      bool flag = _button.wasPressedThisFrame;
      if(flag) { _lastDownTime = Time.realtimeSinceStartup; }
      return flag;
    }
  }
}
}
