#region Usings
using UnityEngine;
using Framework;
using MathBad;
using NaughtyAttributes;
#endregion

public class RandomSprite : SubGenerator
{
  [SerializeField] Sprite[] _sprites;

  private SpriteRenderer _spriteRenderer;
  // MonoBehaviour
  //----------------------------------------------------------------------------------------------------

  private void Awake()
  {
    gameObject.SetActive(true);
    _spriteRenderer = GetComponent<SpriteRenderer>();
  }

  public override void Generate() 
  {
    if(_sprites.IsNullOrEmpty())
    {
      Die();
      return;
    }
    
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
