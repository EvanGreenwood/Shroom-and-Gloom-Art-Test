using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Experimental.Rendering;
using UnityEngine.Rendering;

[ExecuteAlways]
[RequireComponent(typeof(Camera))]
public class DoorMaskEnabledCamera : MonoBehaviour
{
    private Camera _camera;
    private CommandBuffer _commandBuffer;
    private RenderingManager _renderingManager;
    private static readonly int MaskRef = Shader.PropertyToID("_MaskRef");
    private static readonly int UseDoorMasking = Shader.PropertyToID("_UseDoorMasking");

    private void Awake()
    {
        hideFlags = HideFlags.DontSave;
        _camera = GetComponent<Camera>();
    }

    void OnEnable()
    {
        Refresh();
    }
    
    protected void OnDisable()
    {
        if (_camera != null)
        {
            _camera.RemoveCommandBuffer(CameraEvent.BeforeGBuffer, _commandBuffer);
        }
    }

    public void Refresh()
    {
        RefreshDoorMaskCommandBuffer();
    }

    public void OnPreRender()
    {
        Shader.SetGlobalInt(UseDoorMasking, 1);
    }
    
    public void OnPostRender()
    {
        Shader.SetGlobalInt(UseDoorMasking, 0);
    }

    void RefreshDoorMaskCommandBuffer()
    {
        if (_renderingManager == null)
        {
            _renderingManager = ServiceLocator.Get<RenderingManager>();
        }

        if (_camera == null)
        {
            _camera = GetComponent<Camera>();
        }

        if (_commandBuffer != null)
        {
            _camera.RemoveCommandBuffer(CameraEvent.BeforeGBuffer, _commandBuffer);
        }
        
        _commandBuffer = new CommandBuffer { name = "Render Door Mask" };
        
        int texName = Shader.PropertyToID("_DoorMask");

        _commandBuffer.Clear();
        _commandBuffer.GetTemporaryRT(texName,
            _camera.pixelWidth,
            _camera.pixelHeight,
            0,
            FilterMode.Point);
        _commandBuffer.SetRenderTarget(new RenderTargetIdentifier(texName));
        //White, since by default we want everything visible.
        _commandBuffer.ClearRenderTarget(true, true, Color.white);
        foreach ( RenderingManager.DoorMaskData data in _renderingManager.DoorMaskRenderers)
        {
            MaterialPropertyBlock props = new MaterialPropertyBlock();
            data.Renderer.GetPropertyBlock(props);
            props.SetInt(MaskRef, data.MaskValue);
            data.Renderer.SetPropertyBlock(props);
            
            _commandBuffer.DrawRenderer(data.Renderer, _renderingManager.DoorMaskMaterial);
        }
        _camera.AddCommandBuffer(CameraEvent.BeforeGBuffer, _commandBuffer);
    }
}
