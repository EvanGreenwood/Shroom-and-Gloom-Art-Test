#region Usings
using UnityEngine;
using Framework;
using MathBad;
#endregion

public class VisibilityProbability : MonoBehaviour
{
  [SerializeField, Range(0f, 1f)] float _probability = 0.25f;
  
  // MonoBehaviour
  //----------------------------------------------------------------------------------------------------
  void Awake()
  {
    if(RNG.Probability(1f-_probability))
    {
      gameObject.SetActive(false);
    }
  }
}
