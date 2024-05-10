#region Usings
using UnityEngine;
using Framework;
using MathBad;
using UnityEngine.SceneManagement;
#endregion

public class SceneController : MonoSingleton<SceneController>
{
    [SerializeField] float _sceneIntroLeadDelay = 1f;
    [SerializeField] float _sceneIntroFadeTime = 2f;
    [SerializeField] Music _music;

    SceneData _sceneData;
    string _activeSceneName;
    
    public SceneData sceneData => _sceneData;

    void Awake()
    {
        _sceneData = FindObjectOfType<SceneData>();
        _activeSceneName = SceneManager.GetActiveScene().name;

        SceneTransition.inst.Transition(OnTransitionComplete,
                                        _sceneIntroLeadDelay, false, false, _sceneIntroFadeTime,
                                        _sceneData.title, _sceneData.description,
                                        _sceneData.titleColor);
    }

    void Start()
    {
        HUD.inst.Init();
        _sceneData.sceneIntro.Play();
        _music.Init(_sceneData.sceneMusic);
        Player.inst.Init(_sceneData);
    }

    void OnTransitionComplete()
    {
        Player.inst.Activate();
        _music.Play();
    }
}
