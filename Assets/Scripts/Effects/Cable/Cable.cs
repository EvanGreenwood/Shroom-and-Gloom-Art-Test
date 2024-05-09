#region
using Framework;
using MathBad;
using UnityEngine;
#endregion

public class Cable : MonoBehaviour
{
  [SerializeField] Transform _anchor0, _anchor1;
  // Setup
  [SerializeField] float _nodeDensity = 5f; // per meter

  // Physics
  [SerializeField] float _gravity = 10f;
  [SerializeField] float _nodeMass = 3f;
  [SerializeField] float _drag = 3f;
  [SerializeField] float _stiffness = 0.5f;
  [SerializeField] FloatRange _stretchLimits = new FloatRange(0.5f, 1.5f);

  // Private Fields
  LineRenderer _lr;
  CableNode[] _nodes;
  CableNode _root, _leaf;

  int _numNodes;
  float _totalLength, _restLength;
  bool _hasInit = false;

  // Public Fields
  public int numNodes => _numNodes;

  void Awake()
  {
    if(!_hasInit) Init(_anchor0, _anchor1);
  }
  // Init
  //----------------------------------------------------------------------------------------------------
  public void Init(Transform anchor0, Transform anchor1)
  {
    _lr = GetComponent<LineRenderer>();
    _anchor0 = anchor0;
    _anchor1 = anchor1;

    Vector3 heading = _anchor1.position - _anchor0.position;

    _totalLength = heading.magnitude;
    _numNodes = (_totalLength * _nodeDensity).FloorToInt();
    _restLength = _totalLength / _numNodes;

    _nodes = new CableNode[_numNodes];
    _lr.positionCount = _numNodes;
    _lr.SetPositions(new Vector3[_numNodes]);

    Vector3 segment = _restLength * heading.normalized;

    // Create CableNodes
    for(int i = 0; i < _numNodes; i++)
    {
      Vector3 startPos = _anchor0.position + (i * segment);
      _nodes[i] = new CableNode(startPos, _nodeMass,_gravity, _restLength, _stiffness);
      _lr.SetPosition(i, startPos);
    }

    // Assign node status and parents and children
    for(int i = 0; i < _numNodes; i++)
    {
      _nodes[i].isRoot = (i == 0);
      _nodes[i].isLeaf = (i == _numNodes - 1);
      _nodes[i].isSheep = (i > 0 && i < _numNodes - 1);

      if(_nodes[i].isRoot)
      {
        _root = _nodes[i];
        _nodes[i].child = _nodes[i + 1];
      }
      else if(_nodes[i].isLeaf)
      {
        _leaf = _nodes[i];
        _nodes[i].parent = _nodes[i - 1];
      }
      else if(_nodes[i].isSheep)
      {
        _nodes[i].parent = _nodes[i - 1];
        _nodes[i].child = _nodes[i + 1];
      }
    }

    _hasInit = true;
  }

  // MonoBehaviour
  //----------------------------------------------------------------------------------------------------
  void FixedUpdate()
  {
    if(!_hasInit)
      return;

    Step(Time.deltaTime);

    // Render
    for(int i = 0; i < _numNodes; i++) { _lr.SetPosition(i, transform.InverseTransformPoint(_nodes[i].position)); }
  }

  void OnDrawGizmos()
  {
    if(_nodes.IsNullOrEmpty())
      return;

    for(int i = 0; i < _numNodes; i++)
    {
      Gizmos.color = RGB.yellow;
      Gizmos.DrawSphere(_nodes[i].position, 0.1f);
    }
  }

  // Step
  //----------------------------------------------------------------------------------------------------
  void Step(float deltaTime)
  {
    _root.position = _anchor0.position;
    _leaf.position = _anchor1.position;

    for(int i = 0; i < _numNodes; i++)
    {
      _nodes[i].Step(_drag, deltaTime);
    }
    // ApplyDistanceConstraint();
  }

  // Distance Constraint
  void ApplyDistanceConstraint()
  {
    for(int iteration = 0; iteration < 10; iteration++)
    {
      bool success = true;

      for(int i = 1; i < _numNodes - 1; i++)
      {
        CableNode node = _nodes[i];
        Vector3 mid = Vector3.zero;
        if(!node.isLeaf)
        {
          Vector3 forward = node.child.position - node.position;
          if(forward.magnitude > _stretchLimits.Max)
            mid += node.child.position - forward.normalized * _stretchLimits.Max;
        }

        if(!node.isRoot)
        {
          Vector3 backward = node.parent.position - node.position;
          if(backward.magnitude > _stretchLimits.Max)
            mid += node.parent.position - backward.normalized * _stretchLimits.Max;
        }

        mid /= node.isSheep ? 2f : 1f;
        _nodes[i].position = mid;

        // Check if total length is within constraint
        if(node.childDistance > _stretchLimits.Max || node.parentDistance > _stretchLimits.Max)
        {
          success = false;
          break;
        }
      }

      if(success)
        break;
    }
  }
}
