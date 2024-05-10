#region Usings
using System.Collections;
using UnityEngine;
using MathBad;
using UnityEngine.Rendering.PostProcessing;
using VladStorm;
#if UNITY_EDITOR
using UnityEditor;
#endif
#endregion

public class MainMenuCanvas : CanvasSingleton<MainMenuCanvas>
{
    [SerializeField] AudioSource _ratAudio;
    [SerializeField] ParticleSystem _ratSystem;
    [SerializeField] PostProcessVolume _ppv;

    ButtonUI[] _buttons;
    PostProcessProfile _ppprofile;
    VHSPro _vhsPro;
    Coroutine _transitionRoutine;
    bool _isStarting;

    void Awake()
    {
        _ppprofile = _ppv.profile;
        _ppprofile.TryGetSettings(out _vhsPro);
        _buttons = FindObjectsOfType<ButtonUI>();
    }

    public void StartAdventure()
    {
        if(_isStarting)
            return;

        _ratAudio.Play();
        _isStarting = true;
        _buttons.Foreach(button => button.Disable());
        SceneTransition.inst.Transition(() => { }, 0.5f, false, true, 1f, "", "");
        _transitionRoutine = StartCoroutine(TransitionRoutine());
    }

    public void Exit()
    {
#if UNITY_EDITOR
        EditorApplication.isPlaying = false;
#else
        Application.Quit();
#endif
    }

    IEnumerator TransitionRoutine()
    {
        var emission = _ratSystem.emission;
        float rateOverTime = emission.rateOverTimeMultiplier;
        emission.rateOverTimeMultiplier = 0f;
        _ratSystem.Play();

        float duration = 1f;
        float t = 0f;

        while(t < duration)
        {
            t += Time.deltaTime;
            float lerp = (t / duration).Clamp01();
            _vhsPro.feedbackFade.value = 1f - lerp;
            emission.rateOverTimeMultiplier = rateOverTime * lerp;
            yield return null;
        }
        yield return WAIT.ForSeconds(1f);

        SCENE.LoadScene(2);
    }
}
