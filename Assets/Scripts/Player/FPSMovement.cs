using Framework;
using MathBad;
using UnityEngine;

public class FPSMovement : MonoBehaviour
{
    [SerializeField] Transform _view;
    [SerializeField] TunnelGenerator _tunnel;

    [SerializeField] float _speed = 4;
    [SerializeField] float _headHeight = 1.6f;
    [SerializeField] float _directionLookaheadDistance = 1;

    [Header("Bob")]
    [SerializeField] float _bobHeight = 0.1f;
    [SerializeField] float _bobRate = 0.5f;

    float _bobCounter = 0;
    float _currentSpeed = 5;

    bool _isActivated;

    // Activate
    //----------------------------------------------------------------------------------------------------
    public void Activate()
    {
        INPUT.SetCursorPos(SCREEN.center);
        _isActivated = true;
    }

    public void SwitchTunnel(TunnelGenerator newTunnel)
    {
        _tunnel = newTunnel;
    }

    void Start()
    {
        _view.localPosition = new Vector3(0f, _headHeight, 0f);

        if(_tunnel == null)
        {
            _tunnel = TunnelSystem.inst.FindNewTunnel(transform.position);
        }

        _currentSpeed = _speed;
    }

    void Update()
    {
        if(!_isActivated)
            return;
        if(Input.GetKey(KeyCode.LeftShift))
        {
            _currentSpeed = Mathf.MoveTowards(_currentSpeed, _speed * 1.8f, Time.deltaTime * _speed * 0.6f);
        }
        else
        {
            _currentSpeed = Mathf.MoveTowards(_currentSpeed, _speed, Time.deltaTime * _speed);
        }

        float moveDir = Input.GetKey(KeyCode.W) ? 1f : Input.GetKey(KeyCode.S) ? -1f : 0;
        if(Mathf.Abs(moveDir) > 0)
        {
            if(_tunnel == null)
            {
                transform.Translate(transform.forward * (Time.deltaTime * _currentSpeed * moveDir));
            }
            else
            {
                if(moveDir > 0)
                {
                    if(_tunnel.GetNormDistanceFromPoint(transform.position) > 0.99f)
                    {
                        _tunnel = TunnelSystem.inst.FindNewTunnel(transform.position);
                    }
                }
                else if(moveDir < 0)
                {
                    //TODO: RN, can not go back to a previous tunnel. Does not work, closest tunnel is still current.
                    /*if (_tunnel.GetNormDistanceFromPoint(transform.position) < 0.1f)
                    {
                        FindNewTunnel();
                    }*/
                }

                float t = _tunnel.GetClosestPositionAndDirection(transform.position, out Vector3 currentPosition, out Vector3 currentDirection, out Vector3 currentUp);
                Vector3 lookaheadPoint = _tunnel.GetLookaheadPoint(t, _directionLookaheadDistance);
                Vector3 towardsLookahead = (lookaheadPoint - currentPosition).normalized;
                transform.forward = Vector3.RotateTowards(transform.forward, towardsLookahead, Time.deltaTime * 0.25f, 0);
                currentDirection = moveDir * currentDirection;
                Debug.DrawLine(currentPosition, currentPosition + currentDirection);
                transform.position = Vector3.MoveTowards(transform.position, currentPosition + currentDirection, Time.deltaTime * _currentSpeed);
            }

            //Moving bob
            _bobCounter += Time.deltaTime * _speed * moveDir;
            _view.transform.localPosition = _view.transform.localPosition.WithY(Mathf.Lerp(_view.transform.localPosition.y, _headHeight + Mathf.Sin(_bobCounter / _bobRate * Mathf.PI) * _bobHeight, Time.deltaTime * 12));
        }
        else
        {
            //Reduce bob
            _bobCounter = Mathf.Lerp(_bobCounter, Mathf.Round(_bobCounter * _bobRate) / _bobRate, Time.deltaTime * 7);
            _view.transform.localPosition = _view.transform.localPosition.WithY(Mathf.Lerp(_view.transform.localPosition.y, _headHeight + Mathf.Sin(_bobCounter / _bobRate * Mathf.PI) * _bobHeight, Time.deltaTime * 8));
        }
    }
}
