#region Usings
using Framework;
using UnityEngine;
using MathBad;
#endregion

// Testing rotating tunnel peices with some 1D perlin over length of tunnel
public class SGTestNoise : SubGenerator
{
    [SerializeField] SpriteRenderer[] _renderers;
    [SerializeField] FloatRange _randomAngleMinMax = new FloatRange(-25, 25);
    [SerializeField] float _angleNoiseScale = 2f;
    [SerializeField] FloatRange _yOffsetMinMax = new FloatRange(-0.5f, 0.5f);
    [SerializeField] float _yOffsetNoiseScale = 2f;
    // MonoBehaviour
    //----------------------------------------------------------------------------------------------------
    public override void Generate()
    {
        for(int i = 0; i < _renderers.Length; i++)
        {
            float lerp = i / _renderers.Length._float();
            SpriteRenderer sr = _renderers[i];
            float angleNoise = Mathf.PerlinNoise1D(lerp * _angleNoiseScale);
            float angle = Mathf.Lerp(_randomAngleMinMax.Min, _randomAngleMinMax.Max, angleNoise);
            sr.transform.Rotate(new Vector3(0f, 0f, angle), Space.Self);

            float posNoise = Mathf.PerlinNoise1D(lerp * _yOffsetNoiseScale);
            float yOffset = Mathf.Lerp(_yOffsetMinMax.Min, _yOffsetMinMax.Max, posNoise);
            sr.transform.position += new Vector3(0f, yOffset, 0f);
        }
    }
}
