#region Usings
using Framework;
using UnityEngine;
using MathBad;
using UnityEngine.Rendering.PostProcessing;
#endregion

public class View : MonoBehaviour
{
    [SerializeField] Camera _mainCamera;
    [SerializeField] Camera _uiCamera;

    [Header("Clamp")]
    [SerializeField] Vector2 _angleOffset;
    [SerializeField] float _maxXAngle = 35;
    [SerializeField] float _maxYAngle = 35;

    PostProcessVolume _volume;
    PostProcessProfile _ppv;

    bool _hasInit;
    bool _isActivated;

    public Camera mainCamera => _mainCamera;
    public Camera uiCamera => _uiCamera;

    // Init
    //----------------------------------------------------------------------------------------------------
    public void Init(PostProcessProfile ppv)
    {
        _volume = _uiCamera.GetComponentInChildren<PostProcessVolume>();
        _volume.profile = ppv;
        _hasInit = true;
    }

    public void Activate() {_isActivated = true;}

    void Update()
    {
        if(!_hasInit || !_isActivated)
            return;
        
        float halfWidth = SCREEN.size.x * 0.5f, halfHeight = SCREEN.size.y * 0.5f;
        float pitch = (INPUT.mousePos.y - halfHeight) / -halfHeight * _maxXAngle;
        float yaw = (INPUT.mousePos.x - halfWidth) / halfWidth * _maxYAngle;

        transform.localEulerAngles = new Vector3(pitch, yaw, 0);
    }

    public void SetPostProcessingProfile(PostProcessProfile ppv)
    {
        _volume = _uiCamera.GetComponentInChildren<PostProcessVolume>();
        _volume.profile = ppv;
    }
}
