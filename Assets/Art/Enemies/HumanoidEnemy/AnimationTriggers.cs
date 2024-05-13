using MathBad;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimationTrigger : MonoBehaviour
{
    public Animator Animator;
    public string TriggerName;

    //Hacky for quick test
    public void OnCollisionEnter(Collision other)
    {
        //Debug.Log($"Enter: {other.gameObject.name}");
        if (other.gameObject.GetComponentInParent<Player>())
        {
            Animator.SetTrigger(TriggerName);
        }
    }
}
