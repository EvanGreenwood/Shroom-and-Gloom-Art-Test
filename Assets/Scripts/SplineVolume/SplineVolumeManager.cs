
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
    private float _lastUpdateTime;
    private float _currentTransitionTime;
    private SplineVolume _targetVolume;
    private SplineVolume _lastVolume;
    private SplineVolume.SplineVolumeSettings _currentSettings;
    private PostProcessVolume _oldVolume;
    private PostProcessVolume _newVolume;

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

            //Debug.LogError("Chosen Tunnel:" + tunnel.gameObject.name + $" Sample point: {active.transform.name} - {active.transform.position}");
            if (_targetVolume != closestVolume)
            {
                if (_oldVolume == null)
                {
                    _oldVolume = gameObject.AddComponent<PostProcessVolume>();
                    _oldVolume.isGlobal = true;
                    _oldVolume.hideFlags = HideFlags.DontSave;
                }

                if (_newVolume == null)
                {
                    _newVolume = gameObject.AddComponent<PostProcessVolume>();
                    _oldVolume.isGlobal = true;
                    _newVolume.hideFlags = HideFlags.DontSave;
                }

                if (_currentSettings == null)
                {
                    _currentSettings = closestVolume.Settings;
                    _lastVolume = closestVolume;
                    _targetVolume = closestVolume;
                    _currentTransitionTime = closestVolume.Settings.TransitionInTime;
                    _oldVolume.profile = closestVolume.Settings.PostProcessingProfile;
                    _newVolume.profile = closestVolume.Settings.PostProcessingProfile;
                    _oldVolume.weight = 0;
                    _newVolume.weight = 1;
                }
                else
                {
                    
                    _lastVolume = _targetVolume;
                    _targetVolume = closestVolume;
                    _currentTransitionTime = 0;
                    _oldVolume.profile = _newVolume.profile;
                    _newVolume.profile = closestVolume.Settings.PostProcessingProfile;
                    _oldVolume.weight = 1;
                    _newVolume.weight = 0;
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
        
        float dt = SafeTime.Time - _lastUpdateTime;
        _currentTransitionTime += dt;
        float transitionInTime = _targetVolume.Settings.TransitionInTime;
        float normTime = Mathf.Clamp01(_currentTransitionTime / transitionInTime);
        _currentSettings.FogDistance = Mathf.Lerp(_lastVolume.Settings.FogDistance, _targetVolume.Settings.FogDistance, normTime);
        _currentSettings.FogColor = Color.Lerp(_lastVolume.Settings.FogColor, _targetVolume.Settings.FogColor, normTime);
        
        _oldVolume.weight = 1f - normTime;
        _newVolume.weight = normTime;
        
        _lastUpdateTime = SafeTime.Time;
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
            cam.backgroundColor = _currentSettings.FogColor;
        }
    }
}
