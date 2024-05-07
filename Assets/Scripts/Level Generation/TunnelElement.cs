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
