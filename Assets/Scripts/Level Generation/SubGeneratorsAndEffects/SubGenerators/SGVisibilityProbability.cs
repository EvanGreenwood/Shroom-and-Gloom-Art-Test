#region Usings
using UnityEngine;
using MathBad;
#endregion

public class SGVisibilityProbability : SubGenerator
{
  [SerializeField, Range(0f, 1f)] float _probability = 0.25f;
  
  // MonoBehaviour
  //----------------------------------------------------------------------------------------------------
  public override void Generate()
  {
    if(RNG.Probability(1f-_probability))
    {
      gameObject.SetActive(false);
    }
  }
}