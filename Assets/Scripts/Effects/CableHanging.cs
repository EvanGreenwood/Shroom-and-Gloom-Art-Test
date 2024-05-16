#region Usings
using UnityEngine;
using Framework;
using MathBad;
#endregion

[RequireComponent(typeof(LineRenderer))]
public class CableHanging : MonoBehaviour
{
  [SerializeField] float _density = 4f;
  [SerializeField] Vector2 _direction = Vector3.down;
  [SerializeField] FloatRange _length = new FloatRange(1f, 1f);
  [SerializeField] float _stiffness = 50f;
  [SerializeField] float _drag = 5f;

  LineRenderer _lr;
  int _numNodes;

  private Transform _cameraTransform => Camera.main?Camera.main.transform:null;

  Vector3[] _positions;
  Vector3[] _velocities;
  float _restLength;
  bool _isAwake;

  // MonoBehaviour
  //----------------------------------------------------------------------------------------------------
  void Awake()
  {
    float length = _length.ChooseRandom();
    _numNodes = (length * _density).FloorToInt().Max(2);
    _restLength = length / _numNodes;
    _lr = GetComponent<LineRenderer>();
    _lr.positionCount = _numNodes;

    _positions = new Vector3[_numNodes];
    _velocities = new Vector3[_numNodes];

    for(int i = 0; i < _numNodes; i++)
    {
      _positions[i] = transform.TransformPoint(Vector3.Lerp(Vector3.zero,
                                                            transform.TransformDirection(_direction) * length,
                                                            mathi.unlerp(i, _numNodes)));
      _lr.SetPosition(i, _positions[i]);
    }
    _isAwake = true;
  }
  void FixedUpdate() {Step(Time.fixedDeltaTime);}

  void AddForce(Vector3 pos, Vector3 force, float radius)
  {
    for(int i = 0; i < _numNodes; i++)
    {
      float dst = Vector3.Distance(pos, _positions[i]);
      float factor = 1f - Mathf.Clamp01(Mathf.InverseLerp(0f, radius, dst));
      _velocities[i] += force;
    }
  }

  // Step
  //----------------------------------------------------------------------------------------------------
  void Step(float dt)
  {
    // if(RandomUtils.Chance(0.001f))
    //   AddForce(RNG.Vector2InsideUnitCircle().ToVector3() + transform.position,
    //            RNG.Vector2InsideUnitCircle().ToVector3() * RNG.Float(1f, 3f), 1.5f);
    _positions[0] = transform.position;
    _lr.SetPosition(0, transform.position);
    
    Vector3 cameraPos = _cameraTransform?_cameraTransform.position:Vector3.zero;
    for(int i = 1; i < _numNodes; i++)
    {
      Vector3 spring_up = PHYSICS.LinearSpringForce(_positions[i], _positions[i - 1], _restLength, _stiffness);
      Vector3 spring_down = Vector3.zero;

      if(i < _numNodes - 1) spring_down = PHYSICS.LinearSpringForce(_positions[i], _positions[i + 1], _restLength, _stiffness);
      Vector3 total_spring = (spring_up + spring_down) * dt;

      _velocities[i] += total_spring + Physics.gravity * dt;
      Vector3 cameraHeading = _positions[i] - cameraPos;
      float cameraDst = cameraHeading.magnitude;
      if(cameraDst < 2f)
      {
        AddForce(cameraPos, cameraHeading * (10f * dt), 1f);
      }

      _positions[i] += _velocities[i] * dt;

      _velocities[i] = PHYSICS.ApplyLinearDragForce(_velocities[i], _drag, dt);

      _lr.SetPosition(i, _positions[i]);
    }
  }

  void OnDrawGizmos()
  {
    if(!_isAwake)
      return;

    for(int i = 0; i < _numNodes; i++)
    {
      GIZMOS.Sphere(_positions[i], 0.1f, RGB.HueLerp(i, _numNodes));
    }
  }
}
