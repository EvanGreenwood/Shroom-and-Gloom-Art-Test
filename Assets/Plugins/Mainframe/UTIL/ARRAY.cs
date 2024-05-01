//    _                          _   _  _    _  _                                                     
//   /_\   _ _  _ _  __ _  _  _ | | | || |_ (_)| |                                                    
//  / _ \ | '_|| '_|/ _` || || || |_| ||  _|| || |                                                    
// /_/ \_\|_|  |_|  \__,_| \_, | \___/  \__||_||_|                                                    
//                         |__/                                                                       
//----------------------------------------------------------------------------------------------------

#region
using System;
using System.ComponentModel;
#endregion

namespace Mainframe
{
public static class ARRAY
{
  public static T[] New<T>(params T[] items) => items;

  /// <summary>
  /// Method to get a copy of an array
  /// </summary>
  public static T[] Combine<T>(params T[][] arrays)
  {
    int length = 0;
    foreach(T[] array in arrays) length += array.Length;

    T[] result = new T[length];
    int offset = 0;
    foreach(T[] array in arrays)
    {
      Array.Copy(array, 0, result, offset, array.Length);
      offset += array.Length;
    }

    return result;
  }

  /// <summary>
  /// Creates a copy of an array
  /// </summary>
  public static T[] GetCopy<T>(T[] array)
  {
    if(array == null) throw new ArgumentNullException(nameof(array));

    T[] copy = new T[array.Length];
    Array.Copy(array, copy, array.Length);
    return copy;
  }

  /// <summary>
  /// Converts an array of one type to an array of another type
  /// </summary>
  public static TOut[] Convert<TIn, TOut>(TIn[] array) where TIn : IConvertible where TOut : IConvertible
  {
    if(array == null) throw new ArgumentNullException(nameof(array));

    TOut[] result = new TOut[array.Length];
    for(int i = 0; i < array.Length; i++) result[i] = (TOut)TypeDescriptor.GetConverter(typeof(TOut)).ConvertFrom(array[i]);
    return result;
  }

  /// <summary>
  /// Checks if an array contains a certain value
  /// </summary>
  public static bool Contains<T>(T[] array, T value) where T : IEquatable<T>
  {
    if(array == null) throw new ArgumentNullException(nameof(array));

    for(int i = 0; i < array.Length; i++)
      if(array[i].Equals(value))
        return true;
    return false;
  }

  /// <summary>
  /// Fisher-Yates shuffle
  /// </summary>
  public static T[] Shuffle<T>(T[] array)
  {
    if(array == null) throw new ArgumentNullException(nameof(array));

    for(int i = array.Length - 1; i > 0; i--)
    {
      int j = RNG.Int(i + 1);
      (array[i], array[j]) = (array[j], array[i]);
    }

    return array;
  }
}
}
