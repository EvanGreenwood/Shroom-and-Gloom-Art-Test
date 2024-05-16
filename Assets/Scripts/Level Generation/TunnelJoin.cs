using JetBrains.Annotations;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class TunnelJoin : MonoBehaviour
{
    public Transform DoorParentLinear;
    
    [Space]
    public Transform DoorParentBranch;
    public Transform DoorParentThranch; //Dont tell me how to live my life
    
    public TunnelGenerator InTunnel { get; set; }
    public List<TunnelGenerator> OutTunnels { get; set; }

    private int _totalTunnelCount = -1;

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
            
            position = DoorParentLinear.GetChild(0).position;
            rotation = DoorParentLinear.GetChild(0).rotation;
        }
        else if(totalTunnelCount == 2)
        {
            DoorParentLinear.gameObject.SetActive(false);
            DoorParentThranch.gameObject.SetActive(false);
            
           if(OutTunnels.Count == 1)
           {
               position = DoorParentBranch.GetChild(0).position;
               rotation = DoorParentBranch.GetChild(0).rotation;
           }
           else //if(OutTunnels.Count == 2)
           { 
               position = DoorParentBranch.GetChild(1).position;
               rotation = DoorParentBranch.GetChild(1).rotation;
           }
        }
        else
        {
            DoorParentLinear.gameObject.SetActive(false);
            DoorParentBranch.gameObject.SetActive(false);
            
            if(OutTunnels.Count == 1)
            {
                position = DoorParentThranch.GetChild(0).position;
                rotation = DoorParentThranch.GetChild(0).rotation;
            }
            else if(OutTunnels.Count == 2)
            { 
                position = DoorParentThranch.GetChild(1).position;
                rotation = DoorParentThranch.GetChild(1).rotation;
            }
            else //if(OutTunnels.Count == 2)
            { 
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
