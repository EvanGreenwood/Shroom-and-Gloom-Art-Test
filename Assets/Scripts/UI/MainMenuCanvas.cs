#region Usings
using System.Collections;
using UnityEngine;
using MathBad;
#if UNITY_EDITOR
using UnityEditor;
#endif
#endregion

public class MainMenuCanvas : CanvasSingleton<MainMenuCanvas>
{
    [SerializeField] EffectSoundBank _ratSound;
    [SerializeField] ParticleSystem _ratSystem;
    ButtonUI[] _buttons;
    Coroutine _transitionRoutine;
    Timer2 _ratSoundTimer = new Timer2(0.1f, 0.05f);
    bool _isStarting;
    void Awake()
    {
        _buttons = FindObjectsOfType<ButtonUI>();
    }
    void Update()
    {
        if(!_isStarting)
            return;
        _ratSoundTimer.Step(Time.deltaTime);
        if(_ratSoundTimer.hasFinished)
        {
            _ratSound.Play(RNG.Vector3(new Vector3(-10f, -10f, 0f), new Vector3(10f, 10f, 0f)));
            _ratSoundTimer.Reset();
        }
    }

    public void StartAdventure()
    {
        if(_isStarting)
            return;
        _ratSoundTimer.ForceComplete();
        _isStarting = true;
        _buttons.Foreach(button => button.Disable());
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
        _ratSystem.Play();
        yield return WAIT.ForSeconds(3f);

        SceneTransition.inst.Transition(() => SCENE.LoadScene(2),
                                        0f, false, true, 1f, 
                                        "", "", 
                                        RGB.grey, RGB.black);
    }
}
