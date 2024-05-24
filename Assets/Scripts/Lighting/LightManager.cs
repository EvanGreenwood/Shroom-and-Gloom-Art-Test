using System.Collections;
using System.Collections.Generic;
using MathBad;
using UnityEngine;

public class LightManager : MonoService
{
    [SerializeField] GameObject _d1Ruins;
    [SerializeField] GameObject _d2Mines;
    [SerializeField] GameObject _d3Rock;
    [SerializeField] GameObject _d4Caves;
    [SerializeField] GameObject _d5AShroomy;
    [SerializeField] GameObject _d5BShroomy;
    [SerializeField] GameObject _d6ATrippy;
    [SerializeField] GameObject _d6BTrippy;
    [SerializeField] GameObject _d7AGloom;
    [SerializeField] GameObject _d7BGloom;

    void Awake()
    {
        Debug.Log($"Light Manager: Waiting".Bold().Yellow());
    }

    void Start()
    {
        if(ServiceLocator.Has<SceneManager>())
            ServiceLocator.Get<SceneManager>().onSceneLoaded += OnSceneLoaded;
    }
    void OnSceneLoaded()
    {
        Debug.Log($"Light Manager: Scene Loaded".Bold().Yellow());
        if(ServiceLocator.Has<WorldManagerService>())
        {
            for(int i = 0; i < ServiceLocator.Get<WorldManagerService>().tunnels.Count; i++)
            {
                TunnelGenerator tunnel = ServiceLocator.Get<WorldManagerService>().tunnels[i];

                if(tunnel.GenerationSettings == TunnelSettings.D1Ruins) { _d1Ruins.transform.SetParent(tunnel.transform); }
                else if(tunnel.GenerationSettings == TunnelSettings.D2Mines) { _d2Mines.transform.SetParent(tunnel.transform); }
                else if(tunnel.GenerationSettings == TunnelSettings.D3Rock) { _d3Rock.transform.SetParent(tunnel.transform); }
                else if(tunnel.GenerationSettings == TunnelSettings.D4Caves) { _d4Caves.transform.SetParent(tunnel.transform); }
                else if(tunnel.GenerationSettings == TunnelSettings.D5AShroomy) { _d5AShroomy.transform.SetParent(tunnel.transform); }
                else if(tunnel.GenerationSettings == TunnelSettings.D5BShroomy) { _d5BShroomy.transform.SetParent(tunnel.transform); }
                else if(tunnel.GenerationSettings == TunnelSettings.D6Trippy)
                {
                    if(tunnel.name.Contains("B1")) _d6ATrippy.transform.SetParent(tunnel.transform);
                    else if(tunnel.name.Contains("B2")) _d6BTrippy.transform.SetParent(tunnel.transform);
                }
                else if(tunnel.GenerationSettings == TunnelSettings.D7Gloom)
                {
                    if(tunnel.name.Contains("B1")) _d7AGloom.transform.SetParent(tunnel.transform);
                    else if(tunnel.name.Contains("B2")) _d7BGloom.transform.SetParent(tunnel.transform);
                }
            }
        }
        if(ServiceLocator.Has<SceneManager>())
            ServiceLocator.Get<SceneManager>().onSceneLoaded -= OnSceneLoaded;
    }
}
