#region Usings
using UnityEngine;
using Framework;
using Mainframe;
using NaughtyAttributes;
using UnityEngine.UI;
#endregion

public class LetterBox : MonoBehaviourUI
{
    [SerializeField] Image _top, _bottom;
    [SerializeField] float _startDuration = 1f;
    [Range(0f, 1f)]
    [SerializeField]
    float _size = 0.15f;

    float _startTime = 0f;
    FloatAnim _fadeTimer = new FloatAnim(EaseType.OutQuart, LoopType.None, 3f);

    // Monobehaviour
    //----------------------------------------------------------------------------------------------------
    [Button]
    void Resize()
    {
        _top.rectTransform.sizeDelta = _top.rectTransform.sizeDelta.WithY(SCREEN.size.y.Half() * _size);
        _bottom.rectTransform.sizeDelta = _bottom.rectTransform.sizeDelta.WithY(SCREEN.size.y.Half() * _size);
    }

    void Awake() {_startTime = _startDuration;}

    void Update()
    {
        if(_startTime > 0f)
            _startTime -= Time.deltaTime;

        if(_startTime <= 0f)
        {
            _fadeTimer.Step(Time.deltaTime);
            _top.transform.localScale = _top.transform.localScale.WithY(1f - _fadeTimer.value);
            _bottom.transform.localScale = _bottom.transform.localScale.WithY(1f - _fadeTimer.value);

            if(_fadeTimer.wasFinishedThisFrame)
            {
                _startTime = _startDuration;
                gameObject.SetActive(false);
            }
        }
    }
}
