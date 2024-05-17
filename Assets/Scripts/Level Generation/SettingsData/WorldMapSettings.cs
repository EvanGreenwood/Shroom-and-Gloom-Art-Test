using JetBrains.Annotations;
using NaughtyAttributes;
using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[CreateAssetMenu(menuName="S&G/Data/WorldMapSettings")]
public class WorldMapSettings : ScriptableObject
{
    [Serializable]
    public class LevelPath
    {
        public string Name;
     
        public TunnelPath ConnectingPaths;
        public TunnelSettings[] Tunnels;

        public int ConnectingPathCount
        {
            get
            {
                int count = 0;
                foreach (Enum value in Enum.GetValues(typeof(TunnelPath)))
                {
                    if ((TunnelPath)value == TunnelPath.None)
                    {
                        continue;
                    }
            
                    if (ConnectingPaths.HasFlag(value))
                    {
                        count++;
                    }
                }
                return count;
            }
        }
    }

    public List<LevelPath> Paths;

    [Serializable]
    public class GraphPreviewData
    {
        [HideInInspector] public bool TexDirty;
        [HideInInspector] public string TempGraphTexturePath;
        [HideInInspector] public Texture2D GraphTex;
    }

    [HideInInspector] public GraphPreviewData GraphPreview;
    
    public bool TryGeneratePathsData(bool applyAsset)
    {
        GraphPreview.TexDirty = true;
        GraphPreview.TempGraphTexturePath = null;
        List<string> lines = new List<string>
        {
            "using System;",
            "\n",
            "// --------- GENERATED ---------",
            "// ---- Changes will be lost ---",
            "\n",
            "[Flags]",
            "public enum TunnelPath",
            "{"
        };
        int flagValue = 1;
        lines.Add($"\tNone = 0,");
        foreach (var path in Paths)
        {
            string pathName = ScriptGenerationUtility.CreateVarName(path.Name);
            lines.Add($"\t{ pathName} = {flagValue},");
            flagValue *= 2; //power of 2 flags
        }
        lines.Add("}");
        
        ScriptGenerationUtility.Generate("Assets/Generated", "TunnelPath", lines, applyAsset);

        return true;
    }
}
