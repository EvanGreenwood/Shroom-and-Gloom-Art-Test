using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WorldColors : MonoBehaviour
{
     [SerializeField] private Color _fogColor = Color.white;
    [SerializeField] private float _fogStart = 1.5f;
    [SerializeField] private float _fogDistance = 30;
    [SerializeField] private Color _shadowsColor = Color.blue;
    void Start()
    {
        Shader.SetGlobalColor("FogColor", _fogColor);
        Shader.SetGlobalFloat("FogStart", _fogStart);
        Shader.SetGlobalFloat("FogDistance", _fogDistance);
        Shader.SetGlobalColor("ShadowsColor", _shadowsColor);
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
    }

    private void OnValidate()
    {
        SetColors();
    }
}
