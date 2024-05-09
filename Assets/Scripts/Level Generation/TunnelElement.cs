#region Usings
using UnityEngine;
using Framework;
using MathBad;
using System;
using System.Collections.Generic;

#endregion

public class TunnelContext
{
  public float DistanceElementIsAt;
  public float TunnelLength;
}

public class TunnelElement : MonoBehaviour
{
  SpriteRenderer _spriteRenderer;
  SpriteRenderer[] _childRenderers;
  bool _hasInit;

  private TunnelContext _context;
  private List<SubGenerator> _generators = new List<SubGenerator>();
  public SpriteRenderer spriteRenderer => _spriteRenderer;

  // Init
  //----------------------------------------------------------------------------------------------------
  public void Init()
  {
    _spriteRenderer = GetComponent<SpriteRenderer>();
    _childRenderers = GetComponentsInChildren<SpriteRenderer>();
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

  public void SetTunnelContext(TunnelContext context)
  {
    _context = context;

    SetSpriteRendererMaterialContext(_spriteRenderer);
    foreach (var sr in _childRenderers)
    {
      SetSpriteRendererMaterialContext(sr);
    }
  }

  private void SetSpriteRendererMaterialContext(SpriteRenderer sr)
  {
    MaterialPropertyBlock block = new MaterialPropertyBlock();
    sr.GetPropertyBlock(block);
    block.SetFloat("_TunnelDistance", _context.DistanceElementIsAt);
    sr.SetPropertyBlock(block);
  }

  public void SetColor(Color color)
  {
    if(!_hasInit)
      Init();

    if (_spriteRenderer)
    {
      _spriteRenderer.color = color;
    }

    for (int i = 0; i < _childRenderers.Length; i++)
    {
      SpriteRenderer sr = _childRenderers[i];

      if (sr && sr.gameObject.activeInHierarchy)
      {
        sr.color = color;
      }
    }
  }

  public void SubGenerate()
  {
    for (int i = _generators.Count - 1; i >= 0; i--)
    {
      SubGenerator sg = _generators[i];
      if (sg == null)
      {
        _generators.RemoveAt(i);
        i++;
        continue;
      }
      
      SubGenerator generator = _generators[i];
      generator.Generate();
    }
  }

  public void RequestSubGeneratorFlip()
  {
    for (int i = _generators.Count - 1; i >= 0; i--)
    {
      SubGenerator sg = _generators[i];

      if (sg == null)
      {
        _generators.RemoveAt(i);
        i++;
        continue;
      }
      
      sg.RequestFlip();
    }
  }

  public void SubscribeGenerator(SubGenerator toAdd)
  {
    _generators.Add(toAdd);
  }

  public void UnSubscribeGenerator(SubGenerator toRemove)
  {
    _generators.Remove(toRemove);
  }
}
