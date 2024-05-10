#region Usings
using UnityEngine;
using Framework;
using MathBad;
using UnityEngine.SceneManagement;
#endregion

public class SceneController : MonoSingleton<SceneController>
{
    SceneData _sceneData;
    [SerializeField] Music _music;
    string _activeSceneName;
    AudioSource _musicSource;
    public SceneData sceneData => _sceneData;

    void Awake()
    {
        _sceneData = FindObjectOfType<SceneData>();
        _activeSceneName = SceneManager.GetActiveScene().name;
        SceneTransition.inst.Transition(OnTransitionComplete,
                                        1.5f, false, false, 1f,
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
