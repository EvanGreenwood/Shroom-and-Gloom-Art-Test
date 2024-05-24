#region Usings
using System.Collections.Generic;
using UnityEngine;
using MathBad;
using NaughtyAttributes;
using Unity.Mathematics;
#if UNITY_EDITOR
using UnityEditor;
#endif
using UnityEngine.Splines;
#endregion

[RequireComponent(typeof(SplineContainer))]
public class TunnelRig : MonoBehaviour
{
    [SerializeField] SplineContainer _splineContainer;
    [SerializeField] TunnelLight _tunneLightPrefab;

    [SerializeField, ReadOnly] private Transform _root;
    
    [SerializeField, ReadOnly]
    List<TunnelLight> _tunnelLights = new List<TunnelLight>();

    public Spline spline => _splineContainer.Spline;

    private void OnValidate()
    {
        if (!_splineContainer)
        {
            _splineContainer = GetComponent<SplineContainer>();
        }

        for (int i = _tunnelLights.Count - 1; i >= 0; i--)
        {
            if (_tunnelLights[i] == null)
            {
                _tunnelLights.RemoveAt(i);
            }
        }
    }

    public void InitFromTunnelData(TunnelSceneData data)
    {
        GameObject template = new GameObject("TunnelDataLightTemplate");
        Light tLight = template.AddComponent<Light>();
        tLight.type = LightType.Point;
        foreach (var lightData in data.LightData)
        {
            tLight.color = lightData.LightColor;
            tLight.intensity = lightData.LightIntensity;
            tLight.range = lightData.LightRange;
            
            TunnelLight light = AddLight(tLight);
            light.Distance = lightData.SplineDistanceFrom;
            light.SplinePercent = lightData.SplinePercent;
            light.Angle = lightData.SplineAngle;
            
            light.Refresh();
        }
    }

    public void SetTunnelLightFromTransform(TunnelLight light, Transform trans)
    {
        Vector3 localPoint = transform.InverseTransformPoint(trans.position);
        SplineUtility.GetNearestPoint(_splineContainer.Spline, localPoint, out float3 localNearest, out float t);
        Vector3 worldNearest = transform.TransformPoint(localNearest);
        Vector3 tangent = _splineContainer.EvaluateTangent(t);
        Vector3 up = _splineContainer.EvaluateUpVector(t);
        light.SplinePercent = t;
        light.Distance = Vector3.Distance(localPoint, localNearest);
        
        Vector3 toPos = (trans.position - worldNearest).normalized;
        
        Debug.DrawLine(worldNearest, worldNearest + toPos, Color.yellow, 60);
        Debug.DrawLine(worldNearest, worldNearest + tangent, Color.blue, 60);
        Debug.DrawLine(worldNearest, worldNearest + Vector3.up, Color.green, 60);
        
        Quaternion upRot = Quaternion.LookRotation(tangent, up);
        Quaternion diffRot = Quaternion.LookRotation(tangent, toPos);
        light.Angle = -Quaternion.Angle(upRot, diffRot);
    }

    public void AssureRoot()
    {
        if (_root == null)
        {
            _root = transform.Find("TunnelLights");
            if (_root == null)
            {
                _root = new GameObject("TunnelLights").transform;
                _root.SetParent(transform, false);
            }
        }
    }
    
    public TunnelLight AddLight(Light existing = null)
    {
        AssureRoot();
        
        _splineContainer.Spline.Evaluate(0.0f, out float3 pos, out float3 tangent, out float3 up);

        TunnelLight tunnelLight = null;

        if (Application.isPlaying)
        {
            tunnelLight = Instantiate(_tunneLightPrefab, pos, Quaternion.LookRotation(tangent, up));
        }
        else
        {
            #if UNITY_EDITOR
                        tunnelLight = (TunnelLight)PrefabUtility.InstantiatePrefab(_tunneLightPrefab);
                        tunnelLight.transform.position = pos;
                        tunnelLight.transform.rotation = Quaternion.LookRotation(tangent, up);
            #endif
        }

        if (existing)
        {
            tunnelLight.transform.position = existing.transform.position;
            tunnelLight.transform.rotation = existing.transform.rotation;

            tunnelLight.Light.color = existing.color;
            tunnelLight.Light.intensity = existing.intensity;
            tunnelLight.Light.range = existing.range;
        }
        
        tunnelLight.transform.SetParent(_root);
        
        tunnelLight.name = $"TunnelLight_{_tunnelLights.Count}";
        tunnelLight.Init(this);

        _tunnelLights.Add(tunnelLight);
        _root.TakeChild(tunnelLight);
#if UNITY_EDITOR
        Selection.activeGameObject = tunnelLight.gameObject;
#endif
        return tunnelLight;
    }
    
    public void RemoveLight(TunnelLight tunnelLight)
    {
        _tunnelLights.Remove(tunnelLight);
    }

    public void AddColorNode() { }

    public void CollectSceneLights()
    {
        List<Light> lights = new List<Light>(FindObjectsOfType<Light>());

        foreach (Light light in lights)
        {
            if (light.TryGetComponent<TunnelLight>(out _))
            {
                continue;
            }

            if (light.gameObject.CompareTag("Player"))
            {
                continue;
            }
            
            AddLight(light);
        }
    }

    //If for whatever reason the rig does not have the light in its list
    public void RegisterLight(TunnelLight tunnelLight)
    {
        AssureRoot();
        _tunnelLights.Add(tunnelLight);
        _root.TakeChild(tunnelLight);
        tunnelLight.Init(this); //redundant but just in case
    }
}
