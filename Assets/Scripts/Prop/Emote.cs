#region Usings
using UnityEngine;
using Mainframe;
#endregion

public class Emote : MonoBehaviour
{
    SpriteRenderer _sr;
    FloatAnim _anim = new FloatAnim(EaseType.InOutQuad, LoopType.PingPong, 0.5f);
    Vector3 _startScale;
    int _numLoop = 0;
    // Init
    //----------------------------------------------------------------------------------------------------
    public void Init(Color color)
    {
        _sr = GetComponent<SpriteRenderer>();
        _sr.color = color;
    }
    void Awake()
    {
        _startScale = transform.localScale;
        transform.localScale = Vector3.zero;
        _anim.onPingPong += OnPingPong;
    }
    void OnPingPong()
    {
        _numLoop++;
        if(_numLoop == 1)
            Destroy(gameObject);
    }
    public void Update()
    {
        _anim.Step(Time.deltaTime);

        transform.localScale = _startScale * _anim.lerp;
        transform.position += Vector3.up * (0.25f * Time.deltaTime);
    }
}
