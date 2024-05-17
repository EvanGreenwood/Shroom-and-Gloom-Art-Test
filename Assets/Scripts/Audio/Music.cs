#region Usings
using UnityEngine;
using MathBad;
#endregion

public class Music : MonoBehaviour
{
    AudioClip _clip;
    AudioSource _source;
    Timer2 _volumeTimer = new Timer2(3f, 0f);
    bool _hasInit;

    // Init
    //----------------------------------------------------------------------------------------------------
    public void Init(AudioClip clip)
    {
        _clip = clip;
        
        _source = GetComponent<AudioSource>();
        _source.clip = _clip;
        _source.volume = 0f;
        
        _hasInit = true;
    }

    public void Play()
    {
        if(_source.isPlaying)
            return;

        _volumeTimer.Reset();
    }

    void Update()
    {
        if(!_hasInit)
            return;

        if(!_volumeTimer.hasFinished)
        {
            _volumeTimer.Step(UnityEngine.Time.deltaTime);
            _source.volume = EASE.Evaluate(_volumeTimer.percent.Clamp01(), EaseType.InQuad);
        }
    }
}
