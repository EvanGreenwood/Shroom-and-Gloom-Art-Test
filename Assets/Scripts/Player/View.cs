#region Usings
using Framework;
using UnityEngine;
using MathBad;
using UnityEngine.Rendering.PostProcessing;
#endregion

public class View : MonoBehaviour
{
    [SerializeField] Camera _mainCamera;

    PostProcessVolume _volume;
    PostProcessProfile _ppv;
    bool _hasInit;
    bool _isActivated;

    public Camera mainCamera => _mainCamera;

    // Init
    //----------------------------------------------------------------------------------------------------
    public void Init(PostProcessProfile ppv)
    {
        _volume = _mainCamera.GetComponentInChildren<PostProcessVolume>();
        _volume.profile = ppv;
        _hasInit = true;
    }
    public void Activate()
    {
        _isActivated = true;
    }
    void Update()
    {
        if(!_hasInit || !_isActivated)
            return;
    }
}
