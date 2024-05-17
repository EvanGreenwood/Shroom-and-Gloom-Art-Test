#region
using UnityEngine;
#endregion

namespace MathBad
{
[CreateAssetMenu(menuName = "Banks/SpriteBank", fileName = "SpriteBank", order = 0)]
[MenuCreate("Banks/SpriteBank")]
public class SpriteBank : ScriptableBank<Sprite> { }
}
