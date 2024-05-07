using Framework;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class SubGenerator : MonoBehaviour
{
    [Flags]
    public enum FlipMode
    {
        None = 0,
        FlipSpriteX = 1,
        FlipSpriteY = 2,
        FlipLocalPosY = 4,
        FlipLocalPosX = 8,
        FlipWorldPosY = 16,
        FlipWorldPosX = 32,
    }

    public FlipMode FlipFlags;
    private TunnelElement _parent;
    public void OnEnable()
    {
        _parent = GetComponentInParent<TunnelElement>();
        if (_parent)
        {
            _parent.SubscribeGenerator(this);
        }
        else
        {
            //If no parent the generator can do its own thing, else parent will call generate.
            Generate();
        }
    }

    public void OnDisable()
    {
        if (_parent)
        {
            _parent.UnSubscribeGenerator(this);
        }
    }

    //override to add your own generation stuff. Dont use Start, Awake etc
    public abstract void Generate();
    
    protected abstract bool Flip(FlipMode flipMode);
    public bool RequestFlip()
    {
        return Flip(FlipFlags);
    }

    public bool DefaultPosFlip(FlipMode flipMode)
    {
        int performed = 0;
        Transform trans = transform;
        
        if (flipMode.HasFlag(FlipMode.FlipLocalPosX))
        {
            trans.localPosition = trans.localPosition.FlipX();
            performed++;
        }
        
        if (flipMode.HasFlag(FlipMode.FlipLocalPosY))
        {
            trans.localPosition = trans.localPosition.FlipY();
            performed++;
        }
        
        if (flipMode.HasFlag(FlipMode.FlipWorldPosX))
        {
            trans.localPosition = trans.position - trans.localPosition.x00();
            performed++;
        }
        
        if (flipMode.HasFlag(FlipMode.FlipWorldPosY))
        {
            trans.localPosition = trans.position - trans.localPosition.y00();
            performed++;
        }
        
        return performed != 0;
    }
    
    public bool DefaultSpriteFlip(FlipMode flipMode, params SpriteRenderer[] sprites)
    {
        bool flipX = flipMode.HasFlag(FlipMode.FlipSpriteX);
        bool flipY = flipMode.HasFlag(FlipMode.FlipSpriteY);
        if (flipX || flipY)
        {
            foreach (SpriteRenderer spriteRenderer in sprites)
            {
                //will override if already flipped.
                spriteRenderer.flipX = flipX;
                spriteRenderer.flipY = flipY;
            }

            return sprites.Length > 0;
        }
        return false;
    }
    
}
