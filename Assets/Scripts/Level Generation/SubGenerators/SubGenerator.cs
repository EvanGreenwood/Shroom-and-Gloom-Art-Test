using Framework;
using NaughtyAttributes;
using System;
using UnityEngine;

[ExecuteAlways]
public class SubGenerator : MonoBehaviour
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
        FlipLocalRotation = 64,
        InvertScaleXOnFlip = 128,
        InvertScaleYOnFlip = 256
    }
    
    public FlipMode FlipFlags;
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
    public virtual void Generate() { }
    
    protected bool Flip(FlipMode flipMode)
    {
        return DefaultPosFlip(flipMode) || DefaultSpriteFlip(flipMode);
    }

    [Button("Test Flip")]
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

            if (flipMode.HasFlag(FlipMode.InvertScaleXOnFlip))
            {
                trans.localScale = trans.localScale.WithY(-trans.localScale.y);
            }
            performed++;
        }
        
        if (flipMode.HasFlag(FlipMode.FlipLocalPosY))
        {
            trans.localPosition = trans.localPosition.FlipY();
            if (flipMode.HasFlag(FlipMode.InvertScaleYOnFlip))
            {
                trans.localScale = trans.localScale.WithX(-trans.localScale.x);
            }
            performed++;
        }
        
        if (flipMode.HasFlag(FlipMode.FlipWorldPosX))
        {
            trans.localPosition = trans.position - trans.localPosition.x00();
            if (flipMode.HasFlag(FlipMode.InvertScaleXOnFlip))
            {
                trans.localScale = trans.localScale.WithY(-trans.localScale.y);
            }
            performed++;
        }
        
        if (flipMode.HasFlag(FlipMode.FlipWorldPosY))
        {
            trans.localPosition = trans.position - trans.localPosition.y00();
            if (flipMode.HasFlag(FlipMode.InvertScaleYOnFlip))
            {
                trans.localScale = trans.localScale.WithX(-trans.localScale.x);
            }
            performed++;
        }

        if (flipMode.HasFlag(FlipMode.FlipLocalRotation))
        {
            trans.rotation = Quaternion.Inverse(trans.rotation);
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
