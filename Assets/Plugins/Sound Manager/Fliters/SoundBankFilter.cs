using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public abstract class SoundBankFilter
{
    public bool IsEnabled { get { return _isEnabled; } }

    [SerializeField]
    protected bool _isEnabled;

    public virtual void OnAdded(EffectSoundInstance sound)
    {

    }

    public virtual void OnRemoved(EffectSoundInstance sound)
    {

    }

    public virtual void Update(float deltaTime)
    {

    }

    public virtual void Reset()
    {

    }

    public virtual float GetVolumeMultiplier(float time, float clipLength)
    {
        return 1f;
    }

    public virtual float GetPitchMultiplier(float time, float clipLength)
    {
        return 1f;
    }
}
