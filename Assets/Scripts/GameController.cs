#region Usings
using UnityEngine;
using Framework;
using MathBad;
using UnityEngine.SceneManagement;
#endregion

public class GameController : SingletonBehaviour<GameController>
{
    string _activeSceneName;
    [SerializeField] SceneData _sceneData;

    protected override void Awake()
    {
        base.Awake();
        HUD.inst.Init();
        _activeSceneName = SceneManager.GetActiveScene().name;
        SceneTransition.inst.Transition(OnTransitionComplete,
                                        1.5f, false, false, 1f,
                                        _sceneData.title, _sceneData.description,
                                        RGB.grey, RGB.black);
    }
    void Start()
    {
        _sceneData.onSceneLoadedAudio.Play();
    }
    void OnTransitionComplete()
    {
        FreeCamera freeCamera = FindObjectOfType<FreeCamera>();
        if(freeCamera) freeCamera.Pause(false);
    }
}
