using Framework;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class ProximityTrigger : MonoBehaviour
{
    private SphereCollider _collider;

    public LayerMask ValidTriggerSources;

    public float ProximityRadius;

    private void Awake()
    {
        _collider = gameObject.AddComponent<SphereCollider>();
        _collider.isTrigger = true;
        _collider.radius = ProximityRadius;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (ValidTriggerSources.Contains(other.gameObject.layer))
        {
            ProximityEnter(other.gameObject);
        }
    }
    
    private void OnTriggerExit(Collider other)
    {
        if (ValidTriggerSources.Contains(other.gameObject.layer))
        {
            ProximityExit(other.gameObject);
        }
    }
    
    /*private void OnTriggerStay(Collider other)
    {
        if (ValidTriggerSources.Contains(other.gameObject.layer))
        {
            ProximityStay();
        }
         public virtual void ProximityStay() { }
    }*/
    
    public abstract void ProximityEnter(GameObject entered);
   
    public abstract void ProximityExit(GameObject exited);

    private void OnDrawGizmosSelected()
    {
        //Gizmos.DrawWireSphere(transform.position, ProximityRadius);
    }
}
