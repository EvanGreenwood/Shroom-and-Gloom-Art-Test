#region
using System;
using UnityEngine;
#endregion

namespace MathBad
{
public class ScriptableSingleton<T> : ScriptableObject where T : ScriptableSingleton<T>
{
  static object _lock = new object();

  static T _instance;
  public static T inst
  {
    get
    {
      lock(_lock)
      {
        if(_instance == null)
        {
          string resourcesPath = "Singleton";
          T[] assets = Resources.LoadAll<T>(resourcesPath);

          if(assets == null || assets.Length < 1)
            throw new Exception($"Could not find any data singleton of type {typeof(T)} in {resourcesPath}.");
          else if(assets.Length > 1)
            Debug.LogWarning($"{typeof(ScriptableSingleton<T>).ToString().Bold()} There are multiple instances found of {typeof(T)} .");

          _instance = assets[0];
        }

        return _instance;
      }
    }
  }
}
}
