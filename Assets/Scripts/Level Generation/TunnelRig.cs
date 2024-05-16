#region Usings
using System.Collections.Generic;
using UnityEngine;
using MathBad;
using NaughtyAttributes;
using Unity.Mathematics;
using UnityEditor;
using UnityEngine.Splines;
#endregion

[RequireComponent(typeof(SplineContainer))]
public class TunnelRig : MonoBehaviour
{
    [SerializeField] SplineContainer _splineContainer;
    [SerializeField] TunnelLight _tunneLightPrefab;

    [SerializeField, ReadOnly]
    List<TunnelLight> _tunnelLights = new List<TunnelLight>();

    public Spline spline => _splineContainer.Spline;

    public void AddLight()
    {
        _splineContainer.Spline.Evaluate(0.0f, out float3 pos, out float3 tangent, out float3 up);
        TunnelLight tunnelLight = Instantiate(_tunneLightPrefab, pos, Quaternion.LookRotation(tangent, up));

        tunnelLight.name = $"TunnelLight_{_tunnelLights.Count}";
        tunnelLight.Init(this);

        _tunnelLights.Add(tunnelLight);
        transform.TakeChild(tunnelLight);

        Selection.activeGameObject = tunnelLight.gameObject;
    }
    
    public void RemoveLight(TunnelLight tunnelLight)
    {
        _tunnelLights.Remove(tunnelLight);
    }

    public void AddColorNode() { }
}
