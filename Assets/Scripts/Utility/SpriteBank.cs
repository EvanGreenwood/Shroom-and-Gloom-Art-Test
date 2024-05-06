#region Usings
using Framework;
using UnityEngine;
#endregion

[CreateAssetMenu(menuName = "Create SpriteBank", fileName = "SpriteBank", order = 0)]
public class SpriteBank : ScriptableObject
{
  public Sprite[] sprites;

  public Sprite ChooseRandom() {return sprites.ChooseRandom();}
}
