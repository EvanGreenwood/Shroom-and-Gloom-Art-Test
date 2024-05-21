using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.EventSystems;

public class Door : MonoBehaviour, IPointerClickHandler
{
    public Animator Animation;
    private static readonly int Open = Animator.StringToHash("Open");

    private void OnDrawGizmos()
    {
        if (transform.parent == null)
        {
            return;
        }
        
        Color c = Gizmos.color;
        if (transform.parent.childCount == 1)
        {
            Gizmos.color = Color.gray;
        }
        if (transform.parent.childCount == 2)
        {
            Gizmos.color = Color.yellow;
        }
        if (transform.parent.childCount == 3)
        {
            Gizmos.color = Color.red;
        }
        
        Gizmos.DrawLine(transform.position, transform.position+transform.forward*5);
        Gizmos.DrawLine(transform.position+transform.forward*5, transform.position+transform.forward*4 + -transform.right);
        Gizmos.DrawLine(transform.position+transform.forward*5, transform.position+transform.forward*4 + transform.right);
        Gizmos.color = c;
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        Animation.SetTrigger(Open);
    }
}
