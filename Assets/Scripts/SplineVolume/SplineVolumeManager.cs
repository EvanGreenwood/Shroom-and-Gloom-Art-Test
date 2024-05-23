
using HorizonBasedAmbientOcclusion;
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
    private HBAO _hbao;

    private Service<Player> _player;
    private Service<WorldManagerService> _worldManager;
    
    private Camera GetActiveCamera()
    {
        Camera cam = null;
        if (_player.Exists)
        {
            cam = Camera.main;
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
                    _currentSettings = new SplineVolume.SplineVolumeSettings(closestVolume.Settings);
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
        
        //AO
        _currentSettings.Radius = Mathf.Lerp(_lastVolume.Settings.Radius, _targetVolume.Settings.Radius, normTime);
        _currentSettings.MaxRadiusPixels = Mathf.RoundToInt(Mathf.Lerp(_lastVolume.Settings.MaxRadiusPixels, _targetVolume.Settings.MaxRadiusPixels, normTime));
        _currentSettings.Intensity = Mathf.Lerp(_lastVolume.Settings.Intensity, _targetVolume.Settings.Intensity, normTime);
        _currentSettings.BaseColor = Color.Lerp(_lastVolume.Settings.BaseColor, _targetVolume.Settings.BaseColor, normTime);
        _currentSettings.ColorBleedingSaturation = Mathf.Lerp(_lastVolume.Settings.ColorBleedingSaturation, _targetVolume.Settings.ColorBleedingSaturation, normTime);
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
        if (cam != null)
        {
            cam.backgroundColor = _currentSettings.BackgroundColor;

            if (_hbao == null)
            {
                _hbao = cam.GetComponent<HBAO>();
            }

            if (_hbao != null)
            {
                _hbao.GetCurrentPreset();
                _hbao.SetAoRadius(_currentSettings.Radius);
                _hbao.SetAoMaxRadiusPixels(_currentSettings.MaxRadiusPixels);
                _hbao.SetAoIntensity(_currentSettings.Intensity);
                _hbao.SetAoColor(_currentSettings.BaseColor);
                _hbao.SetColorBleedingSaturation(_currentSettings.ColorBleedingSaturation);
            }
        }
    }
}
