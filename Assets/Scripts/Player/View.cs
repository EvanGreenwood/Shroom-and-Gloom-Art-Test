#region Usings
using Framework;
using UnityEngine;
using MathBad;
using UnityEngine.Rendering.PostProcessing;
#endregion

public class View : MonoSingleton<View>
{
    [Header("Cameras")]
    [SerializeField] Camera _mainCamera;
    [SerializeField] Camera _uiCamera;

    [Header("Volumes")]
    [SerializeField] PostProcessVolume _depthVolume;
    [SerializeField] PostProcessVolume _sceneVolume;

    [Header("Clamp")]
    [SerializeField] Vector2 _angleOffset;
    [SerializeField] float _maxXAngle = 35;
    [SerializeField] float _maxYAngle = 35;

    [Header("DepthOfField")]
    [SerializeField] float _autoFocusSmoothTime = 0.075f;

    TunnelGenerator _curTunnel;

    DepthOfField _dof;

    Timer2 _autoFocusTimer = new Timer2(0.15f, 0.025f);
    float _curFocalDst, _lastFocusDepthSuccess;
    float _autoFocusVelocity;

    bool _hasInit, _isActivated;

    public Camera mainCamera => _mainCamera;
    public Camera uiCamera => _uiCamera;

    public PostProcessVolume depthVolume => _depthVolume;
    public PostProcessVolume sceneVolume => _sceneVolume;

    // Init
    //----------------------------------------------------------------------------------------------------
    public void Init(PostProcessProfile uiProfile)
    {
        _sceneVolume.profile = uiProfile;

        _dof = _depthVolume.profile.GetSetting<DepthOfField>();
        _lastFocusDepthSuccess = 10f;
        _dof.focusDistance.Override(_lastFocusDepthSuccess);

        _hasInit = true;
    }

    public void Activate() {_isActivated = true;}

    void Awake() {_lastFocusDepthSuccess = 10f;}
    void Update()
    {
        if(!_hasInit || !_isActivated)
            return;

        Look();
        AutoFocus(Time.deltaTime);
    }

    void Look()
    {
        float halfWidth = SCREEN.size.x * 0.5f, halfHeight = SCREEN.size.y * 0.5f;
        float pitch = (INPUT.mousePos.y - halfHeight) / -halfHeight * _maxXAngle;
        float yaw = (INPUT.mousePos.x - halfWidth) / halfWidth * _maxYAngle;

        transform.localEulerAngles = new Vector3(pitch, yaw, 0);
    }

    // Auto Focus
    //----------------------------------------------------------------------------------------------------
    void AutoFocus(float dt)
    {
        if(!_dof.enabled)
            return;

        _curFocalDst = Mathf.SmoothDamp(_curFocalDst, _lastFocusDepthSuccess, ref _autoFocusVelocity, _autoFocusSmoothTime, 25f, dt);
        _dof.focusDistance.Override(_curFocalDst);

        _autoFocusTimer.Step(dt);
        if(_autoFocusTimer.wasFinishedThisFrame)
        {
            _autoFocusTimer.Reset();
            StepFocusPos();
        }

        return;
        //--------------------------------------------------
        void StepFocusPos()
        {
            Vector3 mousePos = INPUT.mousePos;
            Ray ray = _mainCamera.GetMouseRay();

            TunnelMesh mesh = Player.inst.tunnel.tunnelMesh;
            if(mesh != null && mesh.Raycast(ray, out Vector3 hitPos, out Vector3 hitNormal))
                _lastFocusDepthSuccess = (hitPos - transform.position).magnitude;
        }
    }
}
