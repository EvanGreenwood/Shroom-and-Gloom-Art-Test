using UnityEditor;
using UnityEngine;

namespace MathBad_Editor
{
public class EditorExtended<T> : Editor where T : Component
{
  protected T _target;
  protected T[] _targets;

  protected virtual void OnEnable()
  {
    _target = target as T;
    _targets = new T[targets.Length];
    for(int i = 0; i < targets.Length; i++)
    {
      _targets[i] = targets[i] as T;
    }
  }
}
}
