#region Usings
using UnityEngine;
using MathBad;
#endregion

[MenuCreate("SceneData", 0)]
public class SceneData: ScriptableObject
{
    public string title;
    public string description;

    public EffectSoundBank onSceneLoadedAudio;
}