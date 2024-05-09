#region Usings
using UnityEngine;
using Framework;
using MathBad;
using NaughtyAttributes;
using System;
#endregion

public class RandomSprite : SubGenerator
{
  [SerializeField] Sprite[] _sprites;

  private SpriteRenderer _spriteRenderer;
  // MonoBehaviour
  //----------------------------------------------------------------------------------------------------

  private void Awake()
  {
    _spriteRenderer = GetComponent<SpriteRenderer>();
  }

  public override void Generate() 
  {
    if(_sprites.IsNullOrEmpty())
    {
      Die();
      return;
    }
    
    //reset in case we have already run this.
    gameObject.SetActive(true);

    ChooseRandom();
  }

  [Button]
  void ChooseRandom()
  {
    _spriteRenderer.sprite = _sprites.ChooseRandom();
  }
  
  // Die
  //----------------------------------------------------------------------------------------------------
  void Die()
  {
    //dont destroy since we may want to undo.
    gameObject.SetActive(false);
  }
}
