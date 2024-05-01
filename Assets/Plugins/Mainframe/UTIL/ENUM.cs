//  ___  _  _  _   _  __  __                                                                          
// | __|| \| || | | ||  \/  |                                                                         
// | _| | .` || |_| || |\/| |                                                                         
// |___||_|\_| \___/ |_|  |_|                                                                         
//                                                                                                    
//----------------------------------------------------------------------------------------------------

#region
using System;
#endregion

namespace Mainframe
{
public class ENUM
{
  /// <summary>
  /// Parse as Enum of type T
  /// </summary>
  public static bool TryParse<T>(string s, out T t) where T : Enum
  {
    if(!s.IsNullOrWhiteSpace()
    && Enum.TryParse(typeof(T), s, false, out object result))
    {
      t = (T)result;
      return true;
    }

    t = default;
    return false;
  }

  public static bool Contains<T>(T[] items, string name) where T : Enum
  {
    if(items.IsNullOrEmpty())
      return false;

    foreach(T item in items)
    {
      if(item.ToString() == name.MakeEnumCompatible())
        return true;
    }
    return false;
  }

  /// <summary>
  /// Get Count of Enum values.
  /// </summary>
  public static int Count<TEnum>() where TEnum : Enum => Enum.GetValues(typeof(TEnum)).Length;

  /// <summary>
  /// Get the array of Enum values.
  /// </summary>
  public static TEnum[] GetValues<TEnum>() where TEnum : Enum
  {
    Array source = Enum.GetValues(typeof(TEnum));
    TEnum[] values = new TEnum[source.Length];
    for(int i = 0; i < source.Length; i++)
      values[i] = (TEnum)source.GetValue(i);
    return values;
  }

  /// <summary>
  /// Get a random enum value
  /// </summary>
  public static TEnum Random<TEnum>() where TEnum : Enum
  {
    Array values = Enum.GetValues(typeof(TEnum));
    return (TEnum)values.GetValue(RNG.Int(0, values.Length));
  }
}
}
