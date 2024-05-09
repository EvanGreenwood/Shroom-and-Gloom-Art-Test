#region
using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;
#endregion

namespace MathBad
{
public static class CollectionExt
{
  static NullReferenceException ThrowNullRef(string name) => new NullReferenceException($"{name}: Is null or empty.");

  [MethodImpl(256)] public static bool IsEmpty<T>(this IList<T> list) => list.Count == 0;
  [MethodImpl(256)] public static bool IsNullOrEmpty<T>(this IList<T> list) => list == null || list.Count == 0;
  [MethodImpl(256)] public static bool IsNullOrEmpty<TKey, TValue>(this IDictionary<TKey, TValue> dict) => dict == null || dict.Count == 0;
  [MethodImpl(256)] public static bool IsNullOrEmpty<T>(this Queue<T> queue) => queue == null || queue.Count == 0;
  [MethodImpl(256)] public static bool IsPopulated<T>(this IList<T> list) => list != null && list.Count > 0;
  [MethodImpl(256)] public static bool IsPopulated<T>(this Queue<T> queue) => queue != null && queue.Count > 0;
  [MethodImpl(256)] public static bool IndexOutOfRange<T>(this IList<T> list, int index) => index < 0 || index > list.Count - 1;

  [MethodImpl(256)] public static T Last<T>(this IList<T> list) => list[list.Count - 1];

  // Iterate
  //----------------------------------------------------------------------------------------------------
  public static void Foreach<T>(this IList<T> list, Action<T> content)
  {
    foreach(T item in list)
    {
      content(item);
    }
  }

  public static void Foreach<T>(this T[,] list, Action<T> content)
  {
    foreach(T item in list)
    {
      content(item);
    }
  }

  public static void For<T>(this IList<T> list, Action<int> content)
  {
    for(int i = 0; i < list.Count; i++)
    {
      content(i);
    }
  }

  public static T[] Copy<T>(this T[] array)
  {
    T[] copy = new T[array.Length];
    Array.Copy(array, copy, array.Length);
    return copy;
  }

  public static bool IncrementAndWrap<T>(this IList<T> list, ref int index)
  {
    index++;
    if(index > list.Count - 1)
    {
      index = 0;
      return true;
    }
    return false;
  }

  public static T GetLast<T>(this IList<T> list)
  {
    if(list.IsNullOrEmpty())
      throw ThrowNullRef(nameof(list));
    return list[list.Count - 1];
  }

  public static T PopLast<T>(this IList<T> list)
  {
    if(list.IsNullOrEmpty())
      throw ThrowNullRef(nameof(list));
    T last = list.GetLast();
    list.RemoveAt(list.Count - 1);
    return last;
  }

  public static T PopFirst<T>(this IList<T> list)
  {
    if(list.IsNullOrEmpty())
      throw ThrowNullRef(nameof(list));
    T first = list[0];
    list.RemoveAt(0);
    return first;
  }

  // Get Closest
  //----------------------------------------------------------------------------------------------------
  public static bool TryGetClosest<T>(this IList<T> items, out T t, Vector3 position) where T : Component => items.TryGetClosest(out t, null, position, items.Count - 1);
  public static bool TryGetClosest<T>(this IList<T> items, out T t, Vector3 position, int count) where T : Component => items.TryGetClosest(out t, null, position, count);
  public static bool TryGetClosest<T>(this IList<T> items, out T t, T ignore, Vector3 position) where T : Component => items.TryGetClosest(out t, ignore, position, items.Count - 1);
  public static bool TryGetClosest<T>(this IList<T> items, out T t, T ignore, Vector3 position, int count) where T : Component
  {
    if(items.IsNullOrEmpty() || count >= items.Count || count == 0)
    {
      t = null;
      return false;
    }

    T closest = null;
    float minDist = Mathf.Infinity;
    int i = 0;
    foreach(T item in items)
    {
      if(i >= count)
        break;

      if(item == null)
        continue;
      if(ignore != null && ignore == item)
        continue;

      float distance = (item.transform.position - position).sqrMagnitude;
      if(distance < minDist)
      {
        closest = item;
        minDist = distance;
      }

      i++;
    }

    t = closest;
    return closest != null;
  }
  public static bool TryGetClosest<T, U>(this IList<T> items, out T t, Vector3 position, int count) where T : Component where U : Component
  {
    if(items.IsNullOrEmpty() || count >= items.Count || count == 0)
    {
      t = null;
      return false;
    }

    T closest = null;
    float minDist = Mathf.Infinity;
    int i = 0;
    foreach(T item in items)
    {
      if(i >= count)
        break;

      if(item == null)
        continue;
      if(item.TryGetComponent(out U u))
        continue;

      float distance = (item.transform.position - position).sqrMagnitude;
      if(distance < minDist)
      {
        closest = item;
        minDist = distance;
      }

      i++;
    }

    t = closest;
    return closest != null;
  }
  // Random
  //----------------------------------------------------------------------------------------------------
  [MethodImpl(256)]
  public static int NextRandomIndex<T>(this IList<T> list)
  {
    if(list.IsNullOrEmpty())
      throw ThrowNullRef(nameof(list));

    return RNG.Int(list.Count);
  }
  [MethodImpl(256)]
  public static T NextRandom<T>(this IList<T> list)
  {
    if(list.IsNullOrEmpty())
      throw ThrowNullRef(nameof(list));

    int index = RNG.Int(list.Count);
    return list[index];
  }

  // Dictionary
  //----------------------------------------------------------------------------------------------------
  /// <summary>
  /// Gets a random key from the dictionary.
  /// </summary>
  /// <returns>A random key from the dictionary.</returns>
  [MethodImpl(256)]
  public static TKey RandomKey<TKey, TValue>(this Dictionary<TKey, TValue> dict)
  {
    if(dict.IsNullOrEmpty())
      throw ThrowNullRef(nameof(dict));

    List<TKey> keys = new List<TKey>(dict.Keys);
    int randomIndex = RNG.Int(keys.Count);
    return keys[randomIndex];
  }

  /// <summary>
  /// Gets a random value from the dictionary.
  /// </summary>
  /// <returns>A random value from the dictionary.</returns>
  [MethodImpl(256)]
  public static TValue RandomValue<TKey, TValue>(this Dictionary<TKey, TValue> dict)
  {
    if(dict.IsNullOrEmpty())
      throw ThrowNullRef(nameof(dict));

    List<TValue> values = new List<TValue>(dict.Values);
    int randomIndex = RNG.Int(values.Count);
    return values[randomIndex];
  }
}
}
