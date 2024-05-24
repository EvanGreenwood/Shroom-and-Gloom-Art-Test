#region Usings
using UnityEngine;
using MathBad;
using System.Collections.Generic;
using UnityEngine.Rendering.PostProcessing;
using UnityEngine.Splines;

#endregion

public class Player : MonoService
{
    [SerializeField] PlayerMover _movement;

    bool _cursorConfined;
    float _fwdInput;
    bool _runInput;

    private List<TunnelGenerator> _tunnelStack = new List<TunnelGenerator>();

    public TunnelGenerator Tunnel => _tunnelStack.Count == 0 ? null : _tunnelStack[^1];

    public bool CanMove
    {
        get;
        set;
    }

    private Service<WorldManagerService> _world;
    private SplineContainer _overrideSpline;

    void Start()
    {
        TunnelGenerator current = Tunnel;
        if(_world.Value.TryGetTunnel(transform.position, out current, _tunnelStack.ToArray()))
        {
            _tunnelStack.Add(current);
            _movement.SetTunnel(current);
            _world.Value.CullForTunnel(current);
        }
    }

    void Update()
    {
        if(!CanMove)
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
            if (_movement.HasOverrideSpline())
            {
                if (_movement.AtEndOfOverrideSpline())
                {
                    _movement.SetOverrideSpline(null);
                }
            }
            else if(Tunnel.GetNormDistanceFromPoint(transform.position) > 0.99f)
            {
                TunnelGenerator current = Tunnel;
                if(_world.Value.TryGetTunnel(transform.position, out current, _tunnelStack.ToArray()))
                {
                    _tunnelStack.Add(current);
                    _movement.SetTunnel(current);
                    _world.Value.CullForTunnel(current);
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
        _movement.SetTunnel(Tunnel);
    }

    public void SetOverrideSpline(SplineContainer doorSpine)
    {
        _overrideSpline = doorSpine;
        _movement.SetOverrideSpline(_overrideSpline);
    }
}
