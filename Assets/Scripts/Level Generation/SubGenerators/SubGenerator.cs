using Framework;
using NaughtyAttributes;
using System;
using UnityEngine;

[ExecuteAlways]
public abstract class SubGenerator : MonoBehaviour
{
    private TunnelElement _parent;
    public void Start()
    {
        _parent = GetComponentInParent<TunnelElement>();
        if (_parent)
        {
            _parent.SubscribeGenerator(this);
        }
        else
        {
            //Dont run in editor
            if (Application.isPlaying)
            {
                //If no parent the generator can do its own thing, else parent will call generate.
                Generate();
            }
        }
    }
    
    public void OnDestroy()
    {
        if (Application.isPlaying)
        {
            if (_parent)
            {
                _parent.UnSubscribeGenerator(this);
            }
        }
    }

    //override to add your own generation stuff. Dont use Start, Awake etc
    public abstract void Generate();
}
