using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Splines;

[Serializable]
public class TunnelSceneData
{
    public List<LightData> LightData;
    public List<PrefabData> PrefabData;
    public Spline Spline;
}

[Serializable]
public class RadialTransform
{
    [Header("Position Settings")]
    public float SplinePercent;
    public float SplineAngle;
    public float SplineDistanceFrom;
}
    
[Serializable]
public class LightData : RadialTransform
{
    [Header("Light Settings")] 
    public Color LightColor;

    public float LightIntensity;
    public float LightRange;
}

[Serializable]
public class PrefabData : RadialTransform
{
    public GameObject Prefab;
}