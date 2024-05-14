#region Usings
using UnityEngine;
using MathBad;
using UnityEngine.Rendering.PostProcessing;
#endregion

public class Player : MonoSingleton<Player>
{
    [SerializeField] PlayerMover _movement;
    
    TunnelGenerator _tunnel;

    bool _hasInit, _isActivated;
    bool _cursorConfined;
    float _fwdInput;
    bool _runInput;

    public TunnelGenerator tunnel => _tunnel;

    private Service<WorldManagerService> _world;

    public void Init(SceneData sceneData)
    {
        PlayerView.inst.Init(sceneData.postProcessProfile);
        _hasInit = true;
    }

    public void Activate()
    {
        PlayerView.inst.Activate();
        _movement.Activate();
        _isActivated = true;
    }

    void Start()
    {
        _world.Value.TryGetTunnel(transform.position, out _tunnel);
        _movement.SetTunnel(_tunnel);
    }

    void Update()
    {
        if(!_hasInit || !_isActivated) {return;}

        ReadInput();
        CheckTunnel();
    }

    void ReadInput()
    {
        if(INPUT.tab.down)
        {
            _cursorConfined = !_cursorConfined;
            Cursor.lockState = _cursorConfined ? CursorLockMode.Confined : CursorLockMode.None;
        }

        _fwdInput = INPUT.moveInput2.y;
        _runInput = INPUT.leftShift.pressed;
        _movement.SetInput(_fwdInput, _runInput);
    }

    // Tunnel
    //----------------------------------------------------------------------------------------------------
    void CheckTunnel()
    {
        if(_fwdInput > 0)
        {
            if(_tunnel.GetNormDistanceFromPoint(transform.position) > 0.99f)
            {
                if (_world.Value.TryGetTunnel(transform.position, out _tunnel))
                {
                    _movement.SetTunnel(_tunnel);
                }
            }
        }
        else if(_fwdInput < 0)
        {
            //RN, can not go back to a previous tunnel. Does not work, closest tunnel is still current.
            //Failure is graceful, just cant move backwards anymore
            //We can add a stack of previous tunnels, but we should decide if thats something we want to allow,
            //eg, what happens if you go back to a door choice..?
            
            /*if (_tunnel.GetNormDistanceFromPoint(transform.position) < 0.1f)
            {
                FindNewTunnel();
            }*/
        }
    }

    public void SwitchTunnel(TunnelGenerator newTunnel)
    {
        _tunnel = newTunnel;
        _movement.SetTunnel(_tunnel);
    }

    public void SetScenePostProcessProfile(PostProcessProfile ppv)
    {
        PlayerView.inst.depthVolume.profile = ppv;
    }
}
