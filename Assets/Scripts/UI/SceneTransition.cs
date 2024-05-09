#region Usings
using System;
using System.Collections;
using UnityEngine;
using MathBad;
using TMPro;
using UnityEngine.UI;
#endregion

public class SceneTransition : MonoSingletonUI<SceneTransition>
{
    [SerializeField] TextMeshProUGUI _title;
    [SerializeField] TextMeshProUGUI _message;
    [SerializeField] Image _panel;
    [SerializeField] CanvasGroup _canvasGroup;

    Timer2 _fadeTimer = new Timer2(1f);
    Coroutine _fadeRoutine;

    public void Transition(Action onComplete,
                           float leadDelay, bool waitForInput,
                           bool fadeIn, float fadeTime,
                           string title, string message,
                           Color titleColor, Color panelColor)
    {
        if(_fadeRoutine != null)
            return;

        _canvasGroup.alpha = fadeIn ? 0f : 1f;
        _fadeTimer.Reset(fadeTime);

        _title.color = titleColor;
        if(!title.IsNullOrEmpty())
        {
            _title.text = title;
            _title.gameObject.SetActive(true);
        }
        else
        {
            _title.text = string.Empty;
            _title.gameObject.SetActive(false);
        }

        if(!message.IsNullOrEmpty())
        {
            _message.text = message;
            _message.gameObject.SetActive(true);
        }
        else
        {
            _message.text = string.Empty;
            _message.gameObject.SetActive(false);
        }

        _panel.color = panelColor;

        gameObject.SetActive(true);
        _fadeRoutine = StartCoroutine(FadeInRoutine(onComplete, leadDelay, fadeIn, waitForInput));
    }

    IEnumerator FadeInRoutine(Action onComplete, float leadDelay, bool fadeIn, bool waitForInput)
    {
        yield return WAIT.ForSeconds(leadDelay);

        EaseType easing = fadeIn ? EaseType.InQuad : EaseType.OutQuad;

        while(!_fadeTimer.hasFinished)
        {
            _fadeTimer.Step(Time.deltaTime);
            float lerp = EASE.Evaluate(fadeIn ? _fadeTimer.percent : 1f - _fadeTimer.percent, easing);
            _canvasGroup.alpha = lerp;
            yield return null;
        }

        if(waitForInput)
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
