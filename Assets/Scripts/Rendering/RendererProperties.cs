using System;
using System.Collections.Generic;
using UnityEngine;

[ExecuteAlways]
public class RendererProperties : MonoBehaviour
{
    [SerializeField]private PropertySettingScope _scope = PropertySettingScope.Self;
    
    [SerializeField] private List<Renderer> _autoRenderers;
    [SerializeField] private List<Renderer> _manualRenderers;

    public List<Renderer> Renderers
    {
        get
        {
            if (_scope == PropertySettingScope.Manual)
            {
                return _manualRenderers;
            }

            return _autoRenderers;
        }
    }

    [Header("Common")]
    [SerializeField] private float _maskClipValue = 0.6f;
    [SerializeField] private int _normalMip = 2;
    [SerializeField] private Color _emissionColor;
    [SerializeField] private float _emissionUsesTunnelWave;
    
    [SerializeField] private bool _useFog;
    [Range(0,1)]
    [SerializeField] private float _fogIntensity;
    
    [SerializeField] private bool _replaceLineColor;
    [Range(0,1)]
    [SerializeField] private float _lineReplaceRange;
    [Range(0,1)]
    [SerializeField] private float _lineReplaceFuzziness;
    [SerializeField] private Color _lineColorToReplace;
    [SerializeField] private Color _lineColorAfterReplace;
    [Range(0,1)]
    [SerializeField] private float _lineColorVsHSVBlend;
    [SerializeField] private Vector3 _lineHSVOffset;
    
    [SerializeField] [HideInInspector]private bool _defaultsSet;
    
    private static readonly int Cutoff = Shader.PropertyToID("_Cutoff");
    private static readonly int NormalMip = Shader.PropertyToID("_NormalMip");
    private static readonly int EmissionColor = Shader.PropertyToID("_EmissionColor");
    private static readonly int FogIntensityCoefficient = Shader.PropertyToID("_FogIntensityCoefficient");
    private static readonly int LineReplaceRange = Shader.PropertyToID("_LineReplaceRange");
    private static readonly int LineReplaceFuzziness = Shader.PropertyToID("_LineReplaceFuzziness");
    private static readonly int PreReplaceLineColor = Shader.PropertyToID("_PreReplaceLineColor");
    private static readonly int PostReplaceLineColor = Shader.PropertyToID("_PostReplaceLineColor");
    private static readonly int LineColorVsHSVBlend = Shader.PropertyToID("_LineColorVsHSVBlend");
    private static readonly int LineHueOffset = Shader.PropertyToID("_LineHueOffset");
    private static readonly int LineSaturationOffset = Shader.PropertyToID("_LineSaturationOffset");
    private static readonly int LineValueOffset = Shader.PropertyToID("_LineValueOffset");
    private static readonly int EmissionUsesTunnelWave = Shader.PropertyToID("_EmissionUsesTunnelWave");

    public const string UseFogKeyword = "_USEFOG_ON";
    public const string ReplaceLineColorKeyword = "_REPLACELINECOLOR_ON";
    
    public PropertySettingScope Scope
    {
        get => _scope;
        set
        {
            bool dirty = _scope == value;
            _scope = value;

            if (dirty)
            {
                EnsureRenderers();
            }
        }
    }

    public void OnValidate()
    {
        EnsureRenderers();
    }

    private void Start()
    {
        EnsureRenderers();
    }

    public void LateUpdate()
    {
        MaterialPropertyBlock props = new MaterialPropertyBlock();
        foreach (Renderer r in Renderers)
        {
            r.GetPropertyBlock(props);
            
            props.SetFloat(Cutoff, _maskClipValue);
            props.SetInt(NormalMip, _normalMip);
            props.SetColor(EmissionColor, _emissionColor);
            props.SetFloat(FogIntensityCoefficient, _fogIntensity);
        
            props.SetFloat(EmissionUsesTunnelWave, _emissionUsesTunnelWave);
            props.SetFloat(LineReplaceRange, _lineReplaceRange);
            props.SetFloat(LineReplaceFuzziness, _lineReplaceFuzziness);
            props.SetColor(PreReplaceLineColor, _lineColorToReplace);
            props.SetColor(PostReplaceLineColor, _lineColorAfterReplace);
            props.SetFloat(LineColorVsHSVBlend, _lineColorVsHSVBlend);
            props.SetFloat(LineHueOffset, _lineHSVOffset.x);
            props.SetFloat(LineSaturationOffset, _lineHSVOffset.y);
            props.SetFloat(LineValueOffset, _lineHSVOffset.z);
            r.SetPropertyBlock(props);
        }
    }

    public void EnsureRenderers()
    {
        _autoRenderers ??= new List<Renderer>();
        _autoRenderers.Clear();
        switch (_scope)
        {
            case PropertySettingScope.SelfAndChildren:
                _autoRenderers.Add(GetSelf());
                _autoRenderers.AddRange(GetChildren());
                break;
            case PropertySettingScope.Self:
                _autoRenderers.Add(GetSelf());
                break;
            case PropertySettingScope.Children:
                _autoRenderers.AddRange(GetChildren());
                break;
            case PropertySettingScope.Manual:
                //Do nothing. Expose renderers
                break;
            default:
                throw new ArgumentOutOfRangeException();
        }

        Renderer GetSelf()
        {
            Renderer renderer;
            if (!TryGetComponent(out renderer))
            {
                gameObject.AddComponent<Renderer>();
            }

            return renderer;
        }
        
        List<Renderer> GetChildren()
        {
            List<Renderer> childrenRenderers = new (GetComponentsInChildren<Renderer>());

            for (int i = childrenRenderers.Count - 1; i >= 0; i--)
            {
                Renderer renderer = childrenRenderers[i];

                //removes self and sub property setters
                if (renderer.GetComponent<RendererProperties>())
                {
                    childrenRenderers.RemoveAt(i);
                }
            }
            return childrenRenderers;
        }
    }
    
    public enum PropertySettingScope
    {
        SelfAndChildren,
        Self,
        Children,
        Manual
    }

    public Material GetPrimaryMaterial()
    {
        Debug.Assert(Renderers.Count > 0);
        return Renderers[0].sharedMaterial;
    }

    public void SetDefaultsFromMaterial(bool force = false)
    {
        if (_defaultsSet && !force)
        {
            return;
        }

        var mat = GetPrimaryMaterial();
        
        _maskClipValue = mat.GetFloat(Cutoff);
        _normalMip = mat.GetInt(NormalMip);
        _emissionColor = mat.GetColor(EmissionColor);
        _fogIntensity = mat.GetFloat(FogIntensityCoefficient);
        _lineReplaceRange = mat.GetFloat(LineReplaceRange);
        _lineReplaceFuzziness = mat.GetFloat(LineReplaceFuzziness);
        _lineColorToReplace = mat.GetColor(PreReplaceLineColor);
        _lineColorAfterReplace = mat.GetColor(PostReplaceLineColor);
        _lineColorVsHSVBlend = mat.GetFloat(LineColorVsHSVBlend);
        _emissionUsesTunnelWave = mat.GetFloat(EmissionUsesTunnelWave);

        _lineHSVOffset = new Vector3(mat.GetFloat(LineHueOffset),
            mat.GetFloat(LineSaturationOffset),
            mat.GetFloat(LineValueOffset));

        _defaultsSet = true;
    }
}
