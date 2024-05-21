using MathBad;
using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.Splines;

public abstract class TunnelRigComponent : MonoBehaviour
{
    [SerializeField][HideInInspector] protected TunnelRig _tunnelRig;
    [SerializeField][HideInInspector] protected float _splinePercent;
    [SerializeField][HideInInspector] protected float _angle;
    [SerializeField][HideInInspector] protected float _distance;
    
    public TunnelRig TunnelRig => _tunnelRig;
    public float Angle { get => _angle; set => _angle = value; }
    public float Distance { get => _distance; set => _distance = value; }
    public float SplinePercent { get => _splinePercent; set => _splinePercent = value; }
    
    public abstract bool UsesAngleDistance();
   
    void Awake()
    {
        if(_tunnelRig == null)
        {
            _tunnelRig = GetComponentInParent<TunnelRig>();
        }
    }
    
    public void Refresh()
    {
        if(_tunnelRig == null)
            return;
        _tunnelRig.spline.Evaluate(SplinePercent, out float3 pos, out float3 tangent, out float3 up);

        if (UsesAngleDistance())
        {
            float3 dir = Quaternion.AngleAxis(MATH.Normalize_360(Angle), tangent) * up;
            float3 nextPos = pos + dir * Distance;
            transform.rotation = Quaternion.LookRotation(tangent, up);
            transform.position = nextPos;
        }
        else
        {
            transform.rotation = Quaternion.LookRotation(tangent, up);
            transform.position = pos;
        }
    }
    
    public virtual void SetRig(TunnelRig rig)
    {
        _tunnelRig = rig;
    }
}
