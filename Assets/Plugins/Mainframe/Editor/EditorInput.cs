#region
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Runtime.InteropServices;
using Mainframe;
using UnityEditor;
using UnityEngine;
#endregion

namespace Mainframe_Editor
{
public static class EditorInput
{
  [InitializeOnLoadMethod]
  public static void Init()
  {
    _keyCodes = new HashSet<KeyCode>();
    foreach(int i in Enum.GetValues(typeof(KeyCode)))
    {
      KeyCode key = (KeyCode)i;
      _keyCodes.Add(key);
    }

    FieldInfo info = typeof(EditorApplication).GetField("globalEventHandler", BindingFlags.Static | BindingFlags.NonPublic);
    if(info != null)
    {
      EditorApplication.CallbackFunction value = (EditorApplication.CallbackFunction)info.GetValue(null);
      value += () =>
      {
        for(int i = 0; i < 5; i++)
        {
          if(buttonDown(i)) { _buttonsPressed.Add(i); }
          if(buttonUp(i)) { _buttonsPressed.Remove(i); }
        }
        foreach(KeyCode key in _keyCodes)
        {
          if(keyDown(key)) { _keysPressed.Add(key); }
          if(keyUp(key)) { _keysPressed.Remove(key); }
        }
        onInput?.Invoke();
      };
      info.SetValue(null, value);
    }
  }

  public delegate void GlobalInputHandler();
  public static event GlobalInputHandler onInput;

  static HashSet<KeyCode> _keyCodes = new HashSet<KeyCode>();
  static HashSet<KeyCode> _keysPressed = new HashSet<KeyCode>();
  static HashSet<int> _buttonsPressed = new HashSet<int>();

  public static Event evnt => Event.current;
  public static bool hasEvnt => evnt != null;
  public static bool isRepaintOrLayout => evnt.type == EventType.Repaint || evnt.type == EventType.Layout;

  public static bool isKeyboard => hasEvnt && !isRepaintOrLayout && evnt.isKey;
  public static bool keyDown(KeyCode key) => isKeyboard && evnt.type == EventType.KeyDown && evnt.keyCode == key;
  public static bool keyUp(KeyCode key) => isKeyboard && evnt.type == EventType.KeyUp && evnt.keyCode == key;
  public static bool keyPressed(KeyCode key) => _keysPressed.Contains(key);

  public static bool ctrl => isKeyboard && evnt.control;
  public static bool shift => isKeyboard && evnt.shift;
  public static bool alt => isKeyboard && evnt.alt;

  public static bool isMouse => hasEvnt && !isRepaintOrLayout && evnt.isMouse;
  public static bool buttonDown(int index) => isMouse && evnt.type == EventType.MouseDown && evnt.button == index;
  public static bool buttonUp(int index) => isMouse && evnt.type == EventType.MouseUp && evnt.button == index;
  public static bool buttonPressed(int index) => _buttonsPressed.Contains(index);

  public static Vector2 mousePos => GetCursorPos(out Int2 point) ? new Vector2(point.x, point.y) : Vector2.zero;

  [DllImport("user32.dll")]
  public static extern bool GetCursorPos(out Int2 lpPoint);
}
}
