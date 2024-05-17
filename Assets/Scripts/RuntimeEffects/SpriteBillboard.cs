#region Usings
using Framework;
using UnityEngine;
using MathBad;
#endregion

public class SpriteBillboard : MonoBehaviour
{
    [SerializeField] bool _cameraFade = true;
    [SerializeField] bool _vertical = true;
    SpriteRenderer _sr;
    float _startAlpha;
    
    private Vector3 _cameraPos => Camera.main?Camera.main.transform.position:Vector3.zero; 
    
    void Awake()
    {
        _sr = GetComponent<SpriteRenderer>();
        _startAlpha = _sr.color.a;
    }
    void Update()
    {
        Vector3 dir = (_cameraPos - transform.position).normalized;
        if(_vertical) dir = dir.WithY(0f).normalized;
        
        transform.rotation = Quaternion.LookRotation(-dir, Vector3.up);

        if(!_cameraFade)
            return;

        float dst = Vector3.Distance(transform.position, _cameraPos);
        if(dst < 4f)
        {
            _sr.color = _sr.color.WithA(Mathf.Lerp(0f, _startAlpha, Mathf.InverseLerp(0f, 4f, dst)));
        }
    }
}
