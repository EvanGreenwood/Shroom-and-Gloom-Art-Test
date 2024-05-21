#region Usings
using MathBad;
using UnityEngine;
using NaughtyAttributes;
using System;
using Unity.Mathematics;
using UnityEngine.Splines;
#endregion

[RequireComponent(typeof(Light))]
public class TunnelLight : TunnelRigComponent
{
    [SerializeField] private Light _light;
    
    public Light Light => _light;
    
    void Start()
    {
        if (_tunnelRig)
        {
            _tunnelRig.RegisterLight(this);
        }

        if(Light == null)
        {
            _light = GetComponent<Light>();
        }
    }

    public override void SetRig(TunnelRig rig)
    {
        base.SetRig(rig);
        _tunnelRig.SetTunnelLightFromTransform(this, transform);
        _light = GetComponent<Light>();
    }

    private void OnDestroy()
    {
        if(_tunnelRig == null)
            return;
        _tunnelRig.RemoveLight(this);
    }

    public override bool UsesAngleDistance()
    {
        return true;
    }
}
