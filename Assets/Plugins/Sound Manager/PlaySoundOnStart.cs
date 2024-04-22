using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Framework;

public class PlaySoundOnStart : MonoBehaviour
{
    [SerializeField] private bool _onStart = true;
    [SerializeField] private bool _onEnable = false;
    [SerializeField] private bool _onDestroy = false;
    [SerializeField] private bool _subsequentlyOnEnable = false;
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

    void Start()
    {
        if (_onStart)
        {
            PlaySound();
            if (_subsequentlyOnEnable) _onEnable = true;
        }
    }
    private void OnEnable()
    {
        if (_onEnable)
        {
            PlaySound();
        }
    }
    private void OnDestroy()
    {
        if (_onDestroy)
        {
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
