using NaughtyAttributes;
using Ross.EditorRuntimeCombatibility;
using System;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;
using UnityEngine.Rendering;

[ExecuteAlways]
[DefaultExecutionOrder(-400)]
public class RenderingManager : MonoService
{
    [Serializable]
    public class DoorMaskData
    {
        public DoorMaskData(Renderer render, int maskValue)
        {
            Renderer = render;
            MaskValue = maskValue;
        }

        public Renderer Renderer;
        public int MaskValue;
    }
    
    public Material DoorMaskMaterial;
    public bool MaskSceneCamera;
    
    [ReadOnly]
    public List<DoorMaskData> DoorMaskRenderers = new List<DoorMaskData>();
    [ReadOnly]
    public List<DoorMaskEnabledCamera> ActiveCameras = new List<DoorMaskEnabledCamera>();

    public static readonly int MaskThresholdIndex = Shader.PropertyToID("_MaskThresholdIndex");
    private static readonly int UseDoorMasking = Shader.PropertyToID("_UseDoorMasking");

    public void AddDoorMask(Renderer renderer, int maskValue)
    {
        DoorMaskRenderers.Add(new DoorMaskData(renderer, maskValue));

        ActiveCameras ??= new List<DoorMaskEnabledCamera>();
        foreach (DoorMaskEnabledCamera cam in ActiveCameras)
        {
            if (cam == null)
            {
                continue;
            }
            cam.Refresh();
        }
    }
    
    public void RemoveDoorMask(Renderer renderer)
    {
        for (int i = DoorMaskRenderers.Count - 1; i >= 0; i--)
        {
            DoorMaskData d = DoorMaskRenderers[i];

            if (d.Renderer == null)
            {
                DoorMaskRenderers.RemoveAt(i);
                continue;
            }
            
            if (d.Renderer == renderer)
            {
                DoorMaskRenderers.RemoveAt(i);
                return;
            }
        }

        ActiveCameras ??= new List<DoorMaskEnabledCamera>();
        foreach (DoorMaskEnabledCamera cam in ActiveCameras)
        {
            if (cam == null)
            {
                continue;
            }
            cam.Refresh();
        }
    }
    
    private void LateUpdate()
    {
        Shader.SetGlobalInt(UseDoorMasking, 0);
        List<Camera> activeCameras = new (Camera.allCameras);
        
        #if UNITY_EDITOR
        if (MaskSceneCamera)
        {
            Camera[] sceneCameras = SceneView.GetAllSceneCameras();
            activeCameras.AddRange(sceneCameras);
        }
        #endif

        foreach (Camera cam in activeCameras)
        {
            if (cam.orthographic)
            {
                continue;
            }
            
            if (cam.TryGetComponent(out DoorMaskEnabledCamera existing))
            {
                if (!ActiveCameras.Contains(existing))
                {
                    ActiveCameras.Add(existing);
                }
                continue;
            }

            DoorMaskEnabledCamera maskEnabledCam = cam.AddComponent<DoorMaskEnabledCamera>();
            ActiveCameras.Add(maskEnabledCam);
        }

        //Ensure if a camera gets destroyed or whatever and doesnt unsubscribe, we still remove it.
        for (int i = ActiveCameras.Count - 1; i >= 0; i--)
        {
            if (ActiveCameras[i] == null)
            {
                ActiveCameras.RemoveAt(i);
            }
        }
    }

    protected void Start()
    {
        Debug.Log("[RenderingManager] Setting door mask threshold to: 0");
        Shader.SetGlobalInt(MaskThresholdIndex, 0);
    }

    protected new void OnEnable()
    {
        base.OnEnable();
        foreach (DoorMaskEnabledCamera cam in ActiveCameras)
        {
            Safe.Destroy(cam);
        }
    }

    protected new void OnDisable()
    {
        foreach (DoorMaskEnabledCamera cam in ActiveCameras)
        {
            Safe.Destroy(cam);
        }
        base.OnDisable();
    }
}
