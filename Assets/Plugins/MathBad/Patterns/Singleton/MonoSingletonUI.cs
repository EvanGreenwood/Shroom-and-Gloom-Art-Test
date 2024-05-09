#region Usings
using UnityEngine;
#endregion

namespace MathBad
{
public abstract class MonoSingletonUI<T> : MonoSingleton<T> where T : MonoBehaviour
{
  Canvas _canvas;
  RectTransform _rectTransform;

  public Camera worldCamera => canvas.worldCamera;
  public Vector2 screenPosition => WorldToScreen(rectTransform.position);
  public Vector3 worldPosition => rectTransform.position;
  public Vector2 worldPosition2 => rectTransform.position;

  public Canvas canvas
  {
    get
    {
      if(_canvas == null)
        _canvas = gameObject.GetComponentInParent<Canvas>();
      return _canvas;
    }
  }

  public RectTransform rectTransform
  {
    get
    {
      if(_rectTransform == null)
        _rectTransform = transform as RectTransform;
      return _rectTransform;
    }
  }

  public Rect GetWorldRect() => rectTransform.GetWorldRect();
  public Rect GetScreenRect() => rectTransform.GetScreenRect(worldCamera);

  public Vector2 WorldToScreen(Vector3 pos) => worldCamera.WorldToScreenPoint(pos);
  public Vector3 ScreenToWorld(Vector2 pos) => canvas.ScreenToWorldPoint(pos);
}
}
