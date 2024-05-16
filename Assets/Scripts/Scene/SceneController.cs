#region Usings
using UnityEngine;
using Framework;
using MathBad;
using System;
using UnityEngine.SceneManagement;
#endregion

[RequireComponent(typeof(SceneData))]
public class SceneController : MonoService
{
    [SerializeField] float _sceneIntroLeadDelay = 1f;
    [SerializeField] float _sceneIntroFadeTime = 2f;
    [SerializeField] Music _music;

    public SceneData Data { get; private set; }

    private Service<Player> _player;
    private Service<GameManager> _gameManager;

    void Awake()
    {
        Data = GetComponent<SceneData>();
    }

    private void Start()
    {
        if (!_gameManager.Exists)
        {
            //No game manager to sequence things. Presuming player is in scene.
            BeginIntro();
        }
    }

    public void BeginIntro()
    {
        Debug.Assert(_player.Exists);
        
        HUD.inst.Init();
        Data.sceneIntro.Play();
        _music.Init(Data.sceneMusic);
        _player.Value.Init(Data);
        
        SceneTransition.inst.Transition(() =>
            {
                _player.Value.Activate();
                _music.Play();
            },
            _sceneIntroLeadDelay, false, false, _sceneIntroFadeTime,
            Data.title, Data.description,
            Data.titleColor);
    }
}
