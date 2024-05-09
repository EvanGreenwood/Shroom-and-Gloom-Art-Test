#region
using Mainframe;
using UnityEngine;
#endregion

public class FieldList : MonoBehaviourUI
{
  [SerializeField] Vector2 _fieldPivot;
  [SerializeField] FieldUI _fieldPrefab;

  public FieldUI AppendNewField(string fieldName, string fieldValue)
  {
    FieldUI field = Instantiate(_fieldPrefab, rectTransform, false);
    field.Init(fieldName, fieldValue, _fieldPivot);
    return field;
  }
}
