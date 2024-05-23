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
        [Header("Fog")]
        //TODO: consider using unity fog instead, its the same, but has more integration in editor?
        public Color FogColor = Color.black;
        public float FogStart = 1.5f;
        public float FogDistance = 30;
        public Color BackgroundColor;
    
        //[Header("Shadows")]
        //[SerializeField] private Color _shadowsColor = Color.blue;
    
        [Header("Post")]
        public PostProcessProfile PostProcessingProfile;

        [Header("AO")] private Color BaseColor;

        [Header("Transition")]
        public float TransitionInTime = 2f;
    }
}