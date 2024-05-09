#region
using TMPro;
using UnityEngine;
using UnityEngine.UI;
#endregion

namespace MathBad
{
public class FieldUI : MonoBehaviourUI
{
  [SerializeField] TextMeshProUGUI _fieldName;
  [SerializeField] TextMeshProUGUI _fieldValue;
  HorizontalLayoutGroup _layout;

  public void Init(string fieldName, string fieldValue, Vector2 pivot)
  {
    _layout = GetComponent<HorizontalLayoutGroup>();

    if(pivot.x == 0 && pivot.y == 1f) _layout.childAlignment = TextAnchor.UpperLeft;
    if(pivot.x == 0.5 && pivot.y == 1f) _layout.childAlignment = TextAnchor.UpperCenter;
    if(pivot.x == 1 && pivot.y == 1f) _layout.childAlignment = TextAnchor.UpperRight;

    rectTransform.pivot = pivot;
    _fieldName.text = fieldName;
    _fieldValue.text = fieldValue;
  }
  public void Step(string fieldValue) {_fieldValue.text = fieldValue;}
}
}
