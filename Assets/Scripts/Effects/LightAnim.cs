#region Usings
using Framework;
using UnityEngine;
using MathBad;
#endregion
[RequireComponent(typeof(Light))]
public class LightAnim : MonoBehaviour
{
    [Header("Animate PingPong")]
    [SerializeField] EaseType _easingType = EaseType.InOutExpo;
    [Space]
    [SerializeField] bool _animateIntensity = true;
    [SerializeField] FloatRange _loopDuration = new FloatRange(0.1f, 0.25f);
    [SerializeField] FloatRange _intensityRange = new FloatRange(0.0f, 1f);

    [Space]
    [SerializeField] bool _animateColor = true;
    [SerializeField] Gradient _colorPool;

    [Header("Camera Distance")]
    [Space]
    [SerializeField] bool _cameraDst = true;
    [SerializeField] FloatRange _cameraDstMinMax = new FloatRange(1f, 5f);
    [SerializeField] FloatRange _cameraDstFactor = new FloatRange(0.75f, 1f);

    Light _light;
    FloatAnim _anim;
    float _lastIntensity, _targetIntensity;
    float _lastColor, _targetColor;
    Camera _camera;

    // MonoBehaviour
    //----------------------------------------------------------------------------------------------------
    void Awake()
    {
        _camera = Camera.main;
        _anim = new FloatAnim(_easingType, LoopType.PingPong, 0.1f);
        _light = GetComponent<Light>();
        _targetIntensity = _light.intensity;
        _anim.onLoop += NextTarget;
        NextTarget();
    }

    void Update() {Step(Time.deltaTime);}

    // Step
    //----------------------------------------------------------------------------------------------------
    void Step(float dt)
    {
        _anim.Step(dt);
        if(_animateIntensity) _light.intensity = Mathf.Lerp(_lastIntensity, _targetIntensity, _anim.percent.Clamp01());
        if(_animateColor) _light.color = _colorPool.Evaluate(Mathf.Lerp(_lastColor, _targetColor, _anim.percent.Clamp01()));
        if(_cameraDst && _camera != null)
        {
            float dst = Vector3.Distance(_camera.transform.position, transform.position);
            float dstPercent = Mathf.InverseLerp(_cameraDstMinMax.Min, _cameraDstMinMax.Max, dst);
            _light.intensity *= _cameraDstFactor.GetProportion(dstPercent);
        }
    }

    void NextTarget()
    {
        _lastColor = _targetColor;
        _targetColor = RNG.Float();
        _lastIntensity = _targetIntensity;
        _targetIntensity = _intensityRange.ChooseRandom();
        _targetIntensity = _targetIntensity.Max(0);
        _anim.SetDuration(_loopDuration.ChooseRandom());
    }

    void OnDrawGizmos()
    {
        if(_cameraDst)
        {
            GIZMOS.WireSphere(transform.position, _cameraDstMinMax.Min, RGB.white);
            GIZMOS.WireSphere(transform.position, _cameraDstMinMax.Max, RGB.darkGrey);
        }
    }
}
