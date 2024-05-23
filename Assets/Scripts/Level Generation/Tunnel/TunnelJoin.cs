using JetBrains.Annotations;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[ExecuteAlways]
public class TunnelJoin : MonoBehaviour
{
    public Transform DoorParentLinear;
    
    [Space]
    public Transform DoorParentBranch;
    public Transform DoorParentThranch; //Dont tell me how to live my life

    private TunnelGenerator _inTunnel;
    public TunnelGenerator InInTunnel
    {
        get
        {
            return _inTunnel;
        }
        set
        {
            _inTunnel = value;
            List<Door> doors = new List<Door>(DoorParentLinear.GetComponentsInChildren<Door>());
            doors.AddRange(DoorParentBranch.GetComponentsInChildren<Door>());
            doors.AddRange(DoorParentThranch.GetComponentsInChildren<Door>());

            foreach (Door door in doors)
            {
                door.SetSpriteColors(_inTunnel.ColorGradient.Evaluate(1));
            }
        }
    }

    public List<TunnelGenerator> OutTunnels { get; set; }

    private int _totalTunnelCount = -1;

    private void Awake()
    {
        DoorParentLinear.gameObject.SetActive(false);
        DoorParentBranch.gameObject.SetActive(false);
        DoorParentThranch.gameObject.SetActive(false);
    }

    public void AddOutTunnel(int totalTunnelCount, TunnelGenerator tunnelInstance, out Vector3 position, out Quaternion rotation)
    {
        Debug.Assert(_totalTunnelCount == -1 || totalTunnelCount == _totalTunnelCount, $"A PATH IS CHANGING AN ALREADY SET TUNNEL OUT COUNT. New: {totalTunnelCount} vs Current: {_totalTunnelCount}", gameObject);
        _totalTunnelCount = totalTunnelCount;
        OutTunnels ??= new List<TunnelGenerator>();
        OutTunnels.Add(tunnelInstance);

        if (totalTunnelCount == 1)
        {
            DoorParentBranch.gameObject.SetActive(false);
            DoorParentThranch.gameObject.SetActive(false);
            DoorParentLinear.gameObject.SetActive(true);
            
            Door linearDoor = DoorParentLinear.GetChild(0).GetComponent<Door>();
            linearDoor.MaskingTunnel = tunnelInstance;
            position = DoorParentLinear.GetChild(0).position;
            rotation = DoorParentLinear.GetChild(0).rotation;
        }
        else if(totalTunnelCount == 2)
        {
            DoorParentLinear.gameObject.SetActive(false);
            DoorParentThranch.gameObject.SetActive(false);
            DoorParentBranch.gameObject.SetActive(true);
            
           if(OutTunnels.Count == 1)
           {
               Door branchDoor1 = DoorParentBranch.GetChild(0).GetComponent<Door>();
               branchDoor1.MaskingTunnel = tunnelInstance;
               position = DoorParentBranch.GetChild(0).position;
               rotation = DoorParentBranch.GetChild(0).rotation;
           }
           else //if(OutTunnels.Count == 2)
           { 
               Door branchDoor2 = DoorParentBranch.GetChild(1).GetComponent<Door>();
               branchDoor2.MaskingTunnel = tunnelInstance;
               position = DoorParentBranch.GetChild(1).position;
               rotation = DoorParentBranch.GetChild(1).rotation;
           }
        }
        else
        {
            DoorParentLinear.gameObject.SetActive(false);
            DoorParentBranch.gameObject.SetActive(false);
            DoorParentThranch.gameObject.SetActive(true);
            
            if(OutTunnels.Count == 1)
            {
                Door thranchDoor1 = DoorParentThranch.GetChild(0).GetComponent<Door>();
                thranchDoor1.MaskingTunnel = tunnelInstance;
                position = DoorParentThranch.GetChild(0).position;
                rotation = DoorParentThranch.GetChild(0).rotation;
            }
            else if(OutTunnels.Count == 2)
            { 
                Door thranchDoor2 = DoorParentThranch.GetChild(1).GetComponent<Door>();
                thranchDoor2.MaskingTunnel = tunnelInstance;
                position = DoorParentThranch.GetChild(1).position;
                rotation = DoorParentThranch.GetChild(1).rotation;
            }
            else //if(OutTunnels.Count == 2)
            { 
                Door thranchDoor3 = DoorParentThranch.GetChild(2).GetComponent<Door>();
                thranchDoor3.MaskingTunnel = tunnelInstance;
                position = DoorParentThranch.GetChild(2).position;
                rotation = DoorParentThranch.GetChild(2).rotation;
            }
        }
    }

    private void OnDrawGizmos()
    {
        if (_totalTunnelCount == -1)
        {
            return;
        }

        if (_totalTunnelCount == 1)
        {
            Handles.Label(transform.position, $"*");
        }
        else
        {
            Handles.Label(transform.position, $"{_totalTunnelCount} Doors");
        }
    }
}
