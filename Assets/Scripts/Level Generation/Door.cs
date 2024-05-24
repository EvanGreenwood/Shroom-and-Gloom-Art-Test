using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Splines;

public class Door : MonoBehaviour, IPointerClickHandler
{
    public Animator Animation;
    private static readonly int Open = Animator.StringToHash("Open");
    private static readonly int Close = Animator.StringToHash("Close");
    public DoorMask DoorMask;

    public Door[] Friends;
    
    public SplineContainer DoorSpine;

    private TunnelGenerator _maskingTunnel;
    
    private List<Color> _baseDoorColors = new List<Color>();
    
    [SerializeField] private List<SpriteRenderer> _toColor;
    
    public void SetSpriteColors(Color color)
    {
        if (_baseDoorColors.Count == 0)
        {
            _baseDoorColors.Clear();
            foreach (SpriteRenderer sr in _toColor)
            {
                _baseDoorColors.Add(sr.color);
            }
        }
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
        
        Gizmos.color = c;
    }

    private bool _opened;
    public void OnPointerClick(PointerEventData eventData)
    {
        if (_opened)
        {
            return;
        }
        _opened = true;

        ServiceLocator.Get<Player>().SetOverrideSpline(DoorSpine);
        DoorMask.gameObject.SetActive(true);

        Animation.ResetTrigger(Close);
        Animation.SetTrigger(Open);

        foreach (Door friend in Friends)
        {
            friend.AskToClose();
        }
    }

    public void AskToClose()
    {
        _opened = false;
        Animation.ResetTrigger(Open);
        Animation.SetTrigger(Close);
    }
}
