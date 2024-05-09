//  ___                         ___        _                                                          
// | __| _ _  __ _  _ __   ___ | _ \ __ _ | |_  ___                                                   
// | _| | '_|/ _` || '  \ / -_)|   // _` ||  _|/ -_)                                                  
// |_|  |_|  \__,_||_|_|_|\___||_|_\\__,_| \__|\___|                                                  
//                                                                                                    
//----------------------------------------------------------------------------------------------------

#region
using System;
using System.Collections.Generic;
using System.Text;
using Framework;
using UnityEngine;
#endregion

namespace Mainframe
{
public class FrameRate : MonoSingleton<FrameRate>
{
    public const int UDT_SAMPLE_COUNT = 5;

    // Private Fields
    StringBuilder _sb;
    GUIContent _content;

    int _frameRange;
    int[] _fpsBuffer;
    int _fpsBufferIndex;

    List<float> _udtSamples;
    float _udtAverage;

    int _fpsAverage, _fpsHighest, _fpsLowest;
    int _highest, _lowest;
    Timer2 _delay = new Timer2(2f);

    public delegate void FrameRateHandler();
    public event FrameRateHandler onDelayComplete;

    // Public Fields
    public float udtAverage => _udtAverage;
    public int fpsAverage => _fpsAverage;
    public int fpsHighest => _fpsHighest;
    public int fpsLowest => _fpsLowest;
    public int highest => _highest;
    public int lowest => _lowest;
    public GUIContent content => _content;

    // MonoBehaviour
    //----------------------------------------------------------------------------------------------------
    void Awake()
    {
        _sb = new StringBuilder(100);
        _content = new GUIContent();
        _udtSamples = new List<float>();

        _udtAverage = 0f;
        _frameRange = 60;

        _highest = int.MinValue;
        _lowest = int.MaxValue;
    }

    void Update()
    {
        _udtSamples.Add(Time.unscaledDeltaTime);

        if(_udtSamples.Count >= UDT_SAMPLE_COUNT)
            _udtSamples.RemoveAt(0);

        float ustSum = 0;
        foreach(float sample in _udtSamples)
            ustSum += sample;
        _udtAverage = ustSum / UDT_SAMPLE_COUNT;

        if(_fpsBuffer == null || _fpsBuffer.Length != _frameRange)
        {
            if(_frameRange <= 0)
                _frameRange = 1;

            _fpsBuffer = new int[_frameRange];
            _fpsBufferIndex = 0;
        }

        _fpsBuffer[_fpsBufferIndex++] = (int)(1f / Time.unscaledDeltaTime);
        if(_fpsBufferIndex >= _frameRange)
            _fpsBufferIndex = 0;

        int sum = 0;
        int high = 0;
        int low = int.MaxValue;

        for(int i = 0; i < _frameRange; i++)
        {
            int fps = _fpsBuffer[i];
            sum += fps;
            if(fps > high) high = fps;
            if(fps < low) low = fps;
        }

        _fpsAverage = (int)(sum._float() / _frameRange);
        _fpsHighest = high;
        _fpsLowest = low;

        if(_delay.wasFinishedThisFrame)
        {
            onDelayComplete?.Invoke();
        }
        if(_delay.hasFinished)
        {
            if(_fpsHighest > _highest) _highest = _fpsHighest;
            if(_fpsLowest < _lowest) _lowest = _fpsLowest;
        }
        else { _delay.Step(Time.deltaTime); }
        _content.text = GetString(true);
    }

    public string GetString(bool full = false)
    {
        _sb.Clear();
        if(full)
        {
            _sb.Append("DELTAT: ").Append($"{_udtAverage:0.0000}\n");
            _sb.Append("AVG:    ").Append($"{_fpsAverage:0000}\n");
            string high = _delay.hasFinished ? $": {_highest:0000}" : "";
            string low = _delay.hasFinished ? $": {_lowest:0000}" : "";

            _sb.Append("HIGH:   ").Append($"{_fpsHighest:0000} {high}\n");
            _sb.Append("LOW:    ").Append($"{_fpsLowest:0000} {low}\n");
        }
        else
        {
            _sb.Append("DELTAT: ").Append($"{_udtAverage:0.0000}\n");
            _sb.Append("AVG:    ").Append($"{_fpsAverage:0000}");
        }
        return _sb.ToString();
    }
}
}
