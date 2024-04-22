using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WorldColors : MonoBehaviour
{
     [SerializeField] private Color _fogColor = Color.white;
    [SerializeField] private float _fogStart = 1.5f;
    [SerializeField] private float _fogDistance = 30;
    void Start()
    {
        Shader.SetGlobalColor("FogColor", _fogColor);
        Shader.SetGlobalFloat("FogStart", _fogStart);
        Shader.SetGlobalFloat("FogDistance", _fogDistance);
    }

    //  
    void Update()
    {
        Shader.SetGlobalColor("FogColor", _fogColor);
        Shader.SetGlobalFloat("FogStart", _fogStart);
        Shader.SetGlobalFloat("FogDistance", _fogDistance);

    }
}
