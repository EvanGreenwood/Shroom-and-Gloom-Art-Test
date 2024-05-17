#region Usings

using MathBad;
using UnityEngine;
using NaughtyAttributes;
using System;
using Unity.Mathematics;
using UnityEngine.Splines;

#endregion

[RequireComponent(typeof(Light))]
public class TunnelLight : MonoBehaviour
{
    [SerializeField] Light _light;
    [SerializeField] TunnelRig _rig;
    [SerializeField] float _splinePercent;
    [SerializeField] float _angle;
    [SerializeField] float _distance;

    public TunnelRig rig => _rig;
    public new Light light => _light;

    public float splinePercent { get => _splinePercent; set => _splinePercent = value; }
    public float angle { get => _angle; set => _angle = value; }
    public float distance { get => _distance; set => _distance = value; }

    public void Start()
    {
        if (_rig == null)
        {
            _rig = GetComponentInParent<TunnelRig>();
            _rig?.RegisterLight(this);
        }

        if (light == null)
        {
            _light = GetComponent<Light>();
        }
    }

    public void Refresh()
    {
        if(_rig == null)
            return;
        _rig.spline.Evaluate(splinePercent, out float3 pos, out float3 tangent, out float3 up);
        float3 dir = Quaternion.AngleAxis(MATH.Normalize_360(angle), tangent) * up;
        float3 nextPos = pos + dir * distance;
        transform.rotation = Quaternion.LookRotation(tangent, up);
        transform.position = nextPos;
    }

    public void Init(TunnelRig rig)
    {
        _rig = rig;
        _rig.SetTunnelLightFromTransform(this, transform);
        _light = GetComponent<Light>();
    }

    void OnDestroy()
    {
        if(_rig == null)
            return;
        _rig.RemoveLight(this);
    }
}
