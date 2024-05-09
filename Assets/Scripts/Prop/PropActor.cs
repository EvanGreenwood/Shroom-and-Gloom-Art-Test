#region Usings
using Framework;
using UnityEngine;
using MathBad;
#endregion

public class PropActor : MonoBehaviour
{
    [SerializeField] float _disturbDst = 10f;
    [SerializeField] Emote _disturbedEmotePrefab;

    [SerializeField] float _fleeDst = 5f;
    [SerializeField] FloatRange _fleeSpeed = new FloatRange(2f, 5f);
    [Header("Audio")]
    [SerializeField] EffectSoundBank _onDisturbed;
    [SerializeField] EffectSoundBank _onFlee;
    [SerializeField] bool _randomFleeDir = true;

    FloatAnim _fleeAnim ;
    [SerializeField] float _feelAngleTime = 0.25f;
    [SerializeField]FloatRange _moveAngleRange = new FloatRange(-25f, 25f);

    SpriteRenderer _sr;
    Camera _camera;

    bool _isFleeing;

    float _fleeDir, _speed;

    // MonoBehaviour
    //----------------------------------------------------------------------------------------------------
    void Awake()
    {
        _sr = GetComponent<SpriteRenderer>();
        _camera = Camera.main;
        _fleeAnim = new FloatAnim(EaseType.InQuad, LoopType.PingPong, _feelAngleTime);
        _fleeDir = (_randomFleeDir ? RNG.CoinFlip() : !_sr.flipX) ? 1f : -1f;
        _speed = _fleeSpeed.ChooseRandom();
    }
    void Update()
    {
        if(_isFleeing)
        {
            Flee(Time.deltaTime);
            return;
        }

        float dst = Vector3.Distance(transform.position, _camera.transform.position);
        if(!_isFleeing && dst <= _disturbDst)
        {
            _isFleeing = true;
            float dir = _sr.flipX ? -1f : 1f;
            Emote emote = Instantiate(_disturbedEmotePrefab,
                                      transform.position + transform.right * (1.5f * dir) + Vector3.up * 0.5f,
                                      transform.rotation);

            if(_sr.flipX) emote.GetComponent<SpriteRenderer>().flipX = true;
            _onDisturbed.Play(transform.position);
            _onFlee.Play(transform);
            
            GetComponent<SpriteAnimator>().Play();
        }
    }

    void Flee(float dt)
    {
        _fleeAnim.Step(dt);
        transform.rotation = Quaternion.Euler(0f, 0f, Mathf.Lerp(_moveAngleRange.Min, _moveAngleRange.Max, _fleeAnim.percent));
        Vector3 dir = transform.TransformDirection(Vector3.right * _fleeDir);
        transform.position += dir * (_speed * dt);
    }
}