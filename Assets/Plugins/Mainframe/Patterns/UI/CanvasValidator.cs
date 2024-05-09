#region
using Mainframe;
using NaughtyAttributes;
using UnityEngine;
using UnityEngine.UI;
#endregion

[RequireComponent(typeof(Canvas))]
public class CanvasValidator : MonoBehaviour
{
  [SerializeField] bool _validateOnAwake;
  [SerializeField] CameraID _cameraType = CameraID.UICamera;
  [SerializeField] RenderMode _renderMode = RenderMode.ScreenSpaceCamera;
  [SerializeField] Int2 _referenceResolution = new Int2(1920, 1080);
  [SerializeField] int _planeDst = 5;

  Canvas _canvas;
  CanvasScaler _canvasScaler;

  [Button]
  public void Validate()
  {
    if(_canvas == null)
    {
      _canvas = GetComponent<Canvas>();
      _canvasScaler = GetComponent<CanvasScaler>();
    }

    _canvas.renderMode = _renderMode;
    _canvas.worldCamera = _cameraType == CameraID.MainCamera ? Camera.main : CAMERA.ui;
    _canvas.planeDistance = _planeDst;

    _canvasScaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
    _canvasScaler.referenceResolution = (Vector2)_referenceResolution;
  }

  void Awake()
  {
    if(_validateOnAwake)
      Validate();
  }
}
