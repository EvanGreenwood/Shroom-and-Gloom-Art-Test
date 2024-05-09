//  ___              _                                                                                
// | _ ) __ _  _ _  | |__                                                                             
// | _ \/ _` || ' \ | / /                                                                             
// |___/\__,_||_||_||_\_\                                                                             
//                                                                                                    
//----------------------------------------------------------------------------------------------------

#region
using System;
using UnityEngine;
#endregion

namespace MathBad
{
public abstract class ScriptableBank<T> : ScriptableObject
{
  public T[] bank;

  public T this[int index]
  {
    get
    {
      if(bank.IndexOutOfRange(index))
        throw new IndexOutOfRangeException($"{nameof(index)} is out of range [0 ... {bank.Length}].");
      return bank[index];
    }
  }

  public T NextRandom() => bank.NextRandom();
  public int Length() => bank.Length;
}
}
