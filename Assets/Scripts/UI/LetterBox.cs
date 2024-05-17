#region Usings
using UnityEngine;
using Framework;
using MathBad;
using NaughtyAttributes;
using UnityEngine.UI;
#endregion

public class LetterBox : MonoBehaviourUI
{
    [SerializeField] Image _top, _bottom;
    [SerializeField] float _startDuration = 1f;
    [Range(0f, 2f)]
    [SerializeField] float _size = 0.15f;
    [SerializeField] float _stopSize = 0.0f;

    float _startTime = 0f;
    FloatAnim _fadeTimer = new FloatAnim(EaseType.OutQuart, LoopType.None, 3f);
    bool _isStopped;

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
        if(_isStopped)
            return;
        
        if(_startTime > 0f)
            _startTime -= UnityEngine.Time.deltaTime;

        if(_startTime <= 0f)
        {
            _fadeTimer.Step(UnityEngine.Time.deltaTime);
            float percent = 1f - _fadeTimer.value;
            if(percent <= _stopSize && _stopSize > 0f)
            {
                _isStopped = true;
                return;
            }
            _top.transform.localScale = _top.transform.localScale.WithY(percent);
            _bottom.transform.localScale = _bottom.transform.localScale.WithY(percent);

            if(_fadeTimer.wasFinishedThisFrame)
            {
                _startTime = _startDuration;
                gameObject.SetActive(false);
            }
        }
    }
}
