#region Usings
using UnityEngine;
using MathBad;
#endregion

public class SpriteHueShift : MonoBehaviour
{
    [SerializeField, Range(0f, 1f)] float _minHue = 0f;
    [SerializeField, Range(0f, 1f)] float _maxHue = 1f;
    [SerializeField] LoopType _loopType;
    [SerializeField] EaseType _easeType;
    [SerializeField] float _duration;
    FloatAnim _anim;
    SpriteRenderer _sr;
    float _alpha;
    // MonoBehaviour
    //----------------------------------------------------------------------------------------------------
    void Awake()
    {
        _sr = GetComponent<SpriteRenderer>();
        _alpha = _sr.color.a;
        _anim = new FloatAnim(_easeType, _loopType, _duration);
    }
    void Update()
    {
        _anim.Step(UnityEngine.Time.deltaTime);
        _sr.color = RGB.Hue(_anim.lerp).WithA(_alpha);
    }
}
