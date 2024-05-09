#region Usings
using Framework;
using UnityEngine;
using Mainframe;
#endregion

public class SpriteBillboard : MonoBehaviour
{
    [SerializeField] bool _cameraFade = true;
    SpriteRenderer _sr;
    float _startAlpha;
    void Awake()
    {
        _sr = GetComponent<SpriteRenderer>();
        _startAlpha = _sr.color.a;
    }
    void Update()
    {
        Vector3 dir = (SceneUtils.MainCamera.transform.position - transform.position).normalized;
        transform.rotation = Quaternion.LookRotation(-dir, Vector3.up);

        if(!_cameraFade)
            return;

        float dst = Vector3.Distance(transform.position, SceneUtils.MainCamera.transform.position);
        if(dst < 4f)
        {
            _sr.color = _sr.color.WithA(Mathf.Lerp(0f, _startAlpha, Mathf.InverseLerp(0f, 4f, dst)));
        }
    }
}
