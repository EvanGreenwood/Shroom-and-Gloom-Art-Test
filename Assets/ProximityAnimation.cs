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
    public AnimationClip EndIdle;

    [Range(0,1)]
    public float DisableChance;

    private SpriteAnim _anim;
    
    private void Start()
    {
        _anim = GetComponent<SpriteAnim>();
    }

    public override void ProximityEnter(GameObject entered)
    {
        throw new System.NotImplementedException();
    }

    public override void ProximityExit(GameObject exited)
    {
        throw new System.NotImplementedException();
    }
}
