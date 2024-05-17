#region Usings
using UnityEngine;
using MathBad;
#endregion

public class SpriteBody : MonoBehaviour
{
    [Header("Upright")]
    [SerializeField] PID _uprightPID = new PID(1f, 0f, 0.1f);
    [SerializeField] float _drag = 1f;

    [Header("Walk")]
    [SerializeField] float _walkSpread = 35f;
    [SerializeField] float _walkSpeed = 2f;

    SpriteRenderer _sr;

    float _av;
    float _walkDst = 0.01f;
    Vector3 _lastPos;
    Timer2 _knockTimer2 = new Timer2(0.15f, 0.05f);

    public void Knock(float knockSpeed)
    {
        _av += knockSpeed * 360f;
        _knockTimer2.Reset();
    }

    // MonoBehaviour
    //----------------------------------------------------------------------------------------------------
    void Awake()
    {
        _sr = GetComponent<SpriteRenderer>();
        _lastPos = transform.position;
    }

    void FixedUpdate()
    {
        FixedStep(UnityEngine.Time.fixedDeltaTime);
        _lastPos = transform.position;
    }

    void FixedStep(float dt)
    {
        float dst = (transform.position - _lastPos).magnitude;
        _walkDst += dst;

        float walk_angle = 0f;
        if(_knockTimer2.hasFinished)
        {
            float walk_lerp = EASE.Evaluate(MATH.Sin01(_walkDst * _walkSpeed), EaseType.InOutBounce);
            walk_angle = MATH.Lerp(-_walkSpread.Half(), _walkSpread.Half(), walk_lerp);
        }
        else _knockTimer2.Step(dt);

        float cur_angle = transform.right._Vec2().ToAngle();
        float target_angle = walk_angle + Vector2.right.ToAngle();

        _av += _uprightPID.GetOutput(Mathf.DeltaAngle(cur_angle, target_angle), dt);
        _av = PHYSICS.ApplyLinearDragForce(_av, _drag, dt);

        float result = cur_angle + _av * dt;
        transform.rotation = Quaternion.Euler(0, 0, result);
    }
}
