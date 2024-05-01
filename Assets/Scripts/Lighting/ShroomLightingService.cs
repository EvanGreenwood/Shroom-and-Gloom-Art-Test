using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteAlways]
[DefaultExecutionOrder(-100)]
public class ShroomLightingService : MonoService
{
    private List<ShroomLight> _lights = new List<ShroomLight>();
    private static readonly int ShroomLightPositionsGlobalId = Shader.PropertyToID("ShroomLightPositions");
    private static readonly int ShroomLightInnerRadiusAndIntensityGlobalId = Shader.PropertyToID("ShroomLightInnerRadiusAndIntensity");
    private static readonly int ShroomLightColorsGlobalId = Shader.PropertyToID("ShroomLightColors");

    public const int MAX_LIGHT_COUNT = 10;
    
    public void RegisterLight(ShroomLight light)
    {
        _lights.Add(light);
    }
    
    public void UnRegisterLight(ShroomLight light)
    {
        _lights.Remove(light);
    }

    public void LateUpdate()
    {
        List<Vector4> positions = new List<Vector4>();
        List<Vector4> radiusAndIntensity = new List<Vector4>();
        List<Vector4> colors = new List<Vector4>();

        if (_lights.Count == 0)
        {
            return;
        }

        for (int i = 0; i < _lights.Count && i < MAX_LIGHT_COUNT; i++)
        {
            ShroomLight shroomLight = _lights[i];

            Vector4 pos = new Vector4(shroomLight.transform.position.x,
                shroomLight.transform.position.y,
                shroomLight.transform.position.z,
                1);
            //Debug.Log(pos);
            positions.Add(pos);

            //Encode values in vec to save shader keywords
            radiusAndIntensity.Add(new Vector4(shroomLight.InnerRadius, shroomLight.OuterRadius, shroomLight.Intensity, 0));
            colors.Add(new Vector4(shroomLight.Color.r, shroomLight.Color.g, shroomLight.Color.b, shroomLight.Color.a));
        }

        //Set light parameters
        Shader.SetGlobalVectorArray(ShroomLightPositionsGlobalId, positions);
        Shader.SetGlobalVectorArray(ShroomLightInnerRadiusAndIntensityGlobalId, radiusAndIntensity);
        Shader.SetGlobalVectorArray(ShroomLightColorsGlobalId, colors);
    }
}
