using PowerTools;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(SpriteAnim))]
public class RandomAnimationStartTime : MonoBehaviour
{
    private SpriteAnim _anim;

    void Start()
    {
        _anim = GetComponent<SpriteAnim>();
        _anim.NormalizedTime = Random.value;
    }
}
