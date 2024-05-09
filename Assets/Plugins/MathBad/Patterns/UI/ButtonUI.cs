#region
using System;
using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.UI;
#endregion

namespace MathBad
{
public enum SelectionState
{
  None,
  Highlighted,
  Pressed,
  Selected,
  Disabled,
}

[Serializable]
public class ButtonColor
{
  public Color normalColor = Color.white;
  public Color disabledColor = Color.grey;
  public Color highlightedColor = Color.blue;
  public Color selectedColor = Color.green;
  public Color pressedColor = Color.red;
  public float fadeDuration = 0.1f;
}

public class ButtonUI : MonoBehaviourUI,
                        IMoveHandler,
                        IPointerDownHandler, IPointerUpHandler,
                        IPointerEnterHandler, IPointerExitHandler,
                        ISelectHandler, IDeselectHandler,
                        ISubmitHandler
{
  [SerializeField] bool _isDisabled;
  [SerializeField] string _text = "BUTTON";
  [SerializeField] int _fontSize = 16;
  [SerializeField] EffectSoundBank _onEnter, _onExit, _onDown;
  [SerializeField] ButtonColor _imageButtonColor;
  [SerializeField] ButtonColor _textButtonColor;

  [SerializeField] Image _image;
  [SerializeField] TextMeshProUGUI _label;
  [SerializeField] UnityEvent _onClick = new UnityEvent();

  bool _isOver;

  public TextMeshProUGUI label => _label;
  public UnityEvent onClick => _onClick;

  void Press()
  {
    if(_isDisabled)
      return;
    _onClick.Invoke();
  }

  public void OnDisable()
  {
    ChangeState(SelectionState.None);
  }
  public void OnValidate()
  {
    if(!_image) _image = GetComponentInChildren<Image>();
    if(!_label) _label = GetComponentInChildren<TextMeshProUGUI>();
    if(_label)
    {
      _label.text = _text;
      _label.fontSize = _fontSize;
    }

    ChangeState(_isDisabled ? SelectionState.Disabled : SelectionState.None);
  }

  void ChangeState(SelectionState state)
  {
    switch(state)
    {
      case SelectionState.Highlighted:
        SetColors(_imageButtonColor.highlightedColor, _textButtonColor.highlightedColor);
        break;
      case SelectionState.Pressed:
        SetColors(_imageButtonColor.pressedColor, _textButtonColor.pressedColor);
        break;
      case SelectionState.Selected:
        SetColors(_imageButtonColor.selectedColor, _textButtonColor.selectedColor);
        break;
      case SelectionState.Disabled:
        SetColors(_imageButtonColor.disabledColor, _textButtonColor.disabledColor);
        break;
      case SelectionState.None:
      default:
        SetColors(_imageButtonColor.normalColor, _textButtonColor.normalColor);
        break;
    }
  }

  void SetColors(Color image, Color text)
  {
    if(_image) _image.color = image;
    if(_label) _label.color = text;
  }

  IEnumerator OnFinishSubmit()
  {
    float fadeTime = _imageButtonColor.fadeDuration;
    float elapsedTime = 0f;

    while(elapsedTime < fadeTime)
    {
      elapsedTime += Time.unscaledDeltaTime;
      yield return null;
    }

    ChangeState(SelectionState.None);
  }

  public void OnMove(AxisEventData eventData) { }
  public virtual void OnPointerClick(PointerEventData eventData)
  {
    if(_isDisabled) return;
  }

  public virtual void OnSubmit(BaseEventData eventData)
  {
    if(_isDisabled)
      return;
    Press();
    ChangeState(SelectionState.Pressed);
    StartCoroutine(OnFinishSubmit());
  }

  public void OnPointerDown(PointerEventData eventData)
  {
    if(_isDisabled) return;
    ChangeState(SelectionState.Pressed);
    _onDown.Play(transform.position);
  }
  public void OnPointerUp(PointerEventData eventData)
  {
    if(_isDisabled) return;
    ChangeState(SelectionState.None);
    if(_isOver) ChangeState(SelectionState.Highlighted);
    Press();
  }
  public void OnPointerEnter(PointerEventData eventData)
  {
    if(_isDisabled) return;
    _isOver = true;
    ChangeState(SelectionState.Highlighted);
    _onEnter.Play(transform.position);
  }
  public void OnPointerExit(PointerEventData eventData)
  {
    if(_isDisabled) return;
    _isOver = false;
    ChangeState(SelectionState.None);
    _onExit.Play(transform.position);
  }
  public void OnSelect(BaseEventData eventData)
  {
    if(_isDisabled) return;
    ChangeState(SelectionState.Selected);
  }
  public void OnDeselect(BaseEventData eventData)
  {
    if(_isDisabled) return;
    ChangeState(SelectionState.None);
  }
  public void Enable()
  {
    _isDisabled = false;
    ChangeState(SelectionState.None);
  }
  public void Disable()
  {
    _isDisabled = true;
    ChangeState(SelectionState.Disabled);
  }
}
}
