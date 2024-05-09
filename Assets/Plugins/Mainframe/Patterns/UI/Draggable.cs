#region
using UnityEngine;
using UnityEngine.EventSystems;
#endregion

namespace Mainframe
{
public interface IDraggable
{
  void OnBeginDrag(PointerEventData eventData);
  void OnDrag(PointerEventData eventData);
  void OnEndDrag(PointerEventData eventData);
}

[RequireComponent(typeof(CanvasGroup))]
public class Draggable : MonoBehaviourUI,
                         IBeginDragHandler,
                         IDragHandler,
                         IEndDragHandler
{
  CanvasGroup _canvasGroup;
  IDraggable[] _draggables;

  bool _isBeingDragged;
  Vector2 _dragOffset;
  RectTransform _lastParent;
  int _lastSiblingIndex;

  public bool canDrag { get; set; } = true;
  public bool isBeingDragged => _isBeingDragged;

  // MonoBehaviour
  //----------------------------------------------------------------------------------------------------
  void Awake()
  {
    _canvasGroup = GetComponent<CanvasGroup>();
    _draggables = GetComponents<IDraggable>();
    _lastParent = rectTransform.parent as RectTransform;
    _lastSiblingIndex = rectTransform.GetSiblingIndex();
  }

  // Drag Listener
  //----------------------------------------------------------------------------------------------------
  public void OnBeginDrag(PointerEventData eventData)
  {
    if(!canDrag)
      return;

    _isBeingDragged = true;
    _canvasGroup.blocksRaycasts = false;

    _lastParent = rectTransform.parent as RectTransform;
    _lastSiblingIndex = rectTransform.GetSiblingIndex();

    _dragOffset = CAMERA.mouseWorld - (Vector2)rectTransform.position;

    rectTransform.SetParent(canvas.transform);
    _draggables.Foreach(draggable => draggable.OnBeginDrag(eventData));
  }

  public void OnDrag(PointerEventData eventData)
  {
    if(!canDrag)
      return;

    _draggables.Foreach(draggable => draggable.OnDrag(eventData));

    Vector2 pos = CAMERA.mouseWorldUI;
    rectTransform.position = new Vector3(pos.x, pos.y, -canvas.planeDistance) - _dragOffset._Vec3();
  }

  public void OnEndDrag(PointerEventData eventData)
  {
    if(!canDrag)
      return;

    _isBeingDragged = false;
    _canvasGroup.blocksRaycasts = true;

    if(!SCREEN.rect.Contains(eventData.position))
    {
      DragReset(eventData);
    }

    _draggables.Foreach(draggable => draggable.OnEndDrag(eventData));
  }

  public void DragReset(PointerEventData eventData)
  {
    rectTransform.SetParent(_lastParent);
    rectTransform.SetSiblingIndex(_lastSiblingIndex);
  }
}
}
