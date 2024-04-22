using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.Audio;

[CreateAssetMenu(fileName = "New Effect Bank", menuName = "Sound Bank/Effect Bank")]
public class EffectSoundBank : SingleSoundBank
{

    public EffectSoundInstance Fetch(ISoundPool soundPool)
    {
        EffectSoundInstance sound = soundPool.FetchFromPool<EffectSoundInstance>();

        sound.SetClip(GetNextClip());
        sound.SetRolloffDistance(_rolloffDistance.Min, _rolloffDistance.Max);
        sound.SetMixerGroup(_outputMixer);
        sound.SetBaseVolume(_volumeRange.ChooseRandom());
        sound.SetBasePitch(_pitchRange.ChooseRandom());

        AddFilters(sound);

        return sound;
    }

#if UNITY_EDITOR
    public override SoundInstance TestInEditor(ISoundPool soundPool)
    {
        EffectSoundInstance sound = Fetch(soundPool);

        if (sound != null && TimeSinceLastPlayed > _cooldown)
        {
            sound.Play2D();
            OnPlayed(sound);

            return sound;
        }

        return null;
    }
#endif

    [ContextMenu("Apply Default Values")]
    public override void ApplyDefaultValues()
    {
        base.ApplyDefaultValues();

        _volumeRange = DefaultSoundBankValues.Instance.EffectBank.VolumeRange;
        _pitchRange = DefaultSoundBankValues.Instance.EffectBank.PitchRange;
        _rolloffDistance = DefaultSoundBankValues.Instance.EffectBank.RolloffDistance;
        _cooldown = DefaultSoundBankValues.Instance.EffectBank.Cooldown;
        _outputMixer = DefaultSoundBankValues.Instance.EffectBank.OutputMixer;
        _clipSelection = DefaultSoundBankValues.Instance.EffectBank.ClipSelection;
    }

}

public static class EffectSoundBankExtenstions
{
    /// <summary>
    /// Fetches and then plays a non-spatial sound, ie. one that does not emit from a specific location and rolloff.
    /// </summary>
    /// <param name="soundBank">The sound bank used to set-up the sound instance</param>
    public static EffectSoundInstance Play(this EffectSoundBank soundBank)
    {
        if (soundBank != null && soundBank.TimeSinceLastPlayed > soundBank.Cooldown)
        {
            EffectSoundInstance sound = soundBank.Fetch(RuntimeSoundPool.Instance);
            sound.Play2D();
            soundBank.OnPlayed(sound);

            return sound;
        }

        return null;
    }

    /// <summary>
    /// Fetches and then plays a sound at a specific position.
    /// </summary>
    /// <param name="position">The world-space position of the sound emission</param>
    public static EffectSoundInstance Play(this EffectSoundBank soundBank, Vector3 position)
    {
        if (soundBank != null && soundBank.TimeSinceLastPlayed > soundBank.Cooldown)
        { 
            EffectSoundInstance sound = soundBank.Fetch(RuntimeSoundPool.Instance);
            sound.Play3D(position);
            soundBank.OnPlayed(sound);

            return sound;
        }

        return null;
    }

    /// <summary>
    /// Fetches and then plays a sound that will follow a transform around.
    /// </summary>
    /// <param name="followTransform">The transform to follow</param>
    public static EffectSoundInstance Play(this EffectSoundBank soundBank, Transform followTransform)
    {
        if (soundBank != null && soundBank.TimeSinceLastPlayed > soundBank.Cooldown)
        {
            EffectSoundInstance sound = soundBank.Fetch(RuntimeSoundPool.Instance);
            sound.Play3D(followTransform);
            soundBank.OnPlayed(sound);
 
            return sound;
        }

        return null;
    }

    public static int IndexOf(this EffectSoundBank soundBank, EffectSoundInstance instance)
    {
        for (int i = 0; i < soundBank._audioClips.Length; i++)
        {
            if (instance.Clip == soundBank._audioClips[i])
            {
                return i;
            }
        }
        return -1;
    }
}

