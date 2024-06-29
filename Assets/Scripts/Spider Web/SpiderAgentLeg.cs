using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor.Experimental.GraphView;
using UnityEngine;

public class SpiderAgentLeg : MonoBehaviour
{
    private ConfigurableJoint _webJoint;
    private bool _webJointFullyConnected = false;
    private SpiderWebNode _targetNode;
    private JointDrive _jointDrive;
    [SerializeField] private int legDirection = -1;
    void Start()
    {
        
    }

    //  
    void Update()
    {
        if (_targetNode != null && _webJoint != null && !_webJointFullyConnected)
        {
            if ((transform.position - _targetNode.transform.position).magnitude < 0.1f)
            {
                 
                _webJointFullyConnected = true;
                _webJoint.massScale = 1;
                _webJoint.connectedMassScale = 1;
                _jointDrive.positionSpring = 7200;
                _webJoint.xDrive = _webJoint.yDrive = _webJoint.zDrive = _jointDrive;
            }
        }
    }
    public void CreateNewJoint(SpiderWebNode target)
    {
        if (_targetNode != null)
        {
            _targetNode.SetOccupied(false);
        }
        if (_webJoint != null) Destroy(_webJoint);
        //
        _webJointFullyConnected = false;
        //
        _targetNode = target;
        _targetNode.SetOccupied(true);
        // 
        //
        _webJoint = gameObject.AddComponent<ConfigurableJoint>();
        _webJoint.connectedBody = target.GetComponent<Rigidbody>();
        _webJoint.autoConfigureConnectedAnchor = false;
        _webJoint.anchor = new Vector3(0, 0, 0.02f);
        _webJoint.connectedAnchor = Vector3.zero;
       
        _jointDrive = _webJoint.xDrive;
        _jointDrive.positionSpring = 840;
        _jointDrive.positionDamper = 25f;
        // drive.useAcceleration = true;
        _webJoint.xDrive = _webJoint.yDrive = _webJoint.zDrive = _jointDrive;
        _webJoint.massScale = 1;
        _webJoint.connectedMassScale = 0;
    }

    public  void TryFindNewNode(SpiderWebNetwork network, Transform centerTransform, Vector3 direction)
    {
        SpiderWebNode nextNode = network.FindNextNode(transform.position, legDirection < 0 ? centerTransform.position.x - 1.7f : centerTransform.position.x - 0.1f, legDirection > 0 ? centerTransform.position.x + 1.7f : centerTransform.position.x + 0.1f, direction);
        if (nextNode != null)
        {
            

            CreateNewJoint(nextNode);
        }
    }
    public bool HasFullyAttached()
    {
        return _webJointFullyConnected || _targetNode == null;
    }
}
