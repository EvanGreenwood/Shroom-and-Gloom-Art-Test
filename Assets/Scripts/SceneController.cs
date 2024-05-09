#region Usings
using UnityEngine;
using Framework;
using MathBad;
using UnityEngine.SceneManagement;
#endregion

public class SceneController : SingletonBehaviour<SceneController>
{
    [SerializeField] SceneData _sceneData;
    string _activeSceneName;

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
        _sceneData.sceneIntro.Play();
        Player.inst.Init(_sceneData);
    }
    void OnTransitionComplete()
    {
        Player.inst.Activate();
    }
}
