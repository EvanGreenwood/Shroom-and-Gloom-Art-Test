#region Usings
using UnityEngine;
using MathBad;
using Framework;
#endregion

public class TunnelLight : MonoBehaviour
{
    [SerializeField] Light _light;
    [SerializeField] SpriteRenderer _flare;
    [SerializeField] Gradient _colorRange;
    [SerializeField] FloatRange _intensityMinMax = new FloatRange(0.75f, 1f);
    [SerializeField] FloatRange _rangeMinMax = new FloatRange(4.5f, 5.5f);
    [SerializeField] FloatRange _flareAlphaMinMax = new FloatRange(0.05f, 0.1f);
    [SerializeField] float _flareHueVariance = 0.05f;

    void Awake()
    {
        float intensity = _intensityMinMax.ChooseRandom();
        float range = _rangeMinMax.ChooseRandom();
        Color color = _colorRange.Evaluate(RNG.Float());

        _light.intensity = intensity;
        _light.range = range;
        _light.color = color;

        _flare.color = color.WithHueShift(RNG.FloatVariance(0f, _flareHueVariance)).WithA(_flareAlphaMinMax.ChooseRandom());
        _flare.transform.localScale = new Vector3(RNG.FloatVariance(range, 0.15f),
                                                  RNG.FloatVariance(range, 0.15f),
                                                  1f);
    }
}
