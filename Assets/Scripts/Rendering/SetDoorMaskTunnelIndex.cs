using System.Collections.Generic;
using UnityEngine;

//For testing door masks are working

[ExecuteAlways]
[RequireComponent(typeof(Renderer))]
public class SetDoorMaskTunnelIndex : MonoBehaviour
{
    public int Value;
    public bool SetChildren;
    
    private Renderer _renderer;
    private List<Renderer> _children;
    private static readonly int TunnelIndex = Shader.PropertyToID("_TunnelIndex");

    private void Start()
    {
        _renderer = GetComponent<Renderer>();
        _children = new List<Renderer>(GetComponentsInChildren<Renderer>());
        _children.Remove(_renderer);
    }

    void Update()
    {
        MaterialPropertyBlock mpb = new MaterialPropertyBlock();
        _renderer.GetPropertyBlock(mpb);
        mpb.SetInt(TunnelIndex, Value);
        _renderer.SetPropertyBlock(mpb);

        if (SetChildren)
        {
            foreach (Renderer child in _children)
            {
                MaterialPropertyBlock mpbc = new MaterialPropertyBlock();
                child.GetPropertyBlock(mpbc);
                mpbc.SetInt(TunnelIndex, Value);
                child.SetPropertyBlock(mpbc);
            }
        }
    }
}
