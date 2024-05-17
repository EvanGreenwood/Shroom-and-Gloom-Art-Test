#region Usings
using Framework;
using UnityEngine;
using MathBad;
using UnityEngine.Rendering.PostProcessing;
#endregion

public class PlayerView : MonoBehaviour
{
    [Header("Cameras")]
    [SerializeField] Camera _mainCamera;
    [SerializeField] Camera _uiCamera;

    [Header("Volumes")]
    [SerializeField] PostProcessVolume _depthVolume;

    [Header("Clamp")]
    [SerializeField] Vector2 _angleOffset;
    [SerializeField] float _maxXAngle = 35;
    [SerializeField] float _maxYAngle = 35;

    TunnelGenerator _curTunnel;

    DepthOfField _dof;

    Transform _viewTarget;
    bool _canLook = true;

    public Camera MainCamera => _mainCamera;
    public Camera UICamera => _uiCamera;

    private Service<Player> _player;

    // Init
    //----------------------------------------------------------------------------------------------------

    void Awake()
    {
        _viewTarget = transform.CreateChild("ViewTarget");
        _dof = _depthVolume.profile.GetSetting<DepthOfField>();
        _dof.focusDistance.Override(10f);
    }
    void Update()
    {
        // For editing post effects / materials,
        if(INPUT.leftShift.pressed && INPUT.tab.down)
        {
            _canLook = !_canLook;
        }

        if(_canLook) Look();
        AutoFocus(UnityEngine.Time.deltaTime);
    }

    void OnDrawGizmos()
    {
        if(_viewTarget == null)
        {
            return;
        }
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
        if(!_dof.enabled || !_player.Exists)
            return;

        _dof.focusDistance.value = (_viewTarget.position - transform.position).magnitude;
        StepFocusPos(_player.Value.Tunnel.Mesh);

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
