using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public struct Service<T> where T: MonoService
{
    private T _cachedService;
    public bool Exists => ServiceLocator.Has<T>();

    public T Value
    {
        get
        {
            if (_cachedService == null)
            {
                _cachedService = ServiceLocator.Get<T>();
            }

            return _cachedService;
        }
    }

   // public T V => Value;
}
