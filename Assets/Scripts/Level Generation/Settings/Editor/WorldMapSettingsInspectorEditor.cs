using System;
using System.Collections.Generic;
using System.IO;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(WorldMapSettings))]
public class WorldMapSettingsInspectorEditor : Editor
{
    private WorldMapSettings _worldMapSettings;
    private SerializedProperty _paths;
    private SerializedProperty _texDirty;
    private SerializedProperty _lastGeneratedGraphTexPath;
    private bool _busyGenerating;
    private bool _reloadQueued;

    private void OnEnable()
    {
        _worldMapSettings = (WorldMapSettings)target;
        _paths = serializedObject.FindProperty("Paths");
        _texDirty = serializedObject.FindProperty("_texDirty");
        _lastGeneratedGraphTexPath = serializedObject.FindProperty("_lastGeneratedGraphTexPath");

        _worldMapSettings.GraphPreview.TexDirty = true;
        AssemblyReloadEvents.afterAssemblyReload += () => 
        {
            _reloadQueued = true;
            _busyGenerating = false;
        };
    }

    public override void OnInspectorGUI()
    {
        // Draw default inspector for all serialized fields
        DrawDefaultInspector();

        if (GUILayout.Button("Apply"))
        {
            _worldMapSettings.TryGeneratePathsData(true);
        }
        
        // Load and display the graph texture
        if (!string.IsNullOrEmpty(_worldMapSettings.GraphPreview.TempGraphTexturePath))
        {
            if (_worldMapSettings.GraphPreview.TexDirty || _worldMapSettings.GraphPreview.GraphTex == null)
            {
                string graphName = _worldMapSettings.name; // Assuming the ScriptableObject name is used
                string path = GraphPreviewGenerator.GetPath(graphName);
                if (GraphPreviewGenerator.FileExists(graphName))
                {
                    byte[] fileData = File.ReadAllBytes(path);
                    Texture2D graphTex = new Texture2D(2, 2);
                    graphTex.LoadImage(fileData);

                    _worldMapSettings.GraphPreview.GraphTex = graphTex;
                    _worldMapSettings.GraphPreview.TexDirty = false;
                }
                else
                {
                    GUIPollRegenerate(path);
                }
            }
            else
            {
                //Texture is valid. Render it
                int textureHeight = _worldMapSettings.GraphPreview.GraphTex.height;
                
                Rect texRect = EditorGUILayout.GetControlRect(GUILayout.Height(textureHeight));
                EditorGUI.DrawTextureTransparent(texRect, _worldMapSettings.GraphPreview.GraphTex, ScaleMode.ScaleToFit);
            }
        }
        else
        {
            GUIPollRegenerate();
        }

        // Apply changes
        serializedObject.ApplyModifiedProperties();
    }

    private void GUIPollRegenerate(string path = null)
    {
        if (!_busyGenerating && _reloadQueued)
        {
            _busyGenerating = true;
            RequestGraphToBeGeneratedAsync();
        }
        else
        {
            if (path != null)
            {
                EditorGUILayout.HelpBox($"Graph texture not found at: \n{path}" +
                    $"\nCreating a new one{StringOps.LoadingDots()}", MessageType.Info);
            }
            else
            {
                EditorGUILayout.HelpBox($"\nBusy generating graph{StringOps.LoadingDots()}", MessageType.Info);
            }
            Repaint();
        }
    }
    
    private void RequestGraphToBeGeneratedAsync()
    {
        List<(string, string)> connections = new List<(string, string)>();
        List<(string, List<(string, string)>)> subConnections = new List<(string, List<(string, string)>)>();
        foreach (var path in _worldMapSettings.Paths)
        {
            string left = ScriptGenerationUtility.CreateVarName(path.Name);

            string right = string.Empty;
            
            foreach (Enum value in Enum.GetValues(typeof(TunnelPath)))
            {
                if ((TunnelPath)value == TunnelPath.None)
                {
                    continue;
                }
                if (path.ConnectingPaths.HasFlag(value))
                {
                    right = value.ToString();
                    connections.Add((left, right));
                }
            }

            if (path.Tunnels.Length > 0)
            {
                List<(string, string)> sub = new List<(string, string)>();

                if (path.Tunnels.Length == 1)
                {
                    string t1 = ScriptGenerationUtility.CreateVarName(path.Tunnels[0].name);
                    string t2 = ScriptGenerationUtility.CreateVarName("[*]");
                    sub.Add((t1, t2));
                }
                else
                {
                    for (int i = 0; i < path.Tunnels.Length-1; i++)
                    {
                        string t1 = ScriptGenerationUtility.CreateVarName(path.Tunnels[i].name);
                        string t2 = ScriptGenerationUtility.CreateVarName(path.Tunnels[i+1].name);
                        sub.Add((t1, t2));
                    }
                }
                
                subConnections.Add((left, sub));
            }
        }

        GraphPreviewGenerator.RequestGraphAsync((bool success, string texPath, Texture2D tex) =>
        {
            if (!success)
            {
                _busyGenerating = false;
                return;
            }
            
            _worldMapSettings.GraphPreview.TempGraphTexturePath = texPath;
            _worldMapSettings.GraphPreview.GraphTex = tex; //means we dont need to load tex
            _worldMapSettings.GraphPreview.TexDirty = false;

            _busyGenerating = false;
            _reloadQueued = false;
        }, _worldMapSettings.name, connections, subConnections);
    }
}