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
    [SerializeField] Camera _loadingCamera;
    [SerializeField] Canvas _loadingCanvas;

    public SceneData Data { get; private set; }

    private Service<Player> _player;
    public delegate void SceneHandler();
    public event SceneHandler onSceneLoaded;
    protected virtual void OnSceneLoaded() {onSceneLoaded?.Invoke();}

    private void Awake()
    {
        Data = GetComponent<SceneData>();
    }

    private void Start()
    {
        if(_worldManager.Exists && !_worldManager.Value.SingleTunnelTestMode)
        {
            _worldManager.Value.Generate(() =>
            {
                Instantiate(PlayerPrefab, Vector3.zero, Quaternion.identity);
                BeginIntro();
                OnSceneLoaded();
            });
        }
        else
        {
            Hero hero = FindObjectOfType<Hero>();
            if(!hero)
            {
                Instantiate(PlayerPrefab, Vector3.zero, Quaternion.identity);
            }

            BeginIntro();
        }
    }

    public void BeginIntro()
    {
        _loadingCamera.gameObject.SetActive(false);
        _loadingCanvas.gameObject.SetActive(false);

        Debug.Assert(_player.Exists);

        Data.sceneIntro.Play();
        _music.Init(Data.sceneMusic);

        if (SceneTransition.inst != null && SceneTransition.inst.gameObject.activeSelf)
        {
            SceneTransition.inst.Transition(() =>
                                            {
                                                _player.Value.CanMove = true;
                                                _music.Play();
                                            },
                                            _sceneIntroLeadDelay, false, false, _sceneIntroFadeTime,
                                            Data.title, Data.description,
                                            Data.titleColor);
        }
        else
        {
            _player.Value.CanMove = true;
            _music.Play();
        }
    }
}
