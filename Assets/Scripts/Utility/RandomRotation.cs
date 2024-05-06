#region Usings
using EasyButtons;
using UnityEngine;
using Framework;
#endregion

public class RandomRotation : MonoBehaviour
{
  [SerializeField] FloatRange _angle = new FloatRange(0f, 0f);

  // MonoBehaviour
  //----------------------------------------------------------------------------------------------------
  void Awake() {Rotate();}

  [Button]
  void Rotate() {transform.Rotate(0f, 0f, _angle.ChooseRandom());}
}
