 
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Framework;

public class PlaySoundAtIntervals : MonoBehaviour
{
    [SerializeField] private float _interval = 2;
    [SerializeField] private float _intervalVariance = 0.3f;
    private float _counter = 0;
    public enum SoundPosition
    {
        TwoDimensional,
        TransformPosition,
        FollowTransform
    }

    [SerializeField]
    private SoundBank _soundBank;

    [SerializeField]
    private SoundPosition _position;

    [SerializeField]
    private bool _loop;

    [SerializeField]
    [MinValue(0)]
    private float _fadeInDuration = 0;

    private void Start()
    {
        _counter = _interval * (1 - _intervalVariance * 0.5f + Random.value * _intervalVariance);
    }
    private void Update()
    {
        _counter -= Time.deltaTime;
        if (_counter <= 0)
        {
            _counter = _interval * (1 - _intervalVariance * 0.5f + Random.value * _intervalVariance);
            PlaySound();
        }
    }
    private void PlaySound()
    {
        SoundInstance sound = null;

        switch (_position)
        {
            case SoundPosition.TwoDimensional:
                sound = _soundBank.Play(transform.position); 
                break;
            case SoundPosition.TransformPosition:
                sound = _soundBank.Play(transform.position);
                break;
            case SoundPosition.FollowTransform:
                sound = _soundBank.Play(transform);
                break;
        }

        if (_loop)
        {
            EffectSoundInstance effectSound = sound as EffectSoundInstance;
            if (effectSound != null)
            {
                effectSound.SetLooping(true);
            }
        }

        if (sound != null && _fadeInDuration > 0)
        {
            sound.FadeIn(_fadeInDuration, true);
        }
    }
}
