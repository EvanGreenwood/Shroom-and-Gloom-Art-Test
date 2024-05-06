#region Usings
using EasyButtons;
using UnityEngine;
using Framework;
using Mainframe;
#endregion

[RequireComponent(typeof(SpriteRenderer))]
public class RandomSprite : MonoBehaviour
{
  [SerializeField] Sprite[] _sprites;

  // MonoBehaviour
  //----------------------------------------------------------------------------------------------------
  void Awake()
  {
    if(_sprites.IsNullOrEmpty())
    {
      Die();
      return;
    }

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
    Destroy(this);
  }

}
