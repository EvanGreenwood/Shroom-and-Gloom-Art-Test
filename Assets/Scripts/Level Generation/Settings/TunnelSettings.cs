#if UNITY_EDITOR
using UnityEditor;
#endif

using Framework;
using JetBrains.Annotations;
using NaughtyAttributes;
using System;
using UnityEngine;
using UnityEngine.Serialization;

public partial class TunnelSettings : ScriptableEnum
{
    public Gradient ColorGradient;

    [Header("Size")]
    public float BaseTunnelWidth = 1f;
    public float LumpyWidth = 0.5f;
    public float ClearingWidth = 1f;
    public float ClearingDepth = 2.33f;
    
    [Header("Particles")]
    public ParticleSystem Particles;
    [ShowIf("UsingParticles")] public float ParticleSpawnDistance = 1f;

    [Header("Walls")]
    [SerializeField] TunnelWallData[] WallElements;
    
    public SpriteRenderer FlatWallPrefab;
    [ShowIf("UsingWallFlats")] public float FlatWallSpacing = 1;
    [ShowIf("UsingWallFlats")] public float FlatWallOffset = 1;

    [Header("Surrounds")]
    [SerializeField] TunnelSurroundData[] SurroundElements;

    [Header("Floor")]
    [SerializeField] TunnelFloorData[] FloorElements;
    public SpriteRenderer FlatFloorPrefab;
    [ShowIf("UsingFloorFlats")] public float FlatFloorSpacing = 1f;
    [ShowIf("UsingFloorFlats")] public float FlatFloorOffset = -1;

    [Header("Ceiling")]
    [SerializeField] TunnelCeilingData[] CeilingElements;
    public SpriteRenderer FlatCeilingPrefab;
    [ShowIf("UsingCeilingFlats")] public float FlatCeilingSpacing = 1;
    [ShowIf("UsingCeilingFlats")] public float FlatCeilingOffset = 1;
    
    public void CopyValues(TunnelGenerator t)
    {
        //Do a deep copy of the tunnel generator values into this TunnelSettings.
        //TODO: This is prone to bugs as data changes so the objective would be to move away from embedded data as fast as possible, and then remove this.
        Debug.Assert(!t.UseSOData);

        BaseTunnelWidth = t.Old_BaseTunnelWidth;
        LumpyWidth = t.Old_LumpyWidth;
        ClearingWidth = t.Old_ClearingWidth;
        ClearingDepth = t.Old_ClearingDepth;
        
        Particles = t.Old_Particles;
        ParticleSpawnDistance = t.Old_ParticleSpawnDistance;

        WallElements = new TunnelWallData[t.Old_WallElements.Length];
        Array.Copy(t.Old_WallElements, WallElements, t.Old_WallElements.Length);
        for (int i = 0; i < WallElements.Length; i++) { WallElements[i] = new TunnelWallData(WallElements[i]); }
        FlatWallPrefab = t.Old_FlatWallPrefab;
        FlatWallSpacing = t.Old_FlatWallSpacing;
        FlatWallOffset = t.Old_FlatWallOffset;
        
        SurroundElements = new TunnelSurroundData[t.Old_SurroundElements.Length];
        Array.Copy(t.Old_SurroundElements, SurroundElements, t.Old_SurroundElements.Length);
        for (int i = 0; i < SurroundElements.Length; i++) { SurroundElements[i] = new TunnelSurroundData(SurroundElements[i]); }

        FloorElements = new TunnelFloorData[t.Old_FloorElements.Length];
        Array.Copy(t.Old_FloorElements, FloorElements, t.Old_FloorElements.Length);
        for (int i = 0; i < FloorElements.Length; i++) { FloorElements[i] = new TunnelFloorData(FloorElements[i]); }
        FlatFloorPrefab = t.Old_FlatFloorPrefab;
        FlatFloorSpacing = t.Old_FlatFloorSpacing;
        FlatFloorOffset = t.Old_FlatFloorOffset;
        
        CeilingElements = new TunnelCeilingData[t.Old_CeilingElements.Length];
        Array.Copy(t.Old_CeilingElements, CeilingElements, t.Old_CeilingElements.Length);
        for (int i = 0; i < CeilingElements.Length; i++) { CeilingElements[i] = new TunnelCeilingData(CeilingElements[i]); }
        FlatCeilingPrefab = t.Old_FlatCeilingPrefab;
        FlatCeilingSpacing = t.Old_FlatCeilingSpacing;
        FlatCeilingOffset = t.Old_FlatCeilingOffset;
        
        Debug.Log($"<color=green><b> Copied embedded settings into {name}. Now set UseSOData to true on {t.gameObject.name}</b></color>");
        
        #if UNITY_EDITOR
        AssetDatabase.SaveAssets();
        #endif
    }
    
    [UsedImplicitly]
    private bool UsingWallFlats()
    {
        return FlatWallPrefab != null;
    }
    [UsedImplicitly]
    private bool UsingFloorFlats()
    {
        return FlatFloorPrefab != null;
    }
    [UsedImplicitly]
    private bool UsingCeilingFlats()
    {
        return FlatCeilingPrefab != null;
    }
    
    [UsedImplicitly]
    private bool UsingParticles()
    {
        return Particles != null;
    }
}

#region Tunnel Data
    public enum TunnelSide
    {
        Left,
        Right
    }

    [Serializable]
    public class TunnelWallData
    {
        public TunnelWallData(TunnelWallData rhs)
        {
            Name = rhs.Name;
            Disabled = rhs.Disabled;
            WallPrefabs = rhs.WallPrefabs;
            ZOffset = rhs.ZOffset;
            Spacing = rhs.Spacing;
            Width = rhs.Width;
            HeightOffset = rhs.HeightOffset;
            VerticalLoopLength = rhs.VerticalLoopLength;
            VerticalLoopIncrement = rhs.VerticalLoopIncrement;
            MinMaxAngle = rhs.MinMaxAngle;
            MinMaxPosition = rhs.MinMaxPosition;
            RandomRotation = rhs.RandomRotation;
            RandomFlipY = rhs.RandomFlipY;
        }
        
        [FormerlySerializedAs("name")] public string Name;
        [FormerlySerializedAs("disabled")] public bool Disabled;
        [FormerlySerializedAs("wallPrefabs")] public SpriteRenderer[] WallPrefabs;
        [FormerlySerializedAs("zOffset")] public float ZOffset = 0;
        [FormerlySerializedAs("spacing")] public float Spacing = 0.5f;
        [FormerlySerializedAs("width")] public float Width = 1.5f;
        [FormerlySerializedAs("heightOffset")] public FloatRange HeightOffset = new FloatRange(0f, 0.1f);
        [FormerlySerializedAs("verticalLoopLength")] public int VerticalLoopLength = 4;
        [FormerlySerializedAs("verticalLoopIncrement")] public int VerticalLoopIncrement = 2;
        [FormerlySerializedAs("minMaxAngle")] public FloatRange MinMaxAngle = new FloatRange(-20, 20);
        [FormerlySerializedAs("minMaxPosition")] public FloatRange MinMaxPosition = new FloatRange(0.6f, 2.6f);
        [FormerlySerializedAs("randomRotation")] public float RandomRotation = 5f;
        [FormerlySerializedAs("randomFlipY")] public bool RandomFlipY = true;
    }
    //
    [Serializable]
    public class TunnelSurroundData
    {
        public enum SurroundPattern
        {
            Solid,
            Staggered,
            Spiral,
            SpiralStaggered,
            StaggeredOffset,
        }
        
        public TunnelSurroundData(TunnelSurroundData rhs)
        {
            Name = rhs.Name;
            Disabled = rhs.Disabled;
            SurroundPrefabs = rhs.SurroundPrefabs;
            ZOffset = rhs.ZOffset;
            Spacing = rhs.Spacing;
            RadiusOffset = rhs.RadiusOffset;
            CenterHeight = rhs.CenterHeight;
            Pattern = rhs.Pattern;
            AngleRange = rhs.AngleRange;
            AngleIncrement = rhs.AngleIncrement;
            RandomRotation = rhs.RandomRotation;
            RandomFlipY = rhs.RandomFlipY;
            RandomFlipX = rhs.RandomFlipX;
        }
        
        [FormerlySerializedAs("name")] public string Name;
        [FormerlySerializedAs("disabled")] public bool Disabled;
        [FormerlySerializedAs("surroundPrefabs")] public SpriteRenderer[] SurroundPrefabs;
        [FormerlySerializedAs("zOffset")] public float ZOffset = 0;
        [FormerlySerializedAs("spacing")] public float Spacing = 0.5f;
        [FormerlySerializedAs("radiusOffset")] public float RadiusOffset = 1;
        [FormerlySerializedAs("centerHeight")] public float CenterHeight = 1;
        [FormerlySerializedAs("pattern")] public SurroundPattern Pattern = SurroundPattern.Staggered;
        [FormerlySerializedAs("angleRange")] public FloatRange AngleRange = new FloatRange(44, 316);
        [FormerlySerializedAs("angleIncrement")] public float AngleIncrement = 45;
        [FormerlySerializedAs("randomRotation")] public float RandomRotation = 5f;
        [FormerlySerializedAs("randomFlipY")] public bool RandomFlipY = true;
        [FormerlySerializedAs("randomFlipX")] public bool RandomFlipX = false;
    }
    //
    [Serializable]
    public class TunnelFloorData
    {
        
        public TunnelFloorData(TunnelFloorData rhs)
        {
            Name = rhs.Name;
            Disabled = rhs.Disabled;
            FloorPrefabs = rhs.FloorPrefabs;
            ZOffset = rhs.ZOffset;
            Spacing = rhs.Spacing;
            RandomXPosition = rhs.RandomXPosition;
            MinMaxAngle = rhs.MinMaxAngle;
            XRotation = rhs.XRotation;
            MinMaxPosition = rhs.MinMaxPosition;
            RandomFlipX = rhs.RandomFlipX;
        }
        
        [FormerlySerializedAs("name")] public string Name;
        [FormerlySerializedAs("disabled")] public bool Disabled;
        [FormerlySerializedAs("floorPrefabs")] public SpriteRenderer[] FloorPrefabs;
        [FormerlySerializedAs("zOffset")] public float ZOffset = 0;
        [FormerlySerializedAs("spacing")] public float Spacing = 0.5f;
        [FormerlySerializedAs("randomXPosition")] public float RandomXPosition = 1.5f;
        [FormerlySerializedAs("minMaxAngle")] public FloatRange MinMaxAngle = new FloatRange(-1, 1);
        [FormerlySerializedAs("xRotation")] public float XRotation = 0;
        [FormerlySerializedAs("minMaxPosition")] public FloatRange MinMaxPosition = new FloatRange(-0.05f, 0.05f);
        [FormerlySerializedAs("randomFlipX")] public bool RandomFlipX = true;
    }
    //
    [Serializable]
    public class TunnelCeilingData
    {
        public TunnelCeilingData(TunnelCeilingData rhs)
        {
            Name = rhs.Name;
            Disabled = rhs.Disabled;
            CeilingPrefabs = rhs.CeilingPrefabs;
            Height = rhs.Height;
            ZOffset = rhs.ZOffset;
            Spacing = rhs.Spacing;
            RandomXPosition = rhs.RandomXPosition;
            MinMaxAngle = rhs.MinMaxAngle;
            XRotation = rhs.XRotation;
            MinMaxPosition = rhs.MinMaxPosition;
            RandomFlipX = rhs.RandomFlipX;
        }
        
        [FormerlySerializedAs("name")] public string Name;
        [FormerlySerializedAs("disabled")] public bool Disabled;
        [FormerlySerializedAs("ceilingPrefabs")] public SpriteRenderer[] CeilingPrefabs;
        [FormerlySerializedAs("height")] public float Height = 3;
        [FormerlySerializedAs("zOffset")] public float ZOffset = 0;
        [FormerlySerializedAs("spacing")] public float Spacing = 0.5f;
        [FormerlySerializedAs("randomXPosition")] public float RandomXPosition = 1.5f;
        [FormerlySerializedAs("minMaxAngle")] public FloatRange MinMaxAngle = new FloatRange(-1, 1);
        [FormerlySerializedAs("xRotation")] public float XRotation = 0;
        [FormerlySerializedAs("minMaxPosition")] public FloatRange MinMaxPosition = new FloatRange(-0.05f, 0.05f);
        [FormerlySerializedAs("randomFlipX")] public bool RandomFlipX = true;
    }
#endregion
