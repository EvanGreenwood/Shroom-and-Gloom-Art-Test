#region Usings
using UnityEngine;
using MathBad;
using Framework;
#endregion

[RequireComponent(typeof(Light))]
public class TunnelLight : MonoBehaviour
{
    [SerializeField] Light _light;
    [SerializeField] SpriteRenderer _flare;
    [SerializeField] FloatRange _intensityMinMax = new FloatRange(0.75f, 1f);
    [SerializeField] FloatRange _rangeMinMax = new FloatRange(4.5f, 5.5f);

    // Init
    //----------------------------------------------------------------------------------------------------
    public void Init(Color color)
    {
        _light.color = color;
        _light.intensity = _intensityMinMax.ChooseRandom();
        _light.range = _rangeMinMax.ChooseRandom();
    }
}
