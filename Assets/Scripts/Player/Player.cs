#region Usings
using UnityEngine;
using MathBad;
using System.Collections.Generic;
using UnityEngine.Rendering.PostProcessing;
#endregion

public class Player : MonoService
{
    [SerializeField] PlayerMover _movement;

    private TunnelGenerator _tunnel => _tunnelStack.Count == 0 ? null : _tunnelStack[^1];
    
    bool _cursorConfined;
    float _fwdInput;
    bool _runInput;

    private List<TunnelGenerator> _tunnelStack = new List<TunnelGenerator>();

    public TunnelGenerator tunnel => _tunnel;

    public bool CanMove
    {
        get;
        set;
    }

    private Service<WorldManagerService> _world;

    void Start()
    {
        TunnelGenerator current = _tunnel;
        if (_world.Value.TryGetTunnel(transform.position, out current, _tunnelStack.ToArray()))
        {
            _tunnelStack.Add(current);
            _movement.SetTunnel(_tunnel);
        }
    }

    void Update()
    {
        if (!CanMove)
        {
            return;
        }
        
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
                TunnelGenerator current = _tunnel;
                if (_world.Value.TryGetTunnel(transform.position, out current, _tunnelStack.ToArray()))
                {
                    _tunnelStack.Add(current);
                    _movement.SetTunnel(current);
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
        _tunnelStack.Add(newTunnel);
        _movement.SetTunnel(_tunnel);
    }
}
