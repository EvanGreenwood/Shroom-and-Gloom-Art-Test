#region Usings
using System.Collections.Generic;
using Framework;
using UnityEngine;
using MathBad;
using Unity.Mathematics;

#endregion

public class GloomWorm : MonoBehaviour
{
    [SerializeField] SpriteRenderer _headPrefab;
    [SerializeField] SpriteRenderer _segmentPrefab;
    [SerializeField] int _numSegments = 10;
    [SerializeField] int _segmentSpacing = 3;
    [Space]
    [SerializeField] float _yOffset = 1.5f;
    [SerializeField] float _bodyJiggleSpacing = 1f;
    [SerializeField] float _speedMul = 1f;
    [SerializeField] FloatRange _scaleOverSegments = new FloatRange(1.5f, 3f);
    [SerializeField] Gradient _colorOverSegments = new Gradient();
    [Space]
    [SerializeField] Vector2 _bodyJiggleSize = new Vector2(5f, 2f);
    [SerializeField] Vector2 _bodyJiggleSpeed = new Vector2(1f, 2f);
    [SerializeField] FloatRange _bodyJiggleWobble = new FloatRange(5f, 45f);
    [Space]
    [SerializeField] FloatRange _bodyJiggleSizeFactor = new FloatRange(0.05f, 1f);

    List<SpriteRenderer> _segments = new List<SpriteRenderer>();
    //Vector3 _startPos;

    // MonoBehaviour
    //----------------------------------------------------------------------------------------------------
    void Awake()
    {
        //_startPos = transform.position;
        SpriteRenderer head = Instantiate(_headPrefab);
        head.transform.SetParent(transform);
        head.transform.localScale = Vector3.one * _scaleOverSegments.Min;
        head.transform.localPosition = Vector3.up * _yOffset;

        _segments.Add(head);

        for(int i = 1; i < _numSegments + 1; i++)
        {
            float lerp = mathi.unlerp(i, _numSegments + 1);
            Vector3 pos = new Vector3(0f, _yOffset, i * _segmentSpacing);
            SpriteRenderer segment = Instantiate(_segmentPrefab);
            segment.transform.SetParent(transform);
            
            segment.transform.localPosition = pos;
            segment.transform.localRotation = quaternion.identity;
            segment.transform.localScale = Vector3.one * Mathf.Lerp(_scaleOverSegments.Min, _scaleOverSegments.Max, lerp);
            segment.color = _colorOverSegments.Evaluate(lerp);
            _segments.Add(segment);
       
        }
    }
    void Update()
    {
        StepBody();
    }
    void StepBody()
    {
        for(int i = 0; i < _segments.Count; i++)
        {
            SpriteRenderer bodyPart = _segments[i];

            float lerp = mathi.unlerp(i, _segments.Count);
            float t = Time.time + (lerp * _bodyJiggleSpacing);

            float sizeFactor = Mathf.Lerp(_bodyJiggleSizeFactor.Min, _bodyJiggleSizeFactor.Max, lerp);
            float x = Mathf.Sin((t + _bodyJiggleSpeed.x) * _speedMul) * _bodyJiggleSize.x * sizeFactor;
            float y = Mathf.Sin((t + _bodyJiggleSpeed.y) * _speedMul) * _bodyJiggleSize.y * sizeFactor;
            float angle = Mathf.Sin(t + _bodyJiggleSpeed.x) * Mathf.Lerp(_bodyJiggleWobble.Min, _bodyJiggleWobble.Max, lerp);

            bodyPart.transform.localPosition = new Vector3(x, _yOffset + y, bodyPart.transform.localPosition.z);
            bodyPart.transform.localRotation = Quaternion.Euler(0f, 0f, angle);
        }
    }
}
