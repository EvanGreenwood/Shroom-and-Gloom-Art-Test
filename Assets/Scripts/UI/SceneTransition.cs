#region Usings
using System;
using System.Collections;
using UnityEngine;
using MathBad;
using TMPro;
using UnityEngine.Rendering.PostProcessing;
using UnityEngine.UI;
#endregion

public class SceneTransition : MonoSingletonUI<SceneTransition>
{
    [SerializeField] Color _defaultTitleColor = RGB.darkGrey;
    [SerializeField] TextMeshProUGUI _title;
    [SerializeField] TextMeshProUGUI _message;
    [SerializeField] Image _panel;
    [SerializeField] CanvasGroup _canvasGroup;
    [SerializeField] PostProcessVolume _ppv;

    [SerializeField] string _titleTags = "<incr>$</incr>";
    [SerializeField] string _messageTags = "<bounce>$</bounce>";

    Timer2 _fadeTimer = new Timer2(1f);
    Coroutine _fadeRoutine;

    // Transition
    //----------------------------------------------------------------------------------------------------
    public void Transition(Action onComplete,
                           float leadDelay, bool waitForInput,
                           bool fadeIn, float fadeTime,
                           string title, string message)
    {
        Transition(onComplete,
                   leadDelay, waitForInput,
                   fadeIn, fadeTime,
                   title, message,
                   _defaultTitleColor);
    }

    public void Transition(Action onComplete,
                           float leadDelay, bool waitForInput,
                           bool fadeIn, float fadeTime,
                           string title, string message,
                           Color titleColor)
    {
        if(_fadeRoutine != null)
            return;

        _title.color = titleColor;
        if(!title.IsNullOrEmpty())
        {
            _title.color = _title.color.WithA(0f);
            string[] t0 = _titleTags.Split('$');
            _title.text = $"{t0[0]}{title}{t0[1]}";
            _title.gameObject.SetActive(true);
        }
        else
        {
            _title.text = string.Empty;
            _title.gameObject.SetActive(false);
        }

        if(!message.IsNullOrEmpty())
        {
            _message.color = _message.color.WithA(0f);
            string[] t0 = _messageTags.Split('$');
            _message.text = $"{t0[0]}{message}{t0[1]}";
            _message.gameObject.SetActive(true);
        }
        else
        {
            _message.text = string.Empty;
            _message.gameObject.SetActive(false);
        }

        _fadeTimer.Reset(fadeTime);
        _panel.color = RGB.black;
        _canvasGroup.alpha = 0f;
        _ppv.weight = fadeIn ? 0f : 1f;
        _ppv.gameObject.SetActive(true);

        gameObject.SetActive(true);

        _fadeRoutine = StartCoroutine(FadeInRoutine(onComplete, leadDelay, fadeIn, waitForInput));
    }

    IEnumerator FadeInRoutine(Action onComplete, float leadDelay, bool fadeIn, bool waitForInput)
    {
        _canvasGroup.alpha = fadeIn ? 0f : 1f;

        float leadTime = 0f;
        while(leadTime < leadDelay)
        {
            float percent = (leadTime / leadDelay).Clamp01();
            float lerp = EASE.Evaluate(percent, EaseType.InQuad);

            if(_title.gameObject.activeInHierarchy) _title.color = _title.color.WithA(lerp);
            if(_message.gameObject.activeInHierarchy) _message.color = _message.color.WithA(lerp);

            leadTime += Time.deltaTime;
            yield return null;
        }

        EaseType easing = fadeIn ? EaseType.InQuad : EaseType.InOutQuad;
        bool skip = false;
        while(!_fadeTimer.hasFinished)
        {
            if(INPUT.leftMouse.down)
            {
                skip = true;
                break;
            }

            _fadeTimer.Step(Time.deltaTime);
            float lerp = EASE.Evaluate(fadeIn ? _fadeTimer.percent : 1f - _fadeTimer.percent, easing);
            _canvasGroup.alpha = lerp;
            _ppv.weight = lerp;
            yield return null;
        }
        
        if(skip)
        {
            _canvasGroup.alpha = fadeIn ? 1f : 0f;
            _ppv.weight = fadeIn ? 1f : 0f;
        }
        
        if(!fadeIn) _ppv.gameObject.SetActive(false);

        if(!skip && waitForInput)
        {
            float wait = 3f;
            while(true)
            {
                if(INPUT.leftMouse.down)
                    break;
                wait -= Time.deltaTime;
                if(wait <= 0f)
                    break;
                yield return null;
            }
        }

        _fadeRoutine = null;
        onComplete();
    }
}
