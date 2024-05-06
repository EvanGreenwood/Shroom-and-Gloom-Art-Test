#region Usings
using EasyButtons;
using UnityEngine;
using Framework;
using Mainframe;
#endregion
[RequireComponent(typeof(SpriteRenderer))]
public class RandomFlipSprite : MonoBehaviour
{
  [SerializeField] Bit2 _axis;

  // MonoBehaviour
  //----------------------------------------------------------------------------------------------------
  void Awake() {Flip();}

  [Button]
  void Flip()
  {
    SpriteRenderer sr = GetComponent<SpriteRenderer>();

    if(_axis.x && RNG.Probability(0.5f)) sr.flipX = !sr.flipX;
    if(_axis.y && RNG.Probability(0.5f)) sr.flipY = !sr.flipY;
  }
}
