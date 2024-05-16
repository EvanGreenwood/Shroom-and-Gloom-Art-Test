using UnityEditor;
using UnityEngine;

namespace MathBad_Editor
{
public class ExtendedEditor<T> : Editor where T : Object
{
    T _targetInternal;
    T[] _targetsInternal;

    protected new T target
    {
        get
        {
            if(_targetInternal == null)
                _targetInternal = base.target as T;
            return _targetInternal;
        }
    }
    protected new T[] targets
    {
        get
        {
            if(_targetsInternal == null)
            {
                _targetsInternal = new T[base.targets.Length];
                for(int i = 0; i < base.targets.Length; i++)
                {
                    _targetsInternal[i] = base.targets[i] as T;
                }
            }
            return _targetsInternal;
        }
    }
}
}
