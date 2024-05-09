#region
using System;
using UnityEngine;
using UnityEngine.UI;
#endregion

namespace MathBad
{
public static class CanvasExt
{
  public static float z(this Canvas canvas) => canvas.transform.position.z;
}
[RequireComponent(typeof(Canvas), typeof(GraphicRaycaster), typeof(CanvasScaler))]
public class CanvasSingleton<T> : MonoSingleton<T> where T : MonoBehaviour
{
  RectTransform _rectTransform;
  Canvas _canvas;
  GraphicRaycaster _raycaster;
  CanvasScaler _canvasScaler;

  public Camera worldCamera => canvas.worldCamera;

  public Canvas canvas
  {
    get
    {
      if(_canvas == null)
        _canvas = GetComponent<Canvas>();
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

  public GraphicRaycaster raycaster
  {
    get
    {
      if(_raycaster == null)
        _raycaster = GetComponent<GraphicRaycaster>();
      return _raycaster;
    }
  }
  public CanvasScaler canvasScaler
  {
    get
    {
      if(_canvasScaler == null)
        _canvasScaler = GetComponent<CanvasScaler>();
      return _canvasScaler;
    }
  }

  // First Load
  //----------------------------------------------------------------------------------------------------
  protected override void OnSingletonFirstLoad() {base.OnSingletonFirstLoad();}

  // Util
  //----------------------------------------------------------------------------------------------------
  public Vector3 ScreenToWorld(Vector3 pos) => canvas.ScreenToWorldPoint(pos);
  public Rect GetWorldRect() => rectTransform.GetWorldRect();
  public Rect GetScreenRect() => rectTransform.GetScreenRect(worldCamera);

  public Vector2 WorldToScreenPoint(Vector3 pos) => worldCamera.WorldToScreenPoint(pos);
}
}
