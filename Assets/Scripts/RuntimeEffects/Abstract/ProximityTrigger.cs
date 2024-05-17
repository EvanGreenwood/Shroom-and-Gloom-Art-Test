using Framework;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class ProximityTrigger : MonoBehaviour
{
    private CircleCollider2D _collider;

    public LayerMask ValidTriggerSources;

    public float ProximityRadius;

    private void Awake()
    {
        _collider = gameObject.AddComponent<CircleCollider2D>();
        _collider.isTrigger = true;
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

    private void OnDrawGizmos()
    {
        Gizmos.DrawWireSphere(transform.position, ProximityRadius);
    }
}
