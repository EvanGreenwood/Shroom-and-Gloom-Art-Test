//  ___  _  _  ___  _   _  _____                                                                      
// |_ _|| \| || _ \| | | ||_   _|                                                                     
//  | | | .` ||  _/| |_| |  | |                                                                       
// |___||_|\_||_|   \___/   |_|                                                                       
//                                                                                                    
//----------------------------------------------------------------------------------------------------

#region
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;
using UnityEngine.InputSystem;
#endregion

namespace MathBad
{
public static class INPUT
{
  // Keyboard
  public static Keyboard keyboard => Keyboard.current;
  private static Dictionary<Key, KeyAction> _keys = new Dictionary<Key, KeyAction>();
  // Mouse
  public static Mouse mouse => Mouse.current;
  private static readonly List<Key> _allKeys;

  static INPUT()
  {
    _keys = new Dictionary<Key, KeyAction>();
    _allKeys = new List<Key>();
    for(int index = 0; index < keyboard.allKeys.Count; index++)
    {
      allKeys.Add(keyboard.allKeys[index].keyCode);
      _keys.Add(keyboard.allKeys[index].keyCode, new KeyAction(keyboard.allKeys[index]));
    }
  }

  public static Dictionary<Key, KeyAction> keys => _keys;
  public static List<Key> allKeys => _allKeys;

  public static Vector2 moveInput2
  {
    get
    {
      Vector2 xy = new Vector2();

      if(_keys[Key.A].pressed || _keys[Key.LeftArrow].pressed) xy.x--;
      if(_keys[Key.D].pressed || _keys[Key.RightArrow].pressed) xy.x++;
      if(_keys[Key.W].pressed || _keys[Key.UpArrow].pressed) xy.y++;
      if(_keys[Key.S].pressed || _keys[Key.DownArrow].pressed) xy.y--;

      return xy.normalized;
    }
  }

  // Keys
  //----------------------------------------------------------------------------------------------------
  public static KeyAction leftShift { [MethodImpl(MethodImplOptions.AggressiveInlining)] get => _keys[Key.LeftShift]; }
  public static KeyAction leftCtrl { [MethodImpl(MethodImplOptions.AggressiveInlining)] get => _keys[Key.LeftCtrl]; }
  public static KeyAction leftAlt { [MethodImpl(MethodImplOptions.AggressiveInlining)] get => _keys[Key.LeftAlt]; }
  public static KeyAction space { [MethodImpl(MethodImplOptions.AggressiveInlining)] get => _keys[Key.Space]; }
  public static KeyAction enter { [MethodImpl(MethodImplOptions.AggressiveInlining)] get => _keys[Key.Enter]; }
  public static KeyAction tab { [MethodImpl(MethodImplOptions.AggressiveInlining)] get => _keys[Key.Tab]; }
  public static KeyAction tilda { [MethodImpl(MethodImplOptions.AggressiveInlining)] get => _keys[Key.Backquote]; }
  public static KeyAction backspace { [MethodImpl(MethodImplOptions.AggressiveInlining)] get => _keys[Key.Backspace]; }
  public static KeyAction escape { [MethodImpl(MethodImplOptions.AggressiveInlining)] get => _keys[Key.Escape]; }
  public static KeyAction upArrow { [MethodImpl(MethodImplOptions.AggressiveInlining)] get => _keys[Key.UpArrow]; }
  public static KeyAction downArrow { [MethodImpl(MethodImplOptions.AggressiveInlining)] get => _keys[Key.DownArrow]; }
  public static KeyAction leftArrow { [MethodImpl(MethodImplOptions.AggressiveInlining)] get => _keys[Key.LeftArrow]; }
  public static KeyAction rightArrow { [MethodImpl(MethodImplOptions.AggressiveInlining)] get => _keys[Key.RightArrow]; }
  public static KeyAction a { [MethodImpl(MethodImplOptions.AggressiveInlining)] get => _keys[Key.A]; }
  public static KeyAction b { [MethodImpl(MethodImplOptions.AggressiveInlining)] get => _keys[Key.B]; }
  public static KeyAction c { [MethodImpl(MethodImplOptions.AggressiveInlining)] get => _keys[Key.C]; }
  public static KeyAction d { [MethodImpl(MethodImplOptions.AggressiveInlining)] get => _keys[Key.D]; }
  public static KeyAction e { [MethodImpl(MethodImplOptions.AggressiveInlining)] get => _keys[Key.E]; }
  public static KeyAction f { [MethodImpl(MethodImplOptions.AggressiveInlining)] get => _keys[Key.F]; }
  public static KeyAction g { [MethodImpl(MethodImplOptions.AggressiveInlining)] get => _keys[Key.G]; }
  public static KeyAction h { [MethodImpl(MethodImplOptions.AggressiveInlining)] get => _keys[Key.H]; }
  public static KeyAction i { [MethodImpl(MethodImplOptions.AggressiveInlining)] get => _keys[Key.I]; }
  public static KeyAction j { [MethodImpl(MethodImplOptions.AggressiveInlining)] get => _keys[Key.J]; }
  public static KeyAction k { [MethodImpl(MethodImplOptions.AggressiveInlining)] get => _keys[Key.K]; }
  public static KeyAction l { [MethodImpl(MethodImplOptions.AggressiveInlining)] get => _keys[Key.L]; }
  public static KeyAction m { [MethodImpl(MethodImplOptions.AggressiveInlining)] get => _keys[Key.M]; }
  public static KeyAction n { [MethodImpl(MethodImplOptions.AggressiveInlining)] get => _keys[Key.N]; }
  public static KeyAction o { [MethodImpl(MethodImplOptions.AggressiveInlining)] get => _keys[Key.O]; }
  public static KeyAction p { [MethodImpl(MethodImplOptions.AggressiveInlining)] get => _keys[Key.P]; }
  public static KeyAction q { [MethodImpl(MethodImplOptions.AggressiveInlining)] get => _keys[Key.Q]; }
  public static KeyAction r { [MethodImpl(MethodImplOptions.AggressiveInlining)] get => _keys[Key.R]; }
  public static KeyAction s { [MethodImpl(MethodImplOptions.AggressiveInlining)] get => _keys[Key.S]; }
  public static KeyAction t { [MethodImpl(MethodImplOptions.AggressiveInlining)] get => _keys[Key.T]; }
  public static KeyAction u { [MethodImpl(MethodImplOptions.AggressiveInlining)] get => _keys[Key.U]; }
  public static KeyAction v { [MethodImpl(MethodImplOptions.AggressiveInlining)] get => _keys[Key.V]; }
  public static KeyAction w { [MethodImpl(MethodImplOptions.AggressiveInlining)] get => _keys[Key.W]; }
  public static KeyAction x { [MethodImpl(MethodImplOptions.AggressiveInlining)] get => _keys[Key.X]; }
  public static KeyAction y { [MethodImpl(MethodImplOptions.AggressiveInlining)] get => _keys[Key.Y]; }
  public static KeyAction z { [MethodImpl(MethodImplOptions.AggressiveInlining)] get => _keys[Key.Z]; }
  public static KeyAction f1 { [MethodImpl(MethodImplOptions.AggressiveInlining)] get => _keys[Key.F1]; }
  public static KeyAction f2 { [MethodImpl(MethodImplOptions.AggressiveInlining)] get => _keys[Key.F2]; }
  public static KeyAction f3 { [MethodImpl(MethodImplOptions.AggressiveInlining)] get => _keys[Key.F3]; }
  public static KeyAction f4 { [MethodImpl(MethodImplOptions.AggressiveInlining)] get => _keys[Key.F4]; }
  public static KeyAction f5 { [MethodImpl(MethodImplOptions.AggressiveInlining)] get => _keys[Key.F5]; }
  public static KeyAction f6 { [MethodImpl(MethodImplOptions.AggressiveInlining)] get => _keys[Key.F6]; }
  public static KeyAction f7 { [MethodImpl(MethodImplOptions.AggressiveInlining)] get => _keys[Key.F7]; }
  public static KeyAction f8 { [MethodImpl(MethodImplOptions.AggressiveInlining)] get => _keys[Key.F8]; }
  public static KeyAction f9 { [MethodImpl(MethodImplOptions.AggressiveInlining)] get => _keys[Key.F9]; }
  public static KeyAction f10 { [MethodImpl(MethodImplOptions.AggressiveInlining)] get => _keys[Key.F10]; }
  public static KeyAction f11 { [MethodImpl(MethodImplOptions.AggressiveInlining)] get => _keys[Key.F11]; }
  public static KeyAction f12 { [MethodImpl(MethodImplOptions.AggressiveInlining)] get => _keys[Key.F12]; }

  // Mouse
  //----------------------------------------------------------------------------------------------------
  public static ButtonAction leftMouse { get; } = new ButtonAction(mouse.leftButton);
  public static ButtonAction middleMouse { get; } = new ButtonAction(mouse.middleButton);
  public static ButtonAction rightMouse { get; } = new ButtonAction(mouse.rightButton);
  public static ButtonAction forwardMouse { get; } = new ButtonAction(mouse.forwardButton);
  public static ButtonAction backMouse { get; } = new ButtonAction(mouse.backButton);

  public static Vector2 mousePos { [MethodImpl(MethodImplOptions.AggressiveInlining)] get => mouse.position.ReadValue(); }
  public static Vector2 mousePosCanvas { [MethodImpl(MethodImplOptions.AggressiveInlining)] get => new Vector2(mousePos.x, Screen.height - mousePos.y); }
  public static Vector2 mouseDelta { [MethodImpl(MethodImplOptions.AggressiveInlining)] get => mouse.delta.ReadValue(); }
  public static float mouseScroll { [MethodImpl(MethodImplOptions.AggressiveInlining)] get => mouse.scroll.ReadValue().y; }

  static bool _mouseCaptured;
  public static bool mouseCaptured
  {
    get => _mouseCaptured;
    set
    {
      _mouseCaptured = value;
      Cursor.visible = !_mouseCaptured;
      Cursor.lockState = _mouseCaptured ? CursorLockMode.Locked : CursorLockMode.None;
    }
  }

  public static void SetCursorPos(Vector2 pos) {mouse.WarpCursorPosition(pos);}
}
}
