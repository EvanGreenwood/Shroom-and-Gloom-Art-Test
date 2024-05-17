#region Usings
using System.Collections.Generic;
using UnityEngine;
using MathBad;
using Unity.Mathematics;
#endregion
[System.Serializable]
struct TunnelColorKey
{
    public Color color;
    [Range(0f, 1f)]
    public float pos;

    public TunnelColorKey(Color color, float pos)
    {
        this.color = color;
        this.pos = pos;
    }
}
[RequireComponent(typeof(TunnelGenerator))]
public class TunnelStyle : MonoBehaviour
{
    [SerializeField, HideInInspector]
    TunnelGenerator _generator;
    [SerializeField]
    List<TunnelColorKey> _colorKeys = new List<TunnelColorKey>();

    public TunnelGenerator tunnelGenerator
    {
        get
        {
            if(_generator == null)
                _generator = GetComponent<TunnelGenerator>();
            return _generator;
        }
    }

    public void AddColorKey()
    {
        float pos = _colorKeys.Count > 0 ? (_colorKeys.Last().pos + 0.1f).Clamp01() : 0f;
        _colorKeys.Add(new TunnelColorKey(RGB.white, pos));
    }
    public void GetColor(Vector3 worldPos)
    {
        Vector3 splinePos = _generator.GetClosestPoint(worldPos);
        float t = 0f;
        _generator.Spline.Evaluate(t.Clamp01(), out float3 pos, out float3 tangent, out float3 upVector);
    }

    // MonoBehaviour
    //----------------------------------------------------------------------------------------------------
    void Awake() { }
    void Update() { }
}
