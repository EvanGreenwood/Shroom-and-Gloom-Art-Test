#region Usings
using UnityEngine;
using MathBad;
using NaughtyAttributes;
#endregion
[ExecuteAlways]
[RequireComponent(typeof(LineRenderer))]
public class Limb : MonoBehaviour
{
    [SerializeField] Transform[] _bones;
    LineRenderer _lr;

    // MonoBehaviour
    //----------------------------------------------------------------------------------------------------
    void Awake()
    {
        _lr = GetComponent<LineRenderer>();
        _lr.positionCount = _bones.Length;
        StepLine();
    }
    void Update() {StepLine();}

    void StepLine()
    {
        if(_lr == null)
        {
            _lr = GetComponent<LineRenderer>();
            _lr.positionCount = _bones.Length;
        }
        _bones.For(i =>
        {
            Vector3 pos = transform.InverseTransformPoint(_bones[i].position);
            _lr.SetPosition(i, pos);
        });
    }
}
