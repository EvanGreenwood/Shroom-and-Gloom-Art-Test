using Framework;
using JetBrains.Annotations;
using NaughtyAttributes;
using System;
using UnityEngine;

public class SGFlip : SubGenerator
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
        InvertScaleYOnFlip = 256,
        InvertScaleX = 512,
        InvertScaleY = 1024
    }
    
    //default flip mode. Works for most things
    public FlipMode FlipFlags = FlipMode.FlipLocalPosX | FlipMode.FlipLocalRotation;
    
    protected bool Flip(FlipMode flipMode)
    {
        bool flipped = DefaultPosFlip(flipMode) || DefaultSpriteFlip(flipMode);
        Transform trans = transform;
        if (flipMode.HasFlag(FlipMode.InvertScaleX))
        {
            trans.localScale = trans.localScale.WithX(-trans.localScale.x);
            flipped = true;
        }
        
        if (flipMode.HasFlag(FlipMode.InvertScaleY))
        {
            trans.localScale = trans.localScale.WithY(-trans.localScale.y);
            flipped = true;
        }

        return flipped;
    }

    [Button("Test")][UsedImplicitly]
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
                trans.localScale = trans.localScale.WithX(-trans.localScale.x);
            }
            performed++;
        }
        
        if (flipMode.HasFlag(FlipMode.FlipLocalPosY))
        {
            trans.localPosition = trans.localPosition.FlipY();
            if (flipMode.HasFlag(FlipMode.InvertScaleYOnFlip))
            {
                trans.localScale = trans.localScale.WithY(-trans.localScale.y);
            }
            performed++;
        }
        
        if (flipMode.HasFlag(FlipMode.FlipWorldPosX))
        {
            trans.localPosition = trans.position - trans.localPosition.x00();
            if (flipMode.HasFlag(FlipMode.InvertScaleXOnFlip))
            {
                trans.localScale = trans.localScale.WithX(-trans.localScale.x);
            }
            performed++;
        }
        
        if (flipMode.HasFlag(FlipMode.FlipWorldPosY))
        {
            trans.localPosition = trans.position - trans.localPosition.y00();
            if (flipMode.HasFlag(FlipMode.InvertScaleYOnFlip))
            {
                trans.localScale = trans.localScale.WithY(-trans.localScale.y);
            }
            performed++;
        }

        if (flipMode.HasFlag(FlipMode.FlipLocalRotation))
        {
            trans.localRotation = Quaternion.Inverse(trans.localRotation);
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

    public override void Generate()
    {
        //Flip does not do anything on generate, since its handled
    }
}
