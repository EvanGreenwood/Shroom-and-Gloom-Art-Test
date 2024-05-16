#region Usings
using UnityEngine;
#endregion

[RequireComponent(typeof(SceneData))]
public class SceneManager : MonoService
{
    public GameObject PlayerPrefab;

    private Service<WorldManagerService> _worldManager;
    private Service<SceneManager> _sceneController; //TODO: move into game manager directly.
    
    
    [SerializeField] float _sceneIntroLeadDelay = 1f;
    [SerializeField] float _sceneIntroFadeTime = 2f;
    [SerializeField] Music _music;

    public SceneData Data { get; private set; }

    private Service<Player> _player;

    void Awake()
    {
        Data = GetComponent<SceneData>();
    }

    private void Start()
    {
        if (_worldManager.Exists && !_worldManager.Value.SingleTunnelTestMode)
        {
            _worldManager.Value.Generate(() =>
            {
                Instantiate(PlayerPrefab, Vector3.zero, Quaternion.identity);
                BeginIntro();
            });
        }
        else
        {
            Instantiate(PlayerPrefab, Vector3.zero, Quaternion.identity);
            BeginIntro();
        }
    }

    public void BeginIntro()
    {
        Debug.Assert(_player.Exists);
        
        HUD.inst.Init();
        Data.sceneIntro.Play();
        _music.Init(Data.sceneMusic);
        
        SceneTransition.inst.Transition(() =>
            {
                _player.Value.CanMove = true;
                _music.Play();
            },
            _sceneIntroLeadDelay, false, false, _sceneIntroFadeTime,
            Data.title, Data.description,
            Data.titleColor);
    }
}
