#region Usings
using UnityEngine;
using Framework;
using NaughtyAttributes;
using Unity.Mathematics;

#endregion

public class SGRandomYPos : SubGenerator
{
  [SerializeField] FloatRange _yOffset = new FloatRange(0f, 0f);

  // MonoBehaviour
  //----------------------------------------------------------------------------------------------------
  public override void Generate()
  {
    //reset then rotate
    //transform.rotation = Quaternion.identity;
    Offset();
  }

  [Button]
  void Offset() { transform.Translate(0f, _yOffset.ChooseRandom(), 0f);}
}
