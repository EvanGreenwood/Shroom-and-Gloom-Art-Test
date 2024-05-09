#region Usings
using UnityEngine;
using MathBad;
using UnityEngine.Rendering.PostProcessing;
#endregion

[MenuCreate("SceneData", 0)]
public class SceneData : ScriptableObject
{
    public string title;
    public string description;

    [Header("Audio")]
    public EffectSoundBank sceneIntro;
    public EffectSoundBank sceneMusic;

    [Header("Post Processing")]
    public PostProcessProfile postProcessProfile;
}
