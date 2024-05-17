using System.Collections;
using MathBad;
using TMPro;
using UnityEngine;
public class SpeechBubble : MonoBehaviour
{
    [SerializeField] TextMeshPro _label;
    [SerializeField] ParticleSystem _ps;
    string _text;
    int _charIndex = 0;
    bool _hasInit;

    public void Init(string text, float duration)
    {
        _text = text;
        _label.text = "";

        StartCoroutine(ShowTextRoutine(duration));
        _hasInit = true;
    }
    void Update()
    {
        if(!_hasInit)
            return;
        transform.position += Vector3.up * (0.25f * UnityEngine.Time.deltaTime);
    }
    IEnumerator ShowTextRoutine(float duration)
    {
        while(true)
        {
            _label.text += _text[_charIndex];
            _charIndex++;
            if(_charIndex > _text.Length - 1)
                break;
            yield return WAIT.ForSeconds(0.05f);
        }

        yield return WAIT.ForSeconds(duration);

        Vector3 startScale = transform.localScale;
        float t = 0f;
        _ps.Stop();
        while(t < 1f)
        {
            t += UnityEngine.Time.deltaTime;
            transform.localScale = Vector3.Lerp(startScale, Vector3.zero, EASE.Evaluate((t / duration).Clamp01(), EaseType.OutCubic));
            yield return null;
        }

        Destroy(gameObject);
    }
}
