#region Usings
using UnityEngine;
using MathBad;
#endregion

public class Player : MonoSingleton<Player>
{
    [SerializeField] View _view;
    [SerializeField] FPSMovement _movement;
    bool _hasInit;
    bool _isActivated;
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
    }
}
