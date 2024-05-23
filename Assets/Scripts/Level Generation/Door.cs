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
    public DoorMask DoorMask;

    

    public Transform EntryWaypoint;
    public Transform ExitWaypoint;

    private TunnelGenerator _maskingTunnel;
    
    private List<Color> _baseDoorColors = new List<Color>();
    
    [SerializeField] private List<SpriteRenderer> _toColor;
    
    
    public void SetSpriteColors(Color color)
    {
        Debug.Assert(_toColor.Count == _baseDoorColors.Count);
        for (int i = 0; i < _toColor.Count; i++)
        {
            SpriteRenderer toColor = _toColor[i];
            toColor.color = color * _baseDoorColors[i];
        }
    }
    
    public TunnelGenerator MaskingTunnel
    {
        get => _maskingTunnel;
        set
        {
            _maskingTunnel = value;
            //will cause DoorMask to inform rendering system of new mask.
            DoorMask.MaskRef = _maskingTunnel.TunnelIndex;
        }
    }

    public void Start()
    {
        _baseDoorColors.Clear();
        foreach (SpriteRenderer sr in _toColor)
        {
            _baseDoorColors.Add(sr.color);
        }
        
        DoorMask.gameObject.SetActive(false);
    }

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

        Gizmos.color = Color.blue;
        float offset = 0.05f;
        Gizmos.DrawLine(EntryWaypoint.transform.position + -transform.right * offset, ExitWaypoint.transform.position + -transform.right * offset);
        Gizmos.DrawLine(EntryWaypoint.transform.position + transform.right * offset, ExitWaypoint.transform.position + transform.right * offset);
        Gizmos.color = c;
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        DoorMask.gameObject.SetActive(true);
        Animation.SetTrigger(Open);
    }
}
