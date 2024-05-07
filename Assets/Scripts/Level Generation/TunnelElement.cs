#region Usings
using UnityEngine;
using Framework;
using Mainframe;
using System;
using System.Collections.Generic;

#endregion

public class TunnelElement : MonoBehaviour
{
  SpriteRenderer _spriteRenderer;
  SpriteRenderer[] _childRenderers;
  bool _hasInit;

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

  public void SubGenerate()
  {
    for (int i = 0; i < _generators.Count; i++)
    {
      SubGenerator generator = _generators[i];
      generator.Generate();
    }
  }

  public void RequestSubGeneratorFlip()
  {
    foreach (SubGenerator sg in _generators)
    {
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
