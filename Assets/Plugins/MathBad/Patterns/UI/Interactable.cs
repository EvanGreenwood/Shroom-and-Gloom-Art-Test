#region
using UnityEngine.EventSystems;
#endregion

namespace MathBad
{
public interface IInteractable
{
  void OnPointerEnter(PointerEventData eventData);
  void OnPointerExit(PointerEventData eventData);
  void OnPointerClick(PointerEventData eventData);
}

public class Interactable : MonoBehaviourUI,
                            IPointerEnterHandler,
                            IPointerExitHandler,
                            IPointerClickHandler
{
  IInteractable[] _interactables;

  // MonoBehaviour
  //----------------------------------------------------------------------------------------------------
  void Awake() {_interactables = GetComponents<IInteractable>();}

  // Pointer Listener
  //----------------------------------------------------------------------------------------------------
  public void OnPointerEnter(PointerEventData eventData) {_interactables.Foreach(interactable => interactable.OnPointerEnter(eventData));}
  public void OnPointerExit(PointerEventData eventData) {_interactables.Foreach(interactable => interactable.OnPointerExit(eventData));}
  public void OnPointerClick(PointerEventData eventData) {_interactables.Foreach(interactable => interactable.OnPointerClick(eventData));}
}
}
