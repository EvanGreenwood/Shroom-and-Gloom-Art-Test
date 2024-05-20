using Framework;
using NUnit.Framework;
using UnityEngine;
using UnityEditor;
using System.Collections.Generic;
using System.Text;
using UnityEngine.Rendering;

[CustomEditor(typeof(RendererProperties))]
public class RenderPropertiesInspector : Editor
{
    private RendererProperties _rendererProperties;
    
    private SerializedProperty _scope;
    private SerializedProperty _manualRenderers;
    
    private SerializedProperty _maskClipValue;
    private SerializedProperty _normalMip;
    private SerializedProperty _emissionColor;
    private SerializedProperty _fogIntensity;
    private SerializedProperty _lineReplaceRange;
    private SerializedProperty _lineReplaceFuzziness;
    private SerializedProperty _lineColorToReplace;
    private SerializedProperty _lineColorAfterReplace;
    private SerializedProperty _lineColorVsHSVBlend;
    private SerializedProperty _lineHSVOffset;
    private SerializedProperty _emissionUsesTunnelWave;

    private void OnEnable()
    {
        _rendererProperties = (RendererProperties)target;
        
        _scope = serializedObject.FindProperty("_scope");

        _rendererProperties.EnsureRenderers();
        
        _rendererProperties.SetDefaultsFromMaterial();
        
        _manualRenderers = serializedObject.FindProperty("_manualRenderers");
        _maskClipValue = serializedObject.FindProperty("_maskClipValue");
        _normalMip = serializedObject.FindProperty("_normalMip");
        _emissionColor = serializedObject.FindProperty("_emissionColor");
        
        _fogIntensity = serializedObject.FindProperty("_fogIntensity");
        
        _lineReplaceRange = serializedObject.FindProperty("_lineReplaceRange");
        _lineReplaceFuzziness = serializedObject.FindProperty("_lineReplaceFuzziness");
        _lineColorToReplace = serializedObject.FindProperty("_lineColorToReplace");
        _lineColorAfterReplace = serializedObject.FindProperty("_lineColorAfterReplace");
        _lineColorVsHSVBlend = serializedObject.FindProperty("_lineColorVsHSVBlend");
        _lineHSVOffset = serializedObject.FindProperty("_lineHSVOffset");
        
        _emissionUsesTunnelWave = serializedObject.FindProperty("_emissionUsesTunnelWave");
    }

    public override void OnInspectorGUI()
    {
        serializedObject.Update();

        EditorGUI.BeginChangeCheck();
        EditorGUILayout.PropertyField(_scope);
        if (EditorGUI.EndChangeCheck())
        {
            _rendererProperties.EnsureRenderers();
        }

        if (((RendererProperties.PropertySettingScope)_scope.enumValueIndex) ==
            RendererProperties.PropertySettingScope.Manual)
        {
            EditorGUILayout.PropertyField(_manualRenderers);
        }
        else
        {
            GUI.enabled = false;
            EditorGUILayout.LabelField("Controlling Renderers:");
            StringBuilder rendererList = new StringBuilder();
            foreach (var renderer in _rendererProperties.Renderers)
            {
                string details = $"{renderer.gameObject.name} ({renderer.GetType().Name}({renderer.sharedMaterial.name}))";
                rendererList.AppendLine(details);
            }
        
            EditorGUILayout.TextArea(rendererList.ToString(), new GUIStyle(EditorStyles.textArea)
            {
                wordWrap = true
            });
        
            GUI.enabled = true;
        }
        
        EditorGUILayout.Space();
        
        EditorGUILayout.PropertyField(_maskClipValue);
        EditorGUILayout.PropertyField(_normalMip);
        EditorGUILayout.PropertyField(_emissionColor);
        
        _emissionUsesTunnelWave.floatValue = EditorGUILayout.Toggle("Emission Tunnel Wave", _emissionUsesTunnelWave.floatValue > 0.1)?1:0;
        
        if (_rendererProperties.GetPrimaryMaterial().IsKeywordEnabled(RendererProperties.UseFogKeyword))
        {
            EditorGUILayout.PropertyField(_fogIntensity);
        }
        
        if (_rendererProperties.GetPrimaryMaterial().IsKeywordEnabled(RendererProperties.ReplaceLineColorKeyword))
        {
            EditorGUILayout.PropertyField(_lineColorVsHSVBlend);
            
            EditorGUILayout.PropertyField(_lineReplaceRange);
            EditorGUILayout.PropertyField(_lineReplaceFuzziness);
            EditorGUILayout.PropertyField(_lineColorToReplace);
            EditorGUILayout.PropertyField(_lineColorAfterReplace);
           
            EditorGUILayout.PropertyField(_lineHSVOffset);
        }

        if (GUILayout.Button("Set Material Defaults"))
        {
            _rendererProperties.SetDefaultsFromMaterial(true);
        }
        
        EditorGUILayout.HelpBox(new GUIContent("Changing keywords will create a new material, which may affect batching and performance."));
        serializedObject.ApplyModifiedProperties();
    }
}
