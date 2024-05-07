#region Usings
using UnityEngine;
using Framework;
using NaughtyAttributes;
using Unity.Mathematics;

#endregion

public class SGRandomRotation : SubGenerator
{
  [SerializeField] FloatRange _angle = new FloatRange(0f, 0f);

  // MonoBehaviour
  //----------------------------------------------------------------------------------------------------
  public override void Generate()
  {
    //reset then rotate
    transform.rotation = quaternion.identity;
    Rotate();
  }

  [Button]
  void Rotate() {transform.Rotate(0f, 0f, _angle.ChooseRandom());}
}
