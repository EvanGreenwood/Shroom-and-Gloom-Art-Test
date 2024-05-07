#region Usings
using UnityEngine;
using Framework;
using Mainframe;
using Unity.Jobs;
#endregion

[RequireComponent(typeof(LineRenderer))]
public class LineRope : MonoBehaviour
{
  [SerializeField] float _density = 4f;

  [SerializeField] float _length = 1f;
  [SerializeField] float _width = 0.2f;

  LineRenderer _lr;
  int _numNodes;

  Vector3[] _positions;
  Vector3[] _velocities;

  // MonoBehaviour
  //----------------------------------------------------------------------------------------------------
  void Awake()
  {
    _numNodes = (_length * _density).FloorToInt();
    _positions = new Vector3[_numNodes];
    _velocities = new Vector3[_numNodes];
    _lr = GetComponent<LineRenderer>();
  }
  void Update() {Step(Time.deltaTime);}

  // Step
  //----------------------------------------------------------------------------------------------------
  void Step(float dt)
  {
    for(int i = 0; i < _numNodes; i++)
    {
      
    }
  }
}
