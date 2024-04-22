using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Framework;

public class ImpactSoundInstance : EffectSoundInstance
{
    public float ImpactVelocity { get { return _impactVelocity; } set { SetImpactVelocity(value); } }
    public FloatRange VelocityRange { get { return _velocityRange; } set { SetVelocityRange(value.Min, value.Max); } }

    private float _impactVelocity;
    private FloatRange _velocityRange;

    public ImpactSoundInstance SetImpactVelocity(float impactVelocity)
    {
        _impactVelocity = Mathf.Max(0, impactVelocity);

        return this;
    }

    public ImpactSoundInstance SetVelocityRange(float minMagnitude, float maxMagnitude)
    {
        _velocityRange = new FloatRange(Mathf.Max(0, minMagnitude), Mathf.Max(0, maxMagnitude));

        return this;
    }

    
    public override string GetStatusString()
    {
        if (_audioSource.clip == null)
        {
            return "No clip";
        }

        return _audioSource.clip.name + " (Velocity: " + _velocityRange.GetProportion(_impactVelocity).ToString("P0") + ", Volume: " + BaseVolume.ToString("F2") + ", Pitch: " + BasePitch.ToString("F2") + ")";
    }
}
