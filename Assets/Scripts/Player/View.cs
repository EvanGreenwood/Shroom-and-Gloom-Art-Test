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

    TunnelGenerator _curTunnel;

    DepthOfField _dof;

    Transform _viewTarget;

    bool _hasInit, _isActivated;
    bool _isAwake;

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
        _dof.focusDistance.Override(10f);

        _hasInit = true;
    }

    public void Activate() {_isActivated = true;}

    void Awake()
    {
        _viewTarget = transform.CreateChild("ViewTarget");
        _isAwake = true;
    }
    void Update()
    {
        if(!_hasInit || !_isActivated)
            return;

        Look();
        AutoFocus(Time.deltaTime);
    }

    void OnDrawGizmos()
    {
        if(!_isAwake)
            return;
        GIZMOS.Sphere(_viewTarget.position, 0.25f, RGB.yellow);
        GIZMOS.Line(transform.position, _viewTarget.position, RGB.cyan);
    }

    void Look()
    {
        Vector2 mousePos = INPUT.mousePos;
        if(!SCREEN.rect.Contains(mousePos))
            return;
        
        float halfWidth = SCREEN.size.x * 0.5f, halfHeight = SCREEN.size.y * 0.5f;
        float pitch = (mousePos.y - halfHeight) / -halfHeight * _maxXAngle;
        float yaw = (mousePos.x - halfWidth) / halfWidth * _maxYAngle;

        transform.localEulerAngles = new Vector3(pitch, yaw, 0);
    }

    // Auto Focus
    //----------------------------------------------------------------------------------------------------
    void AutoFocus(float dt)
    {
        if(!_dof.enabled)
            return;

        _dof.focusDistance.value = (_viewTarget.position - transform.position).magnitude;
        StepFocusPos(Player.inst.tunnel.tunnelMesh);

        return;
        //--------------------------------------------------
        void StepFocusPos(TunnelMesh mesh)
        {
            Ray ray = _mainCamera.ScreenPointToRay(INPUT.mousePos);

            if(mesh != null && mesh.Raycast(new Ray(transform.position, ray.direction), out Vector3 hitPos, out Vector3 hitNormal))
            {
                _viewTarget.position = hitPos;
                _viewTarget.rotation = Quaternion.LookRotation(hitNormal, Vector3.up);
            }
        }
    }
}
