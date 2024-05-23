
using Ross.EditorRuntimeCombatibility;
using System;
using Unity.VisualScripting;

#if UNITY_EDITOR
using UnityEditor;
#endif
using UnityEngine;
using UnityEngine.Rendering.PostProcessing;

[ExecuteAlways]
public class SplineVolumeManager : MonoService
{
    private float _currentTransitionTime;
    private SplineVolume _targetVolume;
    private SplineVolume _lastVolume;
    private SplineVolume.SplineVolumeSettings _currentSettings;

    private Service<Player> _player;
    private Service<WorldManagerService> _worldManager;
    
    private Camera GetActiveCamera()
    {
        Camera cam = null;
        if (_player.Exists)
        {
            cam = Camera.main;
        }

        if (cam == null)
        {
            
#if UNITY_EDITOR
            var view = SceneView.currentDrawingSceneView;
            if (view != null)
            {
                cam = view.camera;
            }
#endif
            
            if(cam == null)
            {
                cam = Camera.current;
            }
        }

        return cam;
    }
    
    public void LateUpdate()
    {
        if (!_worldManager.Exists)
        {
            Debug.LogWarning("[WorldVolumeManager] Cant use SplineVolumes. World Manager Service does not exist.");
            return;
        }
        Camera active = GetActiveCamera();

        if (active == null)
        {
            return;
        }
        
        if (_worldManager.Value.TryGetTunnel(active.transform.position, out TunnelGenerator tunnel))
        {
            SplineVolume closestVolume = tunnel.GenerationSettings.SplineVolume;
            
            if (_targetVolume != closestVolume)
            {
                if (_currentSettings == null)
                {
                    _currentSettings = closestVolume.Settings;
                    _lastVolume = closestVolume;
                    _targetVolume = closestVolume;
                    _currentTransitionTime = closestVolume.Settings.TransitionInTime;
                }
                else
                {
                    
                    _lastVolume = _targetVolume;
                    _targetVolume = closestVolume;
                    _currentTransitionTime = 0;
                }
            }
        }

        MoveTowardsTargetVolume();
        UpdateToCurrentSettings();
    }
    
    private void MoveTowardsTargetVolume()
    {
        if (_targetVolume == null || _currentSettings == null || _lastVolume == null)
        {
            return;
        }

        _currentTransitionTime += Safe.Time.deltaTime;
        float transitionInTime = _targetVolume.Settings.TransitionInTime;
        float normTime = Mathf.Clamp01(_currentTransitionTime / transitionInTime);
        _currentSettings.FogDistance = Mathf.Lerp(_lastVolume.Settings.FogDistance, _targetVolume.Settings.FogDistance, normTime);
        _currentSettings.FogColor = Color.Lerp(_lastVolume.Settings.FogColor, _targetVolume.Settings.FogColor, normTime);
        _currentSettings.BackgroundColor = Color.Lerp(_lastVolume.Settings.BackgroundColor, _targetVolume.Settings.BackgroundColor, normTime);
        //_oldVolume.weight = 1f - normTime;
        //_newVolume.weight = normTime;
    }
    
    private void UpdateToCurrentSettings()
    {
        if (_currentSettings == null)
        {
            return;
        }
        Shader.SetGlobalColor("FogColor", _currentSettings.FogColor);
        Shader.SetGlobalFloat("FogStart", _currentSettings.FogStart);
        Shader.SetGlobalFloat("FogDistance", _currentSettings.FogDistance);

        Camera cam = GetActiveCamera();
        if (cam)
        {
            cam.backgroundColor = _currentSettings.BackgroundColor;
        }
    }
}
