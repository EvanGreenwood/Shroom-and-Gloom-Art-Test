#region Usings
using Framework;
using UnityEngine;
using MathBad;
#endregion

public class MoveTowardsCamera : MonoBehaviour
{
    [SerializeField] FloatRange _cameraDstMinMax = new FloatRange(1.5f, 5f);
    Camera _camera;
    Vector3 _startPos;

    // MonoBehaviour
    //----------------------------------------------------------------------------------------------------
    void Awake()
    {
        _camera = SceneUtils.MainCamera;
        _startPos = transform.position;
    }
    void Update()
    {
        Vector3 heading = _camera.transform.position - _startPos;
        float dst = heading.magnitude;
        if(dst > _cameraDstMinMax.Max)
        {
            transform.position = Vector3.Lerp(transform.position, _startPos, Time.deltaTime * 2f);
            return;
        }

        float dstPercent = Mathf.InverseLerp(_cameraDstMinMax.Min, _cameraDstMinMax.Max, dst);
        Vector3 target = _camera.transform.position - heading.normalized * _cameraDstMinMax.Min;
        Vector3 nextPos = Vector3.Lerp(_startPos, target, dstPercent);
        transform.position = Vector3.Lerp(transform.position, nextPos, Time.deltaTime);
    }
}
