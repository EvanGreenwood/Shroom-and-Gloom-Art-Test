#region Usings
using EasyButtons;
using UnityEngine;
using Framework;
using Mainframe;
using System;

#endregion
[RequireComponent(typeof(SpriteRenderer))]

public class SGRandomFlipSprite : SubGenerator
{
  [SerializeField] Bit2 _axis;

  private bool flipXOriginal;
  private bool flipYOriginal;

  private SpriteRenderer _spriteRenderer;

  // MonoBehaviour
  //----------------------------------------------------------------------------------------------------

  private void Awake()
  {
    _spriteRenderer = GetComponent<SpriteRenderer>();
    flipXOriginal = _spriteRenderer.flipX;
    flipYOriginal = _spriteRenderer.flipY;
  }

  public override void Generate()
  {
    //reset then flip
    _spriteRenderer.flipX = flipXOriginal;
    _spriteRenderer.flipY = flipYOriginal;
    Flip();
  }

  [Button]
  void Flip()
  {
    if(_axis.x && RNG.Probability(0.5f)) _spriteRenderer.flipX = !_spriteRenderer.flipX;
    if(_axis.y && RNG.Probability(0.5f)) _spriteRenderer.flipY = !_spriteRenderer.flipY;
  }

  protected override bool Flip(FlipMode flipMode)
  {
    return DefaultPosFlip(flipMode);
  }
}
