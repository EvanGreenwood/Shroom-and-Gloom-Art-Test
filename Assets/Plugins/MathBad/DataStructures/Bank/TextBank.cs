#region Usings
using UnityEngine;
#endregion

namespace MathBad
{
[CreateAssetMenu(menuName = "Banks/TextBank", fileName = "TextBank", order = 0)]
[MenuCreate("Banks/TextBank")]
public class TextBank : ScriptableObject
{
    [Multiline]
    public string[] bank;
    public string NextRandom() => bank.NextRandom();
}
}
