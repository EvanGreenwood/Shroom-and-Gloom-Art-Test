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
    [SerializeField] private Light _light;
    [SerializeField] private TunnelRig _rig;
    [SerializeField] private float _splinePercent;
    [SerializeField] private float _angle;
    [SerializeField] private float _distance;

    public TunnelRig Rig => _rig;
    public Light Light => _light;

    public float SplinePercent { get => _splinePercent; set => _splinePercent = value; }
    public float Angle { get => _angle; set => _angle = value; }
    public float Distance { get => _distance; set => _distance = value; }

    void Start()
    {
        if(_rig == null)
        {
            _rig = GetComponentInParent<TunnelRig>();
            _rig?.RegisterLight(this);
        }

        if(Light == null)
        {
            _light = GetComponent<Light>();
        }
    }

    public void Refresh()
    {
        if(_rig == null)
            return;
        _rig.spline.Evaluate(SplinePercent, out float3 pos, out float3 tangent, out float3 up);
        float3 dir = Quaternion.AngleAxis(MATH.Normalize_360(Angle), tangent) * up;
        float3 nextPos = pos + dir * Distance;
        transform.rotation = Quaternion.LookRotation(tangent, up);
        transform.position = nextPos;
    }

    public void Init(TunnelRig rig)
    {
        _rig = rig;
        _rig.SetTunnelLightFromTransform(this, transform);
        _light = GetComponent<Light>();
    }

    private void OnDestroy()
    {
        if(_rig == null)
            return;
        _rig.RemoveLight(this);
    }
}
