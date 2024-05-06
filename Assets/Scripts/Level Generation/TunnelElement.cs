#region Usings
using UnityEngine;
using Framework;
using Mainframe;
#endregion

public class TunnelElement : MonoBehaviour
{
  SpriteRenderer _spriteRenderer;
  SpriteRenderer[] _childRenderers;
  bool _hasInit;

  public SpriteRenderer spriteRenderer => _spriteRenderer;

  // Init
  //----------------------------------------------------------------------------------------------------
  public void Init()
  {
    _spriteRenderer = GetComponent<SpriteRenderer>();
    _childRenderers = GetComponentsInChildren<SpriteRenderer>();
    _hasInit = true;
  }

  // MonoBehaviour
  //----------------------------------------------------------------------------------------------------
  void Awake()
  {
    if(!_hasInit)
      Init();
  }

  public void SetColor(Color color)
  {
    _spriteRenderer.color = color;
    _childRenderers.Foreach(sr =>
    {
      if(sr.gameObject.activeInHierarchy)
      {
        sr.color = color;
      }
    });
  }
}
