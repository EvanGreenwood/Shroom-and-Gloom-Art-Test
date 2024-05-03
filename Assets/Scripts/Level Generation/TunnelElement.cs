#region Usings
using UnityEngine;
using Framework;
#endregion

public class TunnelElement : MonoBehaviour
{
    SpriteRenderer _sr;
    bool _hasInit;
    // Init
    //----------------------------------------------------------------------------------------------------
    public void Init()
    {
        _sr = GetComponent<SpriteRenderer>();
        _hasInit = true;
    }

    // MonoBehaviour
    //----------------------------------------------------------------------------------------------------
    void Awake()
    {
        if(!_hasInit) 
            Init();
    }
    
    void Update() { }
}
