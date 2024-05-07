#region Usings
using Framework;
using UnityEngine;
using Mainframe;
using Easing = Mainframe.Easing;
#endregion
[RequireComponent(typeof(Light))]
public class LightAnim : MonoBehaviour
{
  [SerializeField] FloatRange _loopDuration = new FloatRange(0.1f, 0.25f);
  [SerializeField] FloatRange _intensityRange = new FloatRange(0.0f, 1f);
  [SerializeField] Gradient _colorPool;

  Light _light;
  FloatAnim _anim = new FloatAnim(Easing.InOutExpo, LoopType.PingPong, 0.1f);
  float _lastIntensity, _targetIntensity;
  float _lastColor, _targetColor;

  // MonoBehaviour
  //----------------------------------------------------------------------------------------------------
  void Awake()
  {
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
    _light.intensity = Mathf.Lerp(_lastIntensity, _targetIntensity, _anim.percent.Clamp01());
    _light.color = _colorPool.Evaluate(Mathf.Lerp(_lastColor, _targetColor, _anim.percent.Clamp01()));
  }

  void NextTarget()
  {
    _lastColor = _targetColor;
    _targetColor = RNG.Float();
    _lastIntensity = _targetIntensity;
    _targetIntensity = _intensityRange.ChooseRandom();
    _targetIntensity = _targetIntensity.Max(0);
    _anim.Reset(_loopDuration.ChooseRandom());
  }
}
