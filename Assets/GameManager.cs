using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoService
{
    public GameObject PlayerPrefab;

    private Service<WorldManagerService> _worldManager;
    private Service<SceneController> _sceneController; //TODO: move into game manager directly.
    
    
    public void Start()
    {
        _worldManager.Value.Generate(() =>
        {
            Instantiate(PlayerPrefab, Vector3.zero, Quaternion.identity);
            _sceneController.Value.BeginIntro();
        });
        
    }
}
