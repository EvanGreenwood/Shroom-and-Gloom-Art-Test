using System.Collections;
using System.Collections.Generic;
using Framework;
using PowerTools;
using UnityEngine;

public class RandomAnimationClip : MonoBehaviour
{
    [SerializeField] private AnimationClip[] _animClips;
    private SpriteAnim _spriteAnim;

    private void Awake()
    {
        _spriteAnim = GetComponent<SpriteAnim>();
        _spriteAnim.Play(_animClips.ChooseRandom());
        Destroy(this);
    }
}
