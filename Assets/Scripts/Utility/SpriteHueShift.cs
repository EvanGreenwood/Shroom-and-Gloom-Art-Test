#region Usings
using UnityEngine;
using Mainframe;
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
    // MonoBehaviour
    //----------------------------------------------------------------------------------------------------
    void Awake()
    {
        _sr = GetComponent<SpriteRenderer>();
        _anim = new FloatAnim(_easeType, _loopType, _duration);
    }
    void Update()
    {
        _anim.Step(Time.deltaTime);
        _sr.color = RGB.Hue(_anim.lerp);
    }
}
