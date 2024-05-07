#region Usings
using EasyButtons;
using UnityEngine;
using Framework;
using Mainframe;
#endregion

[RequireComponent(typeof(SpriteRenderer))]
public class RandomSprite : SubGenerator
{
  [SerializeField] Sprite[] _sprites;

  // MonoBehaviour
  //----------------------------------------------------------------------------------------------------
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
    
    Die();
  }

  [Button]
  void ChooseRandom()
  {
    SpriteRenderer sr = GetComponent<SpriteRenderer>();
    sr.sprite = _sprites.ChooseRandom();
  }
  
  // Die
  //----------------------------------------------------------------------------------------------------
  void Die()
  {
    //dont destroy since we may want to undo.
    gameObject.SetActive(false);
  }

  protected override bool Flip(FlipMode flipMode)
  {
      return DefaultPosFlip(flipMode) || DefaultSpriteFlip(flipMode);
  }
}
