using Framework;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class SpiderWebNode : MonoBehaviour
{
    [SerializeField] private SpiderWebStrand _strandPrefab;
    //
    public SpiderWebNode[] connectedNodes;
    private List<SpiderWebStrand > _strands = new List<SpiderWebStrand>();
    private Collider _collider;
    private Rigidbody _rigidbody;
    //
    public bool IsOccupied => _isOccupied;
    private bool _isOccupied = false;
    void Start()
    {
        _collider = GetComponent<Collider>();
        _rigidbody = GetComponent<Rigidbody>();
        foreach (SpiderWebNode node in connectedNodes)
        {
            if (!node.ContainsNode(this) ||   node.transform.position.y < transform.position.y || (node.transform.position.y == transform.position.y && node.transform.position.x > transform.position.x))
            {
                SpiderWebStrand strand = Instantiate(_strandPrefab, transform.position, Quaternion.identity, transform.parent);
                strand.Setup(transform, node.transform);
                //
                ConfigurableJoint joint = gameObject.AddComponent<ConfigurableJoint>();
                joint.connectedBody = node.GetComponent<Rigidbody>();
                JointDrive drive = joint.xDrive;
                drive.positionSpring = 200;
                drive.positionDamper = 2.1f;
               // drive.useAcceleration = true;
                joint.xDrive = joint.yDrive =joint.zDrive = drive;
              
            }
        }
    }
    public bool ContainsNode(SpiderWebNode node)
    {
        return connectedNodes.Contains(node);
    }
    private void OnDrawGizmos()
    {
        foreach (SpiderWebNode node in connectedNodes)
        {
            Gizmos.color = Color.green;
           Gizmos.DrawLine(transform.position, node.transform.position);
            
        }
    }
    // 
    void Update()
    {
        if (_collider.Raycast(Camera.main.GetMouseRay(), out RaycastHit hit, 4))
        {
            _rigidbody.AddForce(Vector3.down * 50 * Time.deltaTime, ForceMode.VelocityChange);
        }
    }
    public void SetOccupied(bool occupied)
    {
        _isOccupied = occupied;
    }
}
