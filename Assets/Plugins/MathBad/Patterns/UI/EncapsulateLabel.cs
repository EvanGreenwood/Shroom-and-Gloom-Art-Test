#region
using MathBad;
using NaughtyAttributes;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
#endregion

public class EncapsulateLabel : MonoBehaviourUI
{
  [SerializeField] TextMeshProUGUI _label;
  [SerializeField] bool _autoEncapsulate = true;
  [SerializeField] bool _hasLayoutElement;
  [SerializeField] Bit2 _axis = new Bit2(true, true);
  [SerializeField] Vector2 _padding = new Vector2(5f, 5f);

  LayoutElement _layoutElement;

  bool _isDirty;

  // Monobehaviour
  //----------------------------------------------------------------------------------------------------
  void OnValidate()
  {
    if(_label == null)
      _label = GetComponent<TextMeshProUGUI>();
    if(_label == null)
      _label = GetComponentInChildren<TextMeshProUGUI>();

    if(_hasLayoutElement && _layoutElement == null)
      _layoutElement = GetComponent<LayoutElement>();
  }
  void OnEnable() {Encapsulate();}
  void Awake()
  {
    if(_autoEncapsulate)
    {
      TMPro_EventManager.TEXT_CHANGED_EVENT.Add(OnTextChanged);
    }
    Encapsulate();
  }
  void Update()
  {
    if(_autoEncapsulate && _isDirty)
      Encapsulate();
  }

  // Text Event Listener
  //----------------------------------------------------------------------------------------------------
  void OnTextChanged(Object obj)
  {
    if(obj == _label)
    {
      _isDirty = true;
    }
  }

  // Wrap
  //----------------------------------------------------------------------------------------------------
  [Button]
  public void Encapsulate()
  {
    if(_label == null)
      return;

    _isDirty = false;

    if(_label == null) _label = GetComponentInChildren<TextMeshProUGUI>();
    if(_label != null) rectTransform.Encapsulate(_label, _axis, _padding);

    if(_hasLayoutElement && _layoutElement != null)
    {
      _layoutElement.minWidth = rectTransform.sizeDelta.x;
      _layoutElement.minHeight = rectTransform.sizeDelta.y;
    }
  }
}
