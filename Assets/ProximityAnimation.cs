using Framework;
using PowerTools;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProximityAnimation : ProximityTrigger
{
    [Header("Animations")]
    public AnimationClip BeginIdle;
    public AnimationClip BeginToEnd;
    public AnimationClip EndToBegin;
    public AnimationClip EndIdle;

    [Range(0,1)]
    public float DisableChance;

    private SpriteAnim _anim;

    private Coroutine _currentTransition;
    
    private void Start()
    {
        _anim = GetComponent<SpriteAnim>();
        Debug.Assert(BeginIdle.isLooping, $"BeginIdle animation clip not set to looping, {BeginIdle.name}", this);
        Debug.Assert(!BeginToEnd.isLooping, $"BeginToEnd animation clip set to looping, but is a transition, {BeginToEnd.name}", this);
        Debug.Assert(EndIdle.isLooping, $"EndIdle animation clip not set to looping, {EndIdle.name}", this);
        
        _anim.Play(BeginIdle);
    }

    public override void ProximityEnter(GameObject entered)
    {
        //Debug.Log("ENTER");
        //TODO: if proximity enter and exit is called quickly animation will jump
        _anim.Play(BeginToEnd);
        _currentTransition = CoroutineUtils.StartCoroutine(WaitTillCurrentAnimationComplete(() =>
        {
            _anim.Play(EndIdle);
        }));
    }

    public override void ProximityExit(GameObject exited)
    {
        //Debug.Log("EXIT");
        //TODO: if proximity enter and exit is called quickly animation will jump
        _anim.Play(EndToBegin);
        _currentTransition = CoroutineUtils.StartCoroutine(WaitTillCurrentAnimationComplete(() =>
        {
            _anim.Play(BeginIdle);
        }));
    }
    
    private IEnumerator WaitTillCurrentAnimationComplete(Action then)
    {
        while (_anim.NormalizedTime < 1)
        {
            yield return null;
        }
        then?.Invoke();
    }
}
