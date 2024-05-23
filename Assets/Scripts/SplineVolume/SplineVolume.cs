using Framework;
using System;
using UnityEngine;
using UnityEngine.Rendering.PostProcessing;

public partial class SplineVolume : ScriptableEnum
{
    public SplineVolumeSettings Settings;
    
    [Serializable]
    public class SplineVolumeSettings
    {
        public SplineVolumeSettings(SplineVolumeSettings copy)
        {
            FogColor = copy.FogColor;
            FogStart = copy.FogStart;
            FogDistance = copy.FogDistance;
            BackgroundColor = copy.BackgroundColor;
            PostProcessingProfile = copy.PostProcessingProfile;
            Radius = copy.Radius;
            MaxRadiusPixels = copy.MaxRadiusPixels;
            Intensity = copy.Intensity;
            BaseColor = copy.BaseColor;
            ColorBleedingSaturation = copy.ColorBleedingSaturation;
            TransitionInTime = copy.TransitionInTime;
            
            // YOU HEY YOU YES YOU. DONT FORGET TO ADD COPY HERE FOR NEW PROPERTIES!!!!
        }
        
        [Header("Fog")]
        //TODO: consider using unity fog instead, its the same, but has more integration in editor?
        public Color FogColor = Color.black;
        public float FogStart = 1.5f;
        public float FogDistance = 30;
        public Color BackgroundColor;
    
        [Header("Post")]
        public PostProcessProfile PostProcessingProfile;

        [Header("AO")] 
        [Range(0.25f, 5f)] public float Radius = 1.98f;
        [Range(16, 256)] public int MaxRadiusPixels = 102;
        [Range(0f, 4f)] public float Intensity = 3.53f;
        public Color BaseColor = Color.black;
        [Range(0f, 4f)] public float ColorBleedingSaturation = 4;

        [Header("Transition")]
        public float TransitionInTime = 2f;
    }
}