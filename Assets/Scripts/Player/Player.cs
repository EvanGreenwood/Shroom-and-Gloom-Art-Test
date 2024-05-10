#region Usings
using UnityEngine;
using MathBad;
using UnityEngine.Rendering.PostProcessing;
#endregion

public class Player : MonoSingleton<Player>
{
    [SerializeField] View _view;
    [SerializeField] FPSMovement _movement;
    bool _hasInit, _isActivated;
    bool _cursorConfined;
    public View view => _view;

    // Init
    //----------------------------------------------------------------------------------------------------
    public void Init(SceneData sceneData)
    {
        _view.Init(sceneData.postProcessProfile);
        _hasInit = true;
    }
    public void Activate()
    {
        _view.Activate();
        _movement.Activate();
        _isActivated = true;
    }
    void Update()
    {
        if(!_hasInit || !_isActivated)
            return;

        if(INPUT.tab.down)
        {
            _cursorConfined = !_cursorConfined;
            Cursor.lockState = _cursorConfined ? CursorLockMode.Confined : CursorLockMode.None;
        }
    }

    public void SetPostProcessingProfile(PostProcessProfile ppv)
    {
        _view.SetPostProcessingProfile(ppv);
    }
}
