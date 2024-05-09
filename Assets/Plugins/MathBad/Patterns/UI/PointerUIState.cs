#region Usings
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
#endregion

namespace MathBad
{
public static class PointerUIState
{
  public static bool isDragging { get; set; } = false;
  public static Component dragContext { get; set; } = null;

  /// <summary>
  /// Gets a value indicating whether the pointer is over a UI element.
  /// </summary>
  public static bool isOverUI => IsPointerOverUIElement();

  /// <summary>
  /// Retrieves a list of RaycastResults by performing a raycast from the current mouse position.
  /// </summary>
  /// <returns>A list of RaycastResults.</returns>
  public static List<RaycastResult> GetEventSystemRaycastResults()
  {
    PointerEventData eventData = new PointerEventData(EventSystem.current);
    eventData.position = Input.mousePosition;
    List<RaycastResult> raycastResults = new List<RaycastResult>();
    EventSystem.current.RaycastAll(eventData, raycastResults);
    return raycastResults;
  }

  /// <summary>
  /// Determines if the pointer is over any UI element.
  /// </summary>
  /// <returns>A boolean indicating whether the pointer is over a UI element.</returns>
  public static bool IsPointerOverUIElement() => IsPointerOverUIElement(GetEventSystemRaycastResults());

  /// <summary>
  /// Checks if the pointer is over a UI element by checking the given list of RaycastResults.
  /// </summary>
  /// <param name="eventSystemRaycastResults">A list of RaycastResults from the Event System.</param>
  /// <returns>A boolean indicating whether the pointer is over a UI element.</returns>
  public static bool IsPointerOverUIElement(List<RaycastResult> eventSystemRaycastResults)
  {
    foreach(RaycastResult raycastResult in eventSystemRaycastResults)
    {
      if(raycastResult.gameObject.GetComponent<IgnoreUIState>())
        continue;

      if(raycastResult.gameObject.layer == LayerMask.NameToLayer("UI")
      || raycastResult.gameObject.layer == LayerMask.NameToLayer("HUD"))
      {
        Graphic graphic = raycastResult.gameObject.GetComponent<Graphic>();
        if(graphic != null && graphic.raycastTarget) return true;
      }
    }

    return false;
  }
}
}
