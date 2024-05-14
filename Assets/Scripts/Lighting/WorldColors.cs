using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WorldColors : MonoBehaviour
{
    [Header("Fog")]
    [SerializeField] private Color _fogColor = Color.white;
    [SerializeField] private float _fogStart = 1.5f;
    [SerializeField] private float _fogDistance = 30;
    
    [Header("Shadows")]
    [SerializeField] private Color _shadowsColor = Color.blue;
    
    [Header("Ramp")]
    [SerializeField] private Texture2D _rampTexture;
    
    [Header("Highlight")]
    [SerializeField] private float _lightingHighlightThreshold;
    
    void Start()
    {
        SetColors();
    }

    //  
    void Update()
    {
        SetColors();
    }

    private void SetColors()
    {
        Shader.SetGlobalColor("FogColor", _fogColor);
        Shader.SetGlobalFloat("FogStart", _fogStart);
        Shader.SetGlobalFloat("FogDistance", _fogDistance);
        
        Shader.SetGlobalColor("ShadowsColor", _shadowsColor);
        
        Shader.SetGlobalTexture("_Ramp", _rampTexture);
        Shader.SetGlobalFloat("_HighlightValueThreshold", _lightingHighlightThreshold);
        if (Camera.main)
        {
            Camera.main.backgroundColor = _fogColor;
        }
    }

    private void OnValidate()
    {
        SetColors();
    }
}
