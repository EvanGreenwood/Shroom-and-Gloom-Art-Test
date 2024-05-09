#region Usings
using Framework;
using MathBad;
using UnityEngine;
using UnityEngine.Rendering.PostProcessing;
#endregion

public class CameraAutoFocus : MonoBehaviour
{
    [SerializeField] PostProcessVolume _volume;
    [SerializeField] float _smoothTime = 0.15f;
    
    PostProcessProfile _ppv;
    DepthOfField _dof;
    float _curFocalDst, _lastDepthSuccess;
    float _velocity;
    void Awake()
    {
        _ppv = _volume.profile;
        _dof = _ppv.GetSetting<DepthOfField>();

        _lastDepthSuccess = 10f;
        _dof.focusDistance.Override(_lastDepthSuccess);
    }

    void Update()
    {
        AutoFocus();
    }

    void AutoFocus()
    {
        Vector3 mousePos = INPUT.mousePos;
        Ray ray = SceneUtils.MainCamera.GetMouseRay();
        if(Physics.Raycast(ray.origin, ray.direction, out RaycastHit hit, 100f))
        {
            float targetDst = (hit.point - transform.position).magnitude;
            _lastDepthSuccess = targetDst;

            _curFocalDst = Mathf.SmoothDamp(_curFocalDst, targetDst, ref _velocity, _smoothTime, 15f, Time.deltaTime);
            _dof.focusDistance.Override(_curFocalDst);
        }
    }
}
