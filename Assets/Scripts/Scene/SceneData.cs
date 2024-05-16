#region Usings
using UnityEngine;
using MathBad;
using UnityEngine.Rendering.PostProcessing;
#endregion

public class SceneData : MonoBehaviour
{
    public string title;
    public string description;
    public Color titleColor = RGB.darkGrey;

    [Header("Audio")]
    public EffectSoundBank sceneIntro;
    public AudioClip sceneMusic;
}
