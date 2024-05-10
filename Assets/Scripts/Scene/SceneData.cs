#region Usings
using UnityEngine;
using MathBad;
using UnityEngine.Rendering.PostProcessing;
#endregion

public class SceneData : MonoBehaviour
{
    public string title;
    public string description;

    [Header("Audio")]
    public EffectSoundBank sceneIntro;
    public AudioClip sceneMusic;

    [Header("Post Processing")]
    public PostProcessProfile postProcessProfile;
}
