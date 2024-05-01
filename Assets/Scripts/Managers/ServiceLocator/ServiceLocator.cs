using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ServiceLocator : MonoBehaviour
{
    private static List<MonoService> _services = new List<MonoService>();

    public static T Get<T>() where T: MonoService
    {
        foreach (MonoService service in _services)
        {
            if (service.GetType() == typeof(T))
            {
                return (T)service;
            }
        }
        Debug.LogError($"[ServiceLocator] Get for type {typeof(T)} failed. No registered service. Call Has<T>() before getting.");

        return null;
    }
    
    public static bool Has<T>() where T: MonoService
    {
        foreach (MonoService service in _services)
        {
            if (service.GetType() == typeof(T))
            {
                return true;
            }
        }

        return false;
    }

    public static void Register(MonoService service)
    {
        foreach (MonoService existing in _services)
        {
            if (existing.GetType() == service.GetType())
            {
                Debug.LogError($"[ServiceLocator] Registering pre-existing service. {service.GetType()}." +
                    $"Not registering duplicate {service.gameObject.name}");
                return;
            }
        }
        _services.Add(service);
    }

    public static void UnRegister(MonoService service)
    {
        _services.Remove(service);
    }
}
